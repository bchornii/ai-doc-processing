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

public static class ProcessDocumentWorkflow
{
    [Function(nameof(ProcessDocumentWorkflow))]
    public static async Task<WorkflowResult> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<ProcessDocumentFolderInput>()!;
        var logger = context.CreateReplaySafeLogger(nameof(ProcessDocumentWorkflow));

        logger.LogInformation(
            "Starting ProcessDocumentWorkflow for Folder={FolderName} CorrelationId={CorrelationId}",
            input.Folder.Name,
            input.CorrelationId);

        var retryPolicy = CreateActivityRetryPolicy();
        var processingCtx = new ProcessingContext();

        var folder = input.Folder;
        var correlationId = input.CorrelationId;

        foreach (var documentFileName in input.Folder.DocumentFileNames)
        {
            var classifyDocumentInput =
                new ClassifyDocumentActivityInput(folder.ContainerName, documentFileName, correlationId);
            var classificationResult = await context
                .CallActivityAsync<ConfidenceResult<Classifications>?>(nameof(ClassifyDocumentActivity), classifyDocumentInput, retryPolicy);

            if (classificationResult is null)
            {
                logger.LogError(
                    "Classification returned null for document {DocumentFileName} CorrelationId={CorrelationId}",
                    documentFileName,
                    correlationId);
                processingCtx.AddError($"Classification returned null for {documentFileName}");
                break;
            }

            var persistClassificationResultInput = new PersistResultActivityInput(
                folder.ContainerName,
                documentFileName,
                JsonSerializer.SerializeToUtf8Bytes(classificationResult),
                correlationId);
            await context.CallActivityAsync<bool>(nameof(PersistResultActivity), persistClassificationResultInput, retryPolicy);

            var lowConfidenceClarification = classificationResult.OverallConfidence < ConfidenceThreshold;
            if (lowConfidenceClarification)
            {
                logger.LogInformation(
                    "Skipping document {DocumentFileName}: confidence {Confidence} below threshold {Threshold} CorrelationId={CorrelationId}",
                    documentFileName,
                    classificationResult.OverallConfidence,
                    ConfidenceThreshold,
                    correlationId);
                processingCtx.AddError($"Skipped {documentFileName}: confidence {classificationResult.OverallConfidence} below threshold");
                break;
            }

            var processingStrategy = RetrieveMatchingProcessingStrategy(classificationResult.Data?.PageClassifications);
            var noProcessingStrategyDefined = processingStrategy.GetType() == typeof(NoOp);

            if (noProcessingStrategyDefined)
            {
                logger.LogInformation(
                    "No processing required for document {DocumentFileName} type {DetectedType} CorrelationId={CorrelationId}",
                    documentFileName,
                    processingStrategy,
                    correlationId);
            }

            var processingInput = new ProcessingInput(context, folder, documentFileName, correlationId, retryPolicy);
            await processingStrategy.RunAsync(processingInput, processingCtx);
        }

        logger.LogInformation(
            "Completed ProcessDocumentWorkflow for Folder={FolderName} CorrelationId={CorrelationId}. Messages={MessageCount}, Errors={ErrorCount}",
            input.Folder.Name,
            input.CorrelationId,
            processingCtx.Messages.Count,
            processingCtx.Errors.Count);

        return new WorkflowResult(processingCtx.Messages, processingCtx.Errors);
    }
}
