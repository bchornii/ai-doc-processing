namespace DocumentProcessing.Core;

public record ContractData(
    IReadOnlyList<string> Parties,
    string EffectiveDate,
    IReadOnlyList<string> KeyObligations,
    string? ExpirationDate = null,
    string? RenewalTerms = null,
    string? ExitClause = null,
    string? GoverningLaw = null);
