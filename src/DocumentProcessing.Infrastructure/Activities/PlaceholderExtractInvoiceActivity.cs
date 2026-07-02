using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;

namespace DocumentProcessing.Infrastructure.Activities;

public class PlaceholderExtractInvoiceActivity : IExtractInvoiceActivity
{
    public Task<ConfidenceResult<Invoice>> ExecuteAsync(string containerName, string blobName, int? pageStart, int? pageEnd, string correlationId, CancellationToken ct)
    {
        var invoice = new Invoice(
            InvoiceId: "INV-001",
            CustomerName: "Contoso Ltd.",
            VendorName: "Fabrikam Inc.",
            InvoiceDate: "2026-01-01",
            DueDate: "2026-01-31",
            CustomerAddress: new Address("1 Microsoft Way", "Redmond", "WA", "98052", "US"),
            InvoiceTotal: new MonetaryAmount(100.00m, "USD"),
            Items:
            [
                new InvoiceItem(
                    ProductCode: "SKU-001",
                    Description: "Consulting Services",
                    Quantity: 1m,
                    UnitPrice: new MonetaryAmount(100.00m, "USD"),
                    Total: new MonetaryAmount(100.00m, "USD")),
            ]);

        return Task.FromResult(new ConfidenceResult<Invoice>(invoice, 1.0));
    }
}
