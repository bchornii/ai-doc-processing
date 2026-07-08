namespace DocumentProcessing.Core;

public record GeneralDocumentData(
    string SchemaName,
    IReadOnlyList<GeneralDocumentField> Fields);
