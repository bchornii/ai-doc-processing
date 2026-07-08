namespace DocumentProcessing.Functions.Models;

public record ExtractInvoiceActivityInput(
    string ContainerName,
    string BlobName,
    int? PageStart,
    int? PageEnd,
    string CorrelationId);
