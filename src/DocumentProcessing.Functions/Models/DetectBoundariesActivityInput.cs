namespace DocumentProcessing.Functions.Models;

public record DetectBoundariesActivityInput(
    string ContainerName,
    string BlobName,
    string CorrelationId);
