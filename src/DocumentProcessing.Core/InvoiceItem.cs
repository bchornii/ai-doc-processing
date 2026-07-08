namespace DocumentProcessing.Core;

public record InvoiceItem(
    string ProductCode,
    string Description,
    decimal Quantity,
    MonetaryAmount UnitPrice,
    MonetaryAmount Total,
    MonetaryAmount? Tax = null);
