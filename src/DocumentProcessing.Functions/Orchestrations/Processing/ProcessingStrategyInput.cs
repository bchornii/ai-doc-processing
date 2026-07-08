using DocumentProcessing.Core;
using Microsoft.DurableTask;

namespace DocumentProcessing.Functions.Orchestrations.Processing;

public record struct ProcessingInput(
    TaskOrchestrationContext Context,
    DocumentFolder Folder,
    string BlobName,
    string CorrelationId,
    TaskOptions RetryPolicy);
