namespace DocumentProcessing.Core;

public record Invoice(
    string InvoiceId,
    string CustomerName,
    string VendorName,
    string InvoiceDate,
    string DueDate,
    Address CustomerAddress,
    MonetaryAmount InvoiceTotal,
    IReadOnlyList<InvoiceItem> Items,
    string? PurchaseOrder = null,
    MonetaryAmount? Subtotal = null,
    MonetaryAmount? TotalTax = null,
    string? PaymentTerm = null);
