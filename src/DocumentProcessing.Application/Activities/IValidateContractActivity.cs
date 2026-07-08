using DocumentProcessing.Core;

namespace DocumentProcessing.Application.Activities;

public interface IValidateContractActivity
{
    Task<ValidationResult> ExecuteAsync(string contractName, ContractData data, string correlationId, CancellationToken ct);
}
