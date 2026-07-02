using DocumentProcessing.Contracts;

namespace DocumentProcessing.Functions.Models;

public record ProcessDocumentBatchInput(
    DocumentBatchRequest Request,
    string CorrelationId);
