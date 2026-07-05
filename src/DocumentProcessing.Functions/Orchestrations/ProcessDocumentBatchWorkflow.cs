using DocumentProcessing.Core;
using DocumentProcessing.Functions.Activities;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using static DocumentProcessing.Functions.Orchestrations.WorkflowUtils;

namespace DocumentProcessing.Functions.Orchestrations;

public static class ProcessDocumentBatchWorkflow
{
    [Function(nameof(ProcessDocumentBatchWorkflow))]
    public static async Task<WorkflowResult> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<ProcessDocumentBatchInput>()!;
        var logger = context.CreateReplaySafeLogger(nameof(ProcessDocumentBatchWorkflow));
        logger.LogInformation("Starting ProcessDocumentBatchWorkflow for CorrelationId={CorrelationId}", input.CorrelationId);

        var retryPolicy = CreateActivityRetryPolicy();

        var folders = await context.CallActivityAsync<DocumentFolders>(
            nameof(GetDocumentFoldersActivity), input, retryPolicy);

        if (folders.Folders.Count == 0)
        {
            logger.LogInformation(
                "No document folders found for CorrelationId={CorrelationId}. Returning empty result.",
                input.CorrelationId);
            return WorkflowResult.Empty;
        }

        logger.LogInformation(
            "Fan-out: processing {FolderCount} folders for CorrelationId={CorrelationId}",
            folders.Folders.Count,
            input.CorrelationId);

        var folderTasks = folders.Folders
            .Select(folder =>
            {
                var processDocFolderInput = new ProcessDocumentFolderInput(folder, input.CorrelationId);
                return context
                    .CallSubOrchestratorAsync<WorkflowResult>(nameof(ProcessDocumentWorkflow), processDocFolderInput, retryPolicy);
            })
            .ToList();

        var folderResults = await Task.WhenAll(folderTasks);

        var allMessages = folderResults.SelectMany(r => r.Messages).ToList();
        var allErrors = folderResults.SelectMany(r => r.Errors).ToList();

        logger.LogInformation(
            "Completed ProcessDocumentBatchWorkflow for CorrelationId={CorrelationId}. Messages={MessageCount}, Errors={ErrorCount}",
            input.CorrelationId,
            allMessages.Count,
            allErrors.Count);

        return new WorkflowResult(allMessages, allErrors);
    }
}
