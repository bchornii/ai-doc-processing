namespace DocumentProcessing.Core;

public record BoundaryDetectionResult(
    IReadOnlyList<DocumentSegment> Segments);
