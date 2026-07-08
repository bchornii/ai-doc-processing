using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;

namespace DocumentProcessing.Infrastructure.Activities;

public class PlaceholderExtractGeneralDocumentActivity : IExtractGeneralDocumentActivity
{
    public Task<ConfidenceResult<GeneralDocumentData>> ExecuteAsync(string containerName, string blobName, int? pageStart, int? pageEnd, string correlationId, CancellationToken ct)
    {
        var data = new GeneralDocumentData(
            SchemaName: "general-v1",
            Fields:
            [
                new GeneralDocumentField("Title", "Sample Document"),
                new GeneralDocumentField("Author", "Contoso"),
            ]);

        return Task.FromResult(new ConfidenceResult<GeneralDocumentData>(data, 1.0));
    }
}
