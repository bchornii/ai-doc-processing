using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Activities;

public class ClassifyDocumentActivity(
    IClassifyDocumentActivity service,
    ILogger<ClassifyDocumentActivity> logger)
{
    [Function(nameof(ClassifyDocumentActivity))]
    public async Task<ConfidenceResult<Classifications>?> RunAsync(
        [ActivityTrigger] ClassifyDocumentActivityInput input,
        FunctionContext executionContext)
    {
        var instanceId = executionContext.InvocationId;
        var correlationId = input.CorrelationId;

        logger.LogInformation(
            "Starting activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ClassifyDocumentActivity),
            instanceId,
            correlationId);

        var result = await service.ExecuteAsync(
            input.ContainerName, input.BlobName, correlationId, CancellationToken.None);

        logger.LogInformation(
            "Completed activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ClassifyDocumentActivity),
            instanceId,
            correlationId);

        return result;
    }
}
