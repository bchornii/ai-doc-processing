namespace DocumentProcessing.Core;

public record PageClassification(
    string Classification,
    int? ImageRangeStart,
    int? ImageRangeEnd);
