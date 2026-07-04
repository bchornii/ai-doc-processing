namespace DocumentProcessing.Functions.Models;

public record ClassifyDocumentActivityInput(
    string ContainerName,
    string BlobName,
    string CorrelationId);
