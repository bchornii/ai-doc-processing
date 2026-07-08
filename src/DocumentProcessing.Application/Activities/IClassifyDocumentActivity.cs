using DocumentProcessing.Core;

namespace DocumentProcessing.Application.Activities;

public interface IClassifyDocumentActivity
{
    Task<ConfidenceResult<Classifications>?> ExecuteAsync(string containerName, string blobName, string correlationId, CancellationToken ct);

    Task<ConfidenceResult<Classifications>?> ExecuteSegmentAsync(string containerName, string blobName, int pageStart, int pageEnd, string correlationId, CancellationToken ct);
}
