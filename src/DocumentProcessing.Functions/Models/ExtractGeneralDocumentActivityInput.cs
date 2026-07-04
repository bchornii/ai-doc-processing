namespace DocumentProcessing.Functions.Models;

public record ExtractGeneralDocumentActivityInput(
    string ContainerName,
    string BlobName,
    int? PageStart,
    int? PageEnd,
    string CorrelationId);
