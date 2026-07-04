using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Orchestrations;

public static class ProcessDocumentWorkflow
{
    [Function(nameof(ProcessDocumentWorkflow))]
    public static Task<WorkflowResult> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<ProcessDocumentFolderInput>()!;
        var logger = context.CreateReplaySafeLogger(nameof(ProcessDocumentWorkflow));
        logger.LogInformation(
            "Starting ProcessDocumentWorkflow for Folder={FolderName} CorrelationId={CorrelationId}",
            input.Folder.Name,
            input.CorrelationId);

        return Task.FromResult(WorkflowResult.Empty);
    }
}
