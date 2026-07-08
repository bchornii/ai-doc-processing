using System.Text.Json;
using DocumentProcessing.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentProcessing.Functions.Extensions;

public static class FunctionsHttpExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = false };

    /// <summary>
    /// Get correlation id from <c>X-Correlation-Id</c> header of generate one if missing.
    /// </summary>
    /// <param name="req">HttpRequest to extract a header from.</param>
    /// <returns>Correlation id to be used for orchestrator identification.</returns>
    public static string GetOrCreateCorrelationId(this HttpRequest req) =>
        req.Headers.TryGetValue("X-Correlation-Id", out var headerValue)
        && !string.IsNullOrEmpty(headerValue)
            ? headerValue.ToString()
            : Guid.NewGuid().ToString();

    public static string CreateCorrelationId()
        => Guid.NewGuid().ToString();

    public static IResult InvalidStorageContainer() =>
        TypedResults.Problem(new ProblemDetails
        {
            Title = "Invalid Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = "The 'container_name' field is required and must not be empty.",
        });

    public static async Task<DocumentBatchRequest> TryReadDocumentBatchRequestAsync(this HttpRequest req)
    {
        try
        {
            return await req.ReadFromJsonAsync<DocumentBatchRequest>()
                      ?? DocumentBatchRequest.Null;
        }
        catch (JsonException)
        {
            return DocumentBatchRequest.Null;
        }
    }

    public static DocumentBatchRequest TryReadDocumentBatchRequest(this string message)
    {
        try
        {
            var request = JsonSerializer.Deserialize<DocumentBatchRequest>(message, JsonOptions);
            return request ?? DocumentBatchRequest.Null;
        }
        catch (JsonException)
        {
            return DocumentBatchRequest.Null;
        }
    }
}
