using System.Text.Json;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Activities;
using DocumentProcessing.Functions.Models;

namespace DocumentProcessing.Functions.Orchestrations.Processing;

public class ProcessGeneralDocument : IProcessingStrategy
{
    public async Task RunAsync(
        ProcessingInput input,
        ProcessingContext processingContext,
        CancellationToken cancellationToken = default)
    {
        var (context, folder, documentFileName, correlationId, retryPolicy) = input;

        var extractGeneralDocumentInput =
            new ExtractGeneralDocumentActivityInput(folder.ContainerName, documentFileName, null, null, correlationId);
        var generalResult = await context
            .CallActivityAsync<ConfidenceResult<GeneralDocumentData>>(nameof(ExtractGeneralDocumentActivity), extractGeneralDocumentInput, retryPolicy);

        var persistResultInput = new PersistResultActivityInput(
            folder.ContainerName,
            documentFileName,
            JsonSerializer.SerializeToUtf8Bytes(generalResult),
            correlationId);

        await context.CallActivityAsync<bool>(nameof(PersistResultActivity), persistResultInput, retryPolicy);

        processingContext.AddMessage($"Processed general document {documentFileName}");
    }
}
