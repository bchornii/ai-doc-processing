namespace DocumentProcessing.Functions.Orchestrations.Processing;

public class NoOp : IProcessingStrategy
{
    public static IProcessingStrategy Instance { get; } = new NoOp();

    public Task RunAsync(
        ProcessingInput input,
        ProcessingContext processingContext,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
