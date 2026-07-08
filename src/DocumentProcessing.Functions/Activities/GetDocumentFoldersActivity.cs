using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DocumentProcessing.Functions.Activities;

public class GetDocumentFoldersActivity(
    IGetDocumentFoldersActivity service,
    ILogger<GetDocumentFoldersActivity> logger)
{
    [Function(nameof(GetDocumentFoldersActivity))]
    public async Task<DocumentFolders> RunAsync(
        [ActivityTrigger] ProcessDocumentBatchInput input,
        FunctionContext executionContext)
    {
        var instanceId = executionContext.InvocationId;
        var correlationId = input.CorrelationId;

        logger.LogInformation(
            "Starting activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(GetDocumentFoldersActivity),
            instanceId,
            correlationId);

        var result = await service.ExecuteAsync(input.Request, correlationId, CancellationToken.None);

        logger.LogInformation(
            "Completed activity {ActivityName} for orchestrationInstanceId {OrchestrationInstanceId} correlationId {CorrelationId}",
            nameof(GetDocumentFoldersActivity),
            instanceId,
            correlationId);

        return result;
    }
}
