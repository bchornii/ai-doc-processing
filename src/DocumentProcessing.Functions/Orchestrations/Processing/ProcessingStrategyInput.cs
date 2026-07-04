using DocumentProcessing.Core;
using Microsoft.DurableTask;

namespace DocumentProcessing.Functions.Orchestrations.Processing;

public record struct ProcessingInput(
    TaskOrchestrationContext Context,
    DocumentFolder Folder,
    string DocumentFileName,
    string CorrelationId,
    TaskOptions RetryPolicy);
