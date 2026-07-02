using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;

namespace DocumentProcessing.Infrastructure.Activities;

public class PlaceholderClassifyDocumentActivity : IClassifyDocumentActivity
{
    public Task<ConfidenceResult<Classifications>> ExecuteAsync(string containerName, string blobName, string correlationId, CancellationToken ct)
    {
        var result = new ConfidenceResult<Classifications>(
            new Classifications([new PageClassification(ClassificationTypes.Invoice, 1, 1)]),
            1.0);

        return Task.FromResult(result);
    }
}
