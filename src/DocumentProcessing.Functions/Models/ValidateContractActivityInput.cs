using DocumentProcessing.Core;

namespace DocumentProcessing.Functions.Models;

public record ValidateContractActivityInput(
    string ContractName,
    ContractData? Data,
    string CorrelationId);
