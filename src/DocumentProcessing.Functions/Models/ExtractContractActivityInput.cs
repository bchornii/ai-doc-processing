namespace DocumentProcessing.Functions.Models;

public record ExtractContractActivityInput(
    string ContainerName,
    string BlobName,
    int? PageStart,
    int? PageEnd,
    string CorrelationId);
