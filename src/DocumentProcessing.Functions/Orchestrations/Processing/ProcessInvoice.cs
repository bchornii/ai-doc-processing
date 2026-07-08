using System.Text.Json;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Activities;
using DocumentProcessing.Functions.Models;

namespace DocumentProcessing.Functions.Orchestrations.Processing;

/// <summary>
/// Processes invoice document: extract data, persists data, validates data, persists validation result.
/// </summary>
public class ProcessInvoice : IProcessingStrategy
{
    public async Task RunAsync(
        ProcessingInput input,
        ProcessingContext processingContext,
        CancellationToken cancellationToken = default)
    {
        var (context, folder, documentFileName, correlationId, retryPolicy) = input;

        var extractInvoiceInput =
            new ExtractInvoiceActivityInput(folder.ContainerName, documentFileName, null, null, correlationId);
        var invoiceConfidenceResult = await context
            .CallActivityAsync<ConfidenceResult<Invoice>>(nameof(ExtractInvoiceActivity), extractInvoiceInput, retryPolicy);

        var persistInvoiceInput = new PersistResultActivityInput(
            folder.ContainerName,
            documentFileName,
            JsonSerializer.SerializeToUtf8Bytes(invoiceConfidenceResult),
            correlationId);
        await context.CallActivityAsync<bool>(nameof(PersistResultActivity), persistInvoiceInput, retryPolicy);

        var validateInvoiceInput =
            new ValidateInvoiceActivityInput(documentFileName, invoiceConfidenceResult?.Data, correlationId);
        var invoiceValidation = await context
            .CallActivityAsync<ValidationResult>(nameof(ValidateInvoiceActivity), validateInvoiceInput, retryPolicy);

        var persistInvoiceValidationInput = new PersistResultActivityInput(
            folder.ContainerName,
            documentFileName,
            JsonSerializer.SerializeToUtf8Bytes(invoiceValidation),
            correlationId);
        await context.CallActivityAsync<bool>(nameof(PersistResultActivity), persistInvoiceValidationInput, retryPolicy);

        processingContext.AddMessage($"Processed invoice {documentFileName}");
    }
}
