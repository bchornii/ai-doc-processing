using System.Text.Json.Serialization;

namespace DocumentProcessing.Contracts;

public record DocumentBatchRequest
{
    [JsonPropertyName("container_name")]
    public string ContainerName { get; init; } = string.Empty;
}
