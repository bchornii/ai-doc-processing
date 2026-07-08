using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;

namespace DocumentProcessing.Infrastructure.Activities;

public class PlaceholderDetectBoundariesActivity : IDetectBoundariesActivity
{
    public Task<BoundaryDetectionResult> ExecuteAsync(string containerName, string blobName, string correlationId, CancellationToken ct)
    {
        var result = new BoundaryDetectionResult(
        [
            new DocumentSegment(SegmentIndex: 0, PageStart: 1, PageEnd: 3, DetectedType: ClassificationTypes.Invoice),
            new DocumentSegment(SegmentIndex: 1, PageStart: 4, PageEnd: 6, DetectedType: ClassificationTypes.Contract),
        ]);

        return Task.FromResult(result);
    }
}
