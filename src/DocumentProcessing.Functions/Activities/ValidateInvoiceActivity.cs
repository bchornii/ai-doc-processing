using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Activities;

public class ValidateInvoiceActivity(
    IValidateInvoiceActivity service,
    ILogger<ValidateInvoiceActivity> logger)
{
    [Function(nameof(ValidateInvoiceActivity))]
    public async Task<ValidationResult> RunAsync(
        [ActivityTrigger] ValidateInvoiceActivityInput input,
        FunctionContext executionContext)
    {
        var instanceId = executionContext.InvocationId;
        var correlationId = input.CorrelationId;

        logger.LogInformation(
            "Starting activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ValidateInvoiceActivity),
            instanceId,
            correlationId);

        var result = await service.ExecuteAsync(
            input.InvoiceName, input.Data!, correlationId, CancellationToken.None);

        logger.LogInformation(
            "Completed activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ValidateInvoiceActivity),
            instanceId,
            correlationId);

        return result;
    }
}
