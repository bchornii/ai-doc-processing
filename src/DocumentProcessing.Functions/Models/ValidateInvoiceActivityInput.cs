using DocumentProcessing.Core;

namespace DocumentProcessing.Functions.Models;

public record ValidateInvoiceActivityInput(
    string InvoiceName,
    Invoice? Data,
    string CorrelationId);
