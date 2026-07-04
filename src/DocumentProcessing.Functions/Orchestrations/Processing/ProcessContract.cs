using System.Text.Json;
using DocumentProcessing.Core;
using DocumentProcessing.Functions.Activities;
using DocumentProcessing.Functions.Models;

namespace DocumentProcessing.Functions.Orchestrations.Processing;

public class ProcessContract : IProcessingStrategy
{
    public async Task RunAsync(
        ProcessingInput input,
        ProcessingContext processingContext,
        CancellationToken cancellationToken = default)
    {
        var (context, folder, documentFileName, correlationId, retryPolicy) = input;

        var extractContractInput =
            new ExtractContractActivityInput(folder.ContainerName, documentFileName, null, null, correlationId);
        var contractResult = await context
            .CallActivityAsync<ConfidenceResult<ContractData>>(nameof(ExtractContractActivity), extractContractInput, retryPolicy);

        var persistExtractedContractInput = new PersistResultActivityInput(
            folder.ContainerName,
            documentFileName,
            JsonSerializer.SerializeToUtf8Bytes(contractResult),
            correlationId);
        await context.CallActivityAsync<bool>(nameof(PersistResultActivity), persistExtractedContractInput, retryPolicy);

        var validateContractInput =
            new ValidateContractActivityInput(documentFileName, contractResult?.Data, correlationId);
        var contractValidation = await context
            .CallActivityAsync<ValidationResult>(nameof(ValidateContractActivity), validateContractInput, retryPolicy);

        var persistContractValidationInput = new PersistResultActivityInput(
            folder.ContainerName,
            documentFileName,
            JsonSerializer.SerializeToUtf8Bytes(contractValidation),
            correlationId);
        await context.CallActivityAsync<bool>(nameof(PersistResultActivity), persistContractValidationInput, retryPolicy);

        processingContext.AddMessage($"Processed contract {documentFileName}");
    }
}
