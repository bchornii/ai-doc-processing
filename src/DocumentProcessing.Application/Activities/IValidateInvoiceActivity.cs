using DocumentProcessing.Core;

namespace DocumentProcessing.Application.Activities;

public interface IValidateInvoiceActivity
{
    Task<ValidationResult> ExecuteAsync(string invoiceName, Invoice data, string correlationId, CancellationToken ct);
}
