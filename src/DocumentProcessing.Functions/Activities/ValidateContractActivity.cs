using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Activities;

public class ValidateContractActivity(
    IValidateContractActivity service,
    ILogger<ValidateContractActivity> logger)
{
    [Function(nameof(ValidateContractActivity))]
    public async Task<ValidationResult> RunAsync(
        [ActivityTrigger] ValidateContractActivityInput input,
        FunctionContext executionContext)
    {
        var instanceId = executionContext.InvocationId;
        var correlationId = input.CorrelationId;

        logger.LogInformation(
            "Starting activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ValidateContractActivity),
            instanceId,
            correlationId);

        var result = await service.ExecuteAsync(
            input.ContractName, input.Data!, correlationId, CancellationToken.None);

        logger.LogInformation(
            "Completed activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ValidateContractActivity),
            instanceId,
            correlationId);

        return result;
    }
}
