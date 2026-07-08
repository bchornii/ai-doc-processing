using DocumentProcessing.Functions.Models;
using DocumentProcessing.Functions.Orchestrations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using static DocumentProcessing.Functions.Extensions.FunctionsHttpExtensions;

namespace DocumentProcessing.Functions.Triggers;

public class ProcessDocumentsQueueTrigger(ILogger<ProcessDocumentsQueueTrigger> logger)
{
    [Function(nameof(ProcessDocumentsQueueTrigger))]
    public async Task RunAsync(
        [QueueTrigger("documents", Connection = "AzureWebJobsStorage")] string message,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var correlationId = CreateCorrelationId();

        var request = message.TryReadDocumentBatchRequest();

        if (request is null || request.InvalidContainerName)
        {
            throw new InvalidOperationException("Deserialized request is null or ContainerName is empty.");
        }

        var input = new ProcessDocumentBatchInput(request, correlationId);

        logger.LogInformation("Starting orchestration for correlationId {CorrelationId}", correlationId);
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(ProcessDocumentBatchWorkflow), input);
        logger.LogInformation(
            "Started orchestration {OrchestrationInstanceId} for correlationId {CorrelationId}",
            instanceId,
            correlationId);
    }
}
