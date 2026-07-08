using System.Text.Json;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Activities;
using DocumentProcessing.Functions.Models;
using DocumentProcessing.Functions.Orchestrations.Processing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using static DocumentProcessing.Core.DocumentProcessingConstants;
using static DocumentProcessing.Functions.Orchestrations.WorkflowUtils;

namespace DocumentProcessing.Functions.Orchestrations;

public static class ProcessDocumentSegmentWorkflow
{
    [Function(nameof(ProcessDocumentSegmentWorkflow))]
    public static async Task<WorkflowResult> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<DocumentSegmentInput>()!;
        var logger = context.CreateReplaySafeLogger(nameof(ProcessDocumentSegmentWorkflow));

        logger.LogInformation(
            "Starting ProcessDocumentSegmentWorkflow for SegmentIndex={SegmentIndex} CorrelationId={CorrelationId}",
            input.Segment.SegmentIndex,
            input.CorrelationId);

        var retryPolicy = CreateActivityRetryPolicy();
        var processingCtx = new ProcessingContext();

        var classifyInput = new ClassifyDocumentSegmentActivityInput(
            input.ContainerName,
            input.BlobName,
            input.Segment.PageStart,
            input.Segment.PageEnd,
            input.CorrelationId);
        var classificationResult = await context
            .CallActivityAsync<ConfidenceResult<Classifications>?>(nameof(ClassifyDocumentSegmentActivity), classifyInput, retryPolicy);

        if (classificationResult is null)
        {
            logger.LogError(
                "Classification returned null for segment {SegmentIndex} CorrelationId={CorrelationId}",
                input.Segment.SegmentIndex,
                input.CorrelationId);
            processingCtx.AddError($"Classification returned null for segment {input.Segment.SegmentIndex}");
            return new WorkflowResult(processingCtx.Messages, processingCtx.Errors);
        }

        var persistClassificationInput = new PersistResultActivityInput(
            input.ContainerName,
            input.BlobName,
            JsonSerializer.SerializeToUtf8Bytes(classificationResult),
            input.CorrelationId);
        await context.CallActivityAsync<bool>(nameof(PersistResultActivity), persistClassificationInput, retryPolicy);

        if (classificationResult.OverallConfidence < ConfidenceThreshold)
        {
            logger.LogInformation(
                "Skipping segment {SegmentIndex}: confidence {Confidence} below threshold {Threshold} CorrelationId={CorrelationId}",
                input.Segment.SegmentIndex,
                classificationResult.OverallConfidence,
                ConfidenceThreshold,
                input.CorrelationId);
            processingCtx.AddError($"Skipped segment {input.Segment.SegmentIndex}: confidence {classificationResult.OverallConfidence} below threshold");
            return new WorkflowResult(processingCtx.Messages, processingCtx.Errors);
        }

        var processingStrategy = RetrieveDocumentProcessingStrategy(classificationResult.Data?.PageClassifications);
        if (processingStrategy is ProcessBoundedDocument)
        {
            logger.LogInformation(
                "Segment {SegmentIndex} classified as BoundedDocument — falling back to General processing CorrelationId={CorrelationId}",
                input.Segment.SegmentIndex,
                input.CorrelationId);
            processingStrategy = new ProcessGeneralDocument();
        }

        var folder = new DocumentFolder(input.ContainerName, input.FolderName, [input.BlobName]);
        var processingInput = new ProcessingInput(context, folder, input.BlobName, input.CorrelationId, retryPolicy);
        await processingStrategy.RunAsync(processingInput, processingCtx);

        logger.LogInformation(
            "Completed ProcessDocumentSegmentWorkflow for SegmentIndex={SegmentIndex} CorrelationId={CorrelationId}. Messages={MessageCount}, Errors={ErrorCount}",
            input.Segment.SegmentIndex,
            input.CorrelationId,
            processingCtx.Messages.Count,
            processingCtx.Errors.Count);

        return new WorkflowResult(processingCtx.Messages, processingCtx.Errors);
    }
}
