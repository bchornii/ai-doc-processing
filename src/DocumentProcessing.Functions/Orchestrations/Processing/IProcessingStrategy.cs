namespace DocumentProcessing.Functions.Orchestrations.Processing;

public interface IProcessingStrategy
{
    Task RunAsync(
        ProcessingInput input,
        ProcessingContext processingContext,
        CancellationToken cancellationToken = default);
}
