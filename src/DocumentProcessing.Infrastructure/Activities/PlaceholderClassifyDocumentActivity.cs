using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;

namespace DocumentProcessing.Infrastructure.Activities;

public class PlaceholderClassifyDocumentActivity : IClassifyDocumentActivity
{
    public async Task<ConfidenceResult<Classifications>?> ExecuteAsync(string containerName, string blobName, string correlationId, CancellationToken ct)
    {
        var result = new ConfidenceResult<Classifications>(
            new Classifications([new PageClassification(ClassificationTypes.BoundedDocument, 1, 6)]),
            1.0);

        return result;
    }

    public async Task<ConfidenceResult<Classifications>?> ExecuteSegmentAsync(string containerName, string blobName, int pageStart, int pageEnd, string correlationId, CancellationToken ct)
    {
        var result = new ConfidenceResult<Classifications>(
            new Classifications([new PageClassification(ClassificationTypes.Invoice, pageStart, pageEnd)]),
            1.0);

        return result;
    }
}
