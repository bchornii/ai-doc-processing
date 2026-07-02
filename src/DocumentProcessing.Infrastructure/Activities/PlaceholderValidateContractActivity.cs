using DocumentProcessing.Application.Activities;
using DocumentProcessing.Core;

namespace DocumentProcessing.Infrastructure.Activities;

public class PlaceholderValidateContractActivity : IValidateContractActivity
{
    public Task<ValidationResult> ExecuteAsync(string contractName, ContractData data, string correlationId, CancellationToken ct)
    {
        return Task.FromResult(new ValidationResult(IsValid: true, Messages: []));
    }
}
