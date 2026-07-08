namespace DocumentProcessing.Core;

public record Classifications(
    IReadOnlyList<PageClassification> PageClassifications);
