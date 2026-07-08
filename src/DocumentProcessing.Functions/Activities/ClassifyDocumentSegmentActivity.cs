using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Activities;

public class ClassifyDocumentSegmentActivity(
    IClassifyDocumentActivity service,
    ILogger<ClassifyDocumentSegmentActivity> logger)
{
    [Function(nameof(ClassifyDocumentSegmentActivity))]
    public async Task<ConfidenceResult<Classifications>?> RunAsync(
        [ActivityTrigger] ClassifyDocumentSegmentActivityInput input,
        FunctionContext executionContext)
    {
        var instanceId = executionContext.InvocationId;
        var correlationId = input.CorrelationId;

        logger.LogInformation(
            "Starting activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId} pages {PageStart}-{PageEnd}",
            nameof(ClassifyDocumentSegmentActivity),
            instanceId,
            correlationId,
            input.PageStart,
            input.PageEnd);

        var result = await service.ExecuteSegmentAsync(
            input.ContainerName, input.BlobName, input.PageStart, input.PageEnd, correlationId, CancellationToken.None);

        logger.LogInformation(
            "Completed activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(ClassifyDocumentSegmentActivity),
            instanceId,
            correlationId);

        return result;
    }
}
