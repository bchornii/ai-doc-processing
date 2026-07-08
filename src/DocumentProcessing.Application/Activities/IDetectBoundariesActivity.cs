using DocumentProcessing.Core;

namespace DocumentProcessing.Application.Activities;

public interface IDetectBoundariesActivity
{
    Task<BoundaryDetectionResult> ExecuteAsync(string containerName, string blobName, string correlationId, CancellationToken ct);
}
