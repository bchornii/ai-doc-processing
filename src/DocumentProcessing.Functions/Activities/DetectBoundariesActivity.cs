using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Activities;

public class DetectBoundariesActivity(
    IDetectBoundariesActivity service,
    ILogger<DetectBoundariesActivity> logger)
{
    [Function(nameof(DetectBoundariesActivity))]
    public async Task<BoundaryDetectionResult> RunAsync(
        [ActivityTrigger] DetectBoundariesActivityInput input,
        FunctionContext executionContext)
    {
        var instanceId = executionContext.InvocationId;
        var correlationId = input.CorrelationId;

        logger.LogInformation(
            "Starting activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(DetectBoundariesActivity),
            instanceId,
            correlationId);

        var result = await service.ExecuteAsync(
            input.ContainerName, input.BlobName, correlationId, CancellationToken.None);

        logger.LogInformation(
            "Completed activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(DetectBoundariesActivity),
            instanceId,
            correlationId);

        return result;
    }
}
