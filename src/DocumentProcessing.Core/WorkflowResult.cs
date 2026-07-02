namespace DocumentProcessing.Core;

public record WorkflowResult(
    IReadOnlyList<string> Messages,
    IReadOnlyList<string> Errors)
{
    public static WorkflowResult Empty =>
        new WorkflowResult([], []);
}
