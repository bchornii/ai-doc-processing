namespace DocumentProcessing.Core;

public record DocumentSegment(
    int SegmentIndex,
    int PageStart,
    int PageEnd,
    string DetectedType);
