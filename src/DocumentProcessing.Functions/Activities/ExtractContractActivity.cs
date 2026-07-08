using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Activities;

public class ExtractContractActivity(
    IExtractContractActivity service,
    ILogger<ExtractContractActivity> logger)
{
    [Function(nameof(ExtractContractActivity))]
    public async Task<ConfidenceResult<ContractData>> RunAsync(
        [ActivityTrigger] ExtractContractActivityInput input,
        FunctionContext executionContext)
    {
        var instanceId = executionContext.InvocationId;
        var correlationId = input.CorrelationId;

        logger.LogInformation(
            "Starting activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ExtractContractActivity),
            instanceId,
            correlationId);

        var result = await service.ExecuteAsync(
            input.ContainerName, input.BlobName, input.PageStart, input.PageEnd, correlationId, CancellationToken.None);

        logger.LogInformation(
            "Completed activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ExtractContractActivity),
            instanceId,
            correlationId);

        return result;
    }
}
