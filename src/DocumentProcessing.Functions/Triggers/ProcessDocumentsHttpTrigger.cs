using DocumentProcessing.Functions.Extensions;
using DocumentProcessing.Functions.Models;
using DocumentProcessing.Functions.Orchestrations;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Triggers;

public class ProcessDocumentsHttpTrigger(ILogger<ProcessDocumentsHttpTrigger> logger)
{
    [Function(nameof(ProcessDocumentsHttpTrigger))]
    public async Task<IResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "process-documents")] HttpRequest req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var request = await req.TryReadDocumentBatchRequestAsync();

        if (request.InvalidContainerName)
        {
            return FunctionsHttpExtensions.InvalidStorageContainer();
        }

        var correlationId = req.GetOrCreateCorrelationId();
        var input = new ProcessDocumentBatchInput(request, correlationId);

        logger.LogInformation("Starting orchestration for correlationId {CorrelationId}", correlationId);
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(ProcessDocumentBatchWorkflow), input);
        logger.LogInformation(
            "Started orchestration {OrchestrationInstanceId} for correlationId {CorrelationId}",
            instanceId,
            correlationId);

        var payload = client
            .CreateHttpManagementPayload(instanceId);

        return TypedResults
            .Accepted(uri: payload.StatusQueryGetUri, value: payload);
    }
}
