using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Activities;

public class ExtractGeneralDocumentActivity(
    IExtractGeneralDocumentActivity service,
    ILogger<ExtractGeneralDocumentActivity> logger)
{
    [Function(nameof(ExtractGeneralDocumentActivity))]
    public async Task<ConfidenceResult<GeneralDocumentData>> RunAsync(
        [ActivityTrigger] ExtractGeneralDocumentActivityInput input,
        FunctionContext executionContext)
    {
        var instanceId = executionContext.InvocationId;
        var correlationId = input.CorrelationId;

        logger.LogInformation(
            "Starting activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ExtractGeneralDocumentActivity),
            instanceId,
            correlationId);

        var result = await service.ExecuteAsync(
            input.ContainerName, input.BlobName, input.PageStart, input.PageEnd, correlationId, CancellationToken.None);

        logger.LogInformation(
            "Completed activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ExtractGeneralDocumentActivity),
            instanceId,
            correlationId);

        return result;
    }
}
