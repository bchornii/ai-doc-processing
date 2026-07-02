using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;

namespace DocumentProcessing.Infrastructure.Activities;

public class PlaceholderExtractContractActivity : IExtractContractActivity
{
    public Task<ConfidenceResult<ContractData>> ExecuteAsync(string containerName, string blobName, int? pageStart, int? pageEnd, string correlationId, CancellationToken ct)
    {
        var contract = new ContractData(
            Parties: ["Party A", "Party B"],
            EffectiveDate: "2026-01-01",
            KeyObligations: ["Deliver services", "Pay invoices on time"]);

        return Task.FromResult(new ConfidenceResult<ContractData>(contract, 1.0));
    }
}
