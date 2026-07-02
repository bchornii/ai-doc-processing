using DocumentProcessing.Core;

namespace DocumentProcessing.Functions.Models;

public record ProcessDocumentFolderInput(
    DocumentFolder Folder,
    string CorrelationId);
