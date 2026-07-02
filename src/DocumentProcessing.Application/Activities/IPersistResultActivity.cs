namespace DocumentProcessing.Application.Activities;

public interface IPersistResultActivity
{
    Task<bool> ExecuteAsync(string containerName, string blobName, byte[] content, string correlationId, CancellationToken ct);
}
