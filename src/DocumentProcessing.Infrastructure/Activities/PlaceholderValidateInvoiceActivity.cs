using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;

namespace DocumentProcessing.Infrastructure.Activities;

public class PlaceholderValidateInvoiceActivity : IValidateInvoiceActivity
{
    public Task<ValidationResult> ExecuteAsync(string invoiceName, Invoice data, string correlationId, CancellationToken ct)
    {
        return Task.FromResult(new ValidationResult(IsValid: true, Messages: [], HasInvoiceId: true, AllItemsValid: true));
    }
}
