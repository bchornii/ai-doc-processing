using DocumentProcessing.Application.Activities;

namespace DocumentProcessing.Infrastructure.Activities;

public class PlaceholderPersistResultActivity : IPersistResultActivity
{
    public Task<bool> ExecuteAsync(string containerName, string blobName, byte[] content, string correlationId, CancellationToken ct)
    {
        return Task.FromResult(true);
    }
}
