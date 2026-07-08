using System.Text.Json;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Activities;
using DocumentProcessing.Functions.Models;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Orchestrations.Processing;

public class ProcessBoundedDocument : IProcessingStrategy
{
    public async Task RunAsync(
        ProcessingInput input,
        ProcessingContext processingContext,
        CancellationToken cancellationToken = default)
    {
        var (context, folder, blobName, correlationId, retryPolicy) = input;
        var logger = context.CreateReplaySafeLogger(nameof(ProcessDocumentWorkflow));

        var detectBoundariesInput =
            new DetectBoundariesActivityInput(folder.ContainerName, blobName, correlationId);
        var boundaryResult = await context
            .CallActivityAsync<BoundaryDetectionResult>(nameof(DetectBoundariesActivity), detectBoundariesInput, retryPolicy);

        if (boundaryResult.Segments.Count == 0)
        {
            logger.LogError(
                "No segments detected for document {DocumentFileName} CorrelationId={CorrelationId}",
                blobName,
                correlationId);
            processingContext.AddError($"No segments detected for {blobName}");
            return;
        }

        var segmentTasks = boundaryResult.Segments
            .Select(segment =>
            {
                var documentSegmentInput = new DocumentSegmentInput(
                    segment,
                    folder.ContainerName,
                    folder.Name,
                    blobName,
                    correlationId);

                return context
                    .CallSubOrchestratorAsync<WorkflowResult>(
                        nameof(ProcessDocumentSegmentWorkflow),
                        documentSegmentInput,
                        retryPolicy);
            })
            .ToList();

        var segmentResults = await Task.WhenAll(segmentTasks);

        var persistSegmentsInput = new PersistResultActivityInput(
            folder.ContainerName,
            blobName,
            JsonSerializer.SerializeToUtf8Bytes(segmentResults),
            correlationId);

        await context.CallActivityAsync<bool>(nameof(PersistResultActivity), persistSegmentsInput, retryPolicy);

        processingContext.AddMessages(segmentResults.SelectMany(r => r.Messages));
        processingContext.AddErrors(segmentResults.SelectMany(r => r.Errors));
    }
}
