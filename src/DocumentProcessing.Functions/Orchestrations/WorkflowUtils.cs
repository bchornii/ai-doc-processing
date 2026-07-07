using DocumentProcessing.Core;
using DocumentProcessing.Functions.Orchestrations.Processing;
using Microsoft.DurableTask;

namespace DocumentProcessing.Functions.Orchestrations;

public static class WorkflowUtils
{
    private static readonly string[] TypePriority =
    [
        ClassificationTypes.Invoice,
        ClassificationTypes.Contract,
        ClassificationTypes.BoundedDocument,
        ClassificationTypes.General,
        ClassificationTypes.Email,
        ClassificationTypes.None,
    ];

    public static TaskOptions CreateActivityRetryPolicy() =>
        new(new RetryPolicy(
            maxNumberOfAttempts: 3,
            firstRetryInterval: TimeSpan.FromSeconds(5),
            backoffCoefficient: 2.0));

    public static IProcessingStrategy RetrieveDocumentProcessingStrategy(
        IReadOnlyList<PageClassification>? pageClassifications)
    {
        if (pageClassifications is null || pageClassifications.Count == 0)
        {
            return NoOp.Instance;
        }

        foreach (var type in TypePriority)
        {
            if (pageClassifications.Any(p => p.Classification == type))
            {
                return InstantiateStrategy(type);
            }
        }

        return NoOp.Instance;
    }

    public static IProcessingStrategy InstantiateStrategy(string documentType)
    {
        return documentType switch
        {
            ClassificationTypes.Invoice => new ProcessInvoice(),
            ClassificationTypes.Contract => new ProcessContract(),
            ClassificationTypes.BoundedDocument => new ProcessBoundedDocument(),
            ClassificationTypes.General or ClassificationTypes.Email => new ProcessGeneralDocument(),
            _ => NoOp.Instance,
        };
    }
}
