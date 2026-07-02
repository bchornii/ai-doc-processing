using DocumentProcessing.Core;

namespace DocumentProcessing.Application.Activities;

public interface IExtractInvoiceActivity
{
    Task<ConfidenceResult<Invoice>> ExecuteAsync(string containerName, string blobName, int? pageStart, int? pageEnd, string correlationId, CancellationToken ct);
}
