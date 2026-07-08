namespace DocumentProcessing.Core;

public record DocumentFolder(
    string ContainerName,
    string Name,
    IReadOnlyList<string> DocumentFileNames)
{
    public string BlobPath(string documentFileName) => $"{Name}/{documentFileName}";
}
