namespace DocumentProcessing.Core;

/// <summary>
/// Domain-level constants for document processing pipeline.
/// </summary>
public static class DocumentProcessingConstants
{
    /// <summary>
    /// Minimum confidence score required to proceed with type-specific document processing.
    /// Documents with overall confidence below this threshold are logged and skipped.
    /// </summary>
    public const double ConfidenceThreshold = 0.8;
}
