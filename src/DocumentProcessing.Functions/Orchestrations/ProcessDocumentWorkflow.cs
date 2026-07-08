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
    /// <summary>
    /// Orchestrator function that processes a folder of documents. It classifies each document,
    /// persists the classification result, and applies the appropriate processing strategy based on the classification.
    /// If any activity fails, the error is captured and logged, but processing continues for the remaining documents.
    /// </summary>
    /// <remarks>If the classification confidence is below a defined threshold, the document is skipped and an error message is logged.</remarks>
    /// <remarks>This function is designed to be idempotent and can be safely retried in case of transient failures.</remarks>
    /// <param name="context">The orchestration context.</param>
    /// <returns>A <see cref="WorkflowResult"/> containing the messages and errors encountered during processing.</returns>
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
            try
            {
                await ProcessDocumentAsync(
                    context,
                    folder,
                    documentFileName,
                    correlationId,
                    retryPolicy,
                    logger,
                    processingCtx);
            }
            catch (TaskFailedException e)
            {
                var exceptionMessage = e.InnerException?.Message ?? e.Message;
                var exceptionMessageDetails =
                    $"Task name: {e.TaskName}, Exception Message: {exceptionMessage}, Failure details message: {e.FailureDetails.ErrorMessage}, Failure type: {e.FailureDetails.ErrorType}";
                processingCtx.AddError(exceptionMessageDetails);
            }
        }

        logger.LogInformation(
            "Completed ProcessDocumentWorkflow for Folder={FolderName} CorrelationId={CorrelationId}. Messages={MessageCount}, Errors={ErrorCount}",
            input.Folder.Name,
            input.CorrelationId,
            processingCtx.Messages.Count,
            processingCtx.Errors.Count);

        return new WorkflowResult(processingCtx.Messages, processingCtx.Errors);
    }

    private static async Task ProcessDocumentAsync(
        TaskOrchestrationContext context,
        DocumentFolder folder,
        string documentFileName,
        string correlationId,
        TaskOptions retryPolicy,
        ILogger logger,
        ProcessingContext processingCtx)
    {
        var blobName = folder.BlobPath(documentFileName);
        var classifyDocumentInput =
            new ClassifyDocumentActivityInput(folder.ContainerName, blobName, correlationId);
        var classificationResult = await context
            .CallActivityAsync<ConfidenceResult<Classifications>?>(nameof(ClassifyDocumentActivity), classifyDocumentInput, retryPolicy);

        if (classificationResult is null)
        {
            logger.LogError(
                "Classification returned null for document {DocumentFileName} CorrelationId={CorrelationId}",
                documentFileName,
                correlationId);
            processingCtx.AddError($"Classification returned null for {documentFileName}");
            return;
        }

        var persistClassificationResultInput = new PersistResultActivityInput(
            folder.ContainerName,
            blobName,
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
            return;
        }

        var processingStrategy = RetrieveDocumentProcessingStrategy(classificationResult.Data?.PageClassifications);
        var noProcessingStrategyDefined = processingStrategy.GetType() == typeof(NoOp);

        if (noProcessingStrategyDefined)
        {
            logger.LogInformation(
                "No processing required for document {DocumentFileName} type {DetectedType} CorrelationId={CorrelationId}",
                documentFileName,
                processingStrategy,
                correlationId);
        }

        var processingInput = new ProcessingInput(context, folder, blobName, correlationId, retryPolicy);
        await processingStrategy.RunAsync(processingInput, processingCtx);
    }
}
