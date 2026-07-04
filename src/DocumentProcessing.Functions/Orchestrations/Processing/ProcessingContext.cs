namespace DocumentProcessing.Functions.Orchestrations.Processing;

public record ProcessingContext
{
    private readonly List<string> _messages = [];
    private readonly List<string> _errors = [];

    public IReadOnlyList<string> Messages => _messages.AsReadOnly();

    public IReadOnlyList<string> Errors => _errors.AsReadOnly();

    public void AddMessage(string message)
        => _messages.Add(message);

    public void AddMessages(IEnumerable<string> messages)
        => _messages.AddRange(messages);

    public void AddError(string error)
        => _errors.Add(error);

    public void AddErrors(IEnumerable<string> errors)
        => _errors.AddRange(errors);
}
