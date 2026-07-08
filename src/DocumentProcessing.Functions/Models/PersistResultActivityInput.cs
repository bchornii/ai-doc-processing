namespace DocumentProcessing.Functions.Models;

#pragma warning disable CA1819 // Properties should not return arrays - DTO for activity input serialization
public record PersistResultActivityInput(
    string ContainerName,
    string BlobName,
    byte[] Content,
    string CorrelationId);
#pragma warning restore CA1819
