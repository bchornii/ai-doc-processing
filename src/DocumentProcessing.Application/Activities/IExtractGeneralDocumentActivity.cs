using DocumentProcessing.Core;

namespace DocumentProcessing.Application.Activities;

public interface IExtractGeneralDocumentActivity
{
    Task<ConfidenceResult<GeneralDocumentData>> ExecuteAsync(string containerName, string blobName, int? pageStart, int? pageEnd, string correlationId, CancellationToken ct);
}
