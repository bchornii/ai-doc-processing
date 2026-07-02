using System.Text.Json.Serialization;

namespace DocumentProcessing.Contracts;

public record DocumentBatchRequest
{
    public static DocumentBatchRequest Null { get; } = new();

    [JsonPropertyName("container_name")]
    public string ContainerName { get; init; } = string.Empty;

    public bool InvalidContainerName
        => string.IsNullOrWhiteSpace(ContainerName);
}
