namespace DocumentProcessing.Functions.Models;

public record ClassifyDocumentSegmentActivityInput(
    string ContainerName,
    string BlobName,
    int PageStart,
    int PageEnd,
    string CorrelationId);
