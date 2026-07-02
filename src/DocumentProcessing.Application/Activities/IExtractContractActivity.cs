using DocumentProcessing.Core;

namespace DocumentProcessing.Application.Activities;

public interface IExtractContractActivity
{
    Task<ConfidenceResult<ContractData>> ExecuteAsync(string containerName, string blobName, int? pageStart, int? pageEnd, string correlationId, CancellationToken ct);
}
