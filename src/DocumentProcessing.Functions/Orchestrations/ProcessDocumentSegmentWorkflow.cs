using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Orchestrations;

public static class ProcessDocumentSegmentWorkflow
{
    [Function(nameof(ProcessDocumentSegmentWorkflow))]
    public static Task<WorkflowResult> RunAsync(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<DocumentSegmentInput>()!;
        var logger = context.CreateReplaySafeLogger(nameof(ProcessDocumentSegmentWorkflow));
        logger.LogInformation(
            "Starting ProcessDocumentSegmentWorkflow for SegmentIndex={SegmentIndex} CorrelationId={CorrelationId}",
            input.Segment.SegmentIndex,
            input.CorrelationId);

        return Task.FromResult(WorkflowResult.Empty);
    }
}
