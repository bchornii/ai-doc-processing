using DocumentProcessing.Core;

namespace DocumentProcessing.Functions.Models;

public record DocumentSegmentInput(
    DocumentSegment Segment,
    string ContainerName,
    string FolderName,
    string BlobName,
    string CorrelationId);
