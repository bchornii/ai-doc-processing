using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Orchestrations;

public static class ProcessDocumentBatchWorkflow
{
    [Function(nameof(ProcessDocumentBatchWorkflow))]
    public static Task<WorkflowResult> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<ProcessDocumentBatchInput>()!;
        var logger = context.CreateReplaySafeLogger(nameof(ProcessDocumentBatchWorkflow));
        logger.LogInformation("Starting ProcessDocumentBatchWorkflow for CorrelationId={CorrelationId}", input.CorrelationId);

        return Task.FromResult(WorkflowResult.Empty);
    }
}
