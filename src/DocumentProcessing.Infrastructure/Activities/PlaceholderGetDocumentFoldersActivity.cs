using DocumentProcessing.Application.Activities;
using DocumentProcessing.Contracts;
using DocumentProcessing.Core;

namespace DocumentProcessing.Infrastructure.Activities;

public class PlaceholderGetDocumentFoldersActivity : IGetDocumentFoldersActivity
{
    public Task<DocumentFolders> ExecuteAsync(DocumentBatchRequest request, string correlationId, CancellationToken ct)
    {
        var folders = new DocumentFolders(
        [
            new DocumentFolder(request.ContainerName, "folder-1", ["document-1.pdf"]),
            new DocumentFolder(request.ContainerName, "folder-2", ["document-2.pdf"]),
        ]);

        return Task.FromResult(folders);
    }
}
