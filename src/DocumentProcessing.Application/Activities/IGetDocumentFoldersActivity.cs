using DocumentProcessing.Contracts;
using DocumentProcessing.Core;

namespace DocumentProcessing.Application.Activities;

public interface IGetDocumentFoldersActivity
{
    Task<DocumentFolders> ExecuteAsync(DocumentBatchRequest request, string correlationId, CancellationToken ct);
}
