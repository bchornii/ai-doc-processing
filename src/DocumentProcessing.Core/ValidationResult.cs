namespace DocumentProcessing.Core;

public record ValidationResult(
    bool IsValid,
    IReadOnlyList<string> Messages,
    bool? HasInvoiceId = null,
    bool? AllItemsValid = null);
