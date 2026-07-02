namespace DocumentProcessing.Core;

public record ConfidenceResult<T>(
    T Data,
    double OverallConfidence);
