using DocumentProcessing.Application.Activities;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Activities;

public class PersistResultActivity(
    IPersistResultActivity service,
    ILogger<PersistResultActivity> logger)
{
    [Function(nameof(PersistResultActivity))]
    public async Task<bool> RunAsync(
        [ActivityTrigger] PersistResultActivityInput input,
        FunctionContext executionContext)
    {
        var instanceId = executionContext.InvocationId;
        var correlationId = input.CorrelationId;

        logger.LogInformation(
            "Starting activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(PersistResultActivity),
            instanceId,
            correlationId);

        var result = await service.ExecuteAsync(
            input.ContainerName, input.BlobName, input.Content, correlationId, CancellationToken.None);

        logger.LogInformation(
            "Completed activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(PersistResultActivity),
            instanceId,
            correlationId);

        return result;
    }
}
