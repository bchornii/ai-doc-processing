# Data Model: Durable Functions Workflow Foundation

**Feature**: `001-durable-functions-workflow`
**Date**: 2026-06-30
**Layer**: `DocumentProcessing.Core` (all entities unless noted)

---

## Layer Assignments

| Type | Layer / Project |
|---|---|
| `DocumentBatchRequest` | `DocumentProcessing.Contracts` |
| `DocumentFolder` | `DocumentProcessing.Core` |
| `DocumentFolders` | `DocumentProcessing.Core` |
| `ConfidenceResult<T>` | `DocumentProcessing.Core` |
| `PageClassification` | `DocumentProcessing.Core` |
| `Classifications` | `DocumentProcessing.Core` |
| `Address` | `DocumentProcessing.Core` |
| `MonetaryAmount` | `DocumentProcessing.Core` |
| `InvoiceItem` | `DocumentProcessing.Core` |
| `Invoice` | `DocumentProcessing.Core` |
| `ValidationResult` | `DocumentProcessing.Core` |
| `ContractData` | `DocumentProcessing.Core` |
| `DocumentSegment` | `DocumentProcessing.Core` |
| `BoundaryDetectionResult` | `DocumentProcessing.Core` |
| `GeneralDocumentData` | `DocumentProcessing.Core` |
| `GeneralDocumentField` | `DocumentProcessing.Core` |
| `WorkflowResult` | `DocumentProcessing.Core` |
| `DocumentProcessingConstants` | `DocumentProcessing.Core` |
| `ClassificationTypes` | `DocumentProcessing.Core` |

---

## Entities

### `DocumentBatchRequest` — `DocumentProcessing.Contracts`

Inbound trigger payload for both HTTP and Queue triggers.

```csharp
namespace DocumentProcessing.Contracts;

public record DocumentBatchRequest
{
    [JsonPropertyName("container_name")]
    public string ContainerName { get; init; } = string.Empty;
}
```

**Validation**: `ContainerName` must be non-null and non-empty. Validated by HTTP trigger before orchestration starts. Queue trigger logs and dead-letters invalid messages.

---

### `DocumentFolder` — `DocumentProcessing.Core`

Represents a folder within the blob container that contains document files.

```csharp
public record DocumentFolder(
    string ContainerName,
    string Name,
    IReadOnlyList<string> DocumentFileNames
);
```

---

### `DocumentFolders` — `DocumentProcessing.Core`

Output of the `GetDocumentFolders` activity.

```csharp
public record DocumentFolders(
    IReadOnlyList<DocumentFolder> Folders
);
```

---

### `ConfidenceResult<T>` — `DocumentProcessing.Core`

Generic result wrapper carrying typed output alongside a confidence score derived from AI model signals.

```csharp
public record ConfidenceResult<T>(
    T Data,
    double OverallConfidence
);
```

**Constraints**: `OverallConfidence` is in range [0.0, 1.0].

---

### `PageClassification` — `DocumentProcessing.Core`

A single classification for a page range within a document.

```csharp
public record PageClassification(
    string Classification,
    int? ImageRangeStart,
    int? ImageRangeEnd
);
```

**Classification values**: see `ClassificationTypes` constants below.

---

### `Classifications` — `DocumentProcessing.Core`

Output of the `ClassifyDocument` activity. Contains page-level classification results.

```csharp
public record Classifications(
    IReadOnlyList<PageClassification> PageClassifications
);
```

---

### `ClassificationTypes` — `DocumentProcessing.Core`

Named string constants for document classification values.

```csharp
public static class ClassificationTypes
{
    public const string Invoice = "Invoice";
    public const string Contract = "Contract";
    public const string BoundedDocument = "BoundedDocument";
    public const string General = "General";
    public const string Email = "Email";
    public const string None = "None";
}
```

---

### `DocumentProcessingConstants` — `DocumentProcessing.Core`

Domain-level constants. The confidence threshold must be referenced symbolically throughout all orchestrators and never appear as an inline literal.

```csharp
public static class DocumentProcessingConstants
{
    /// <summary>
    /// Minimum confidence score required to proceed with type-specific document processing.
    /// Documents with overall confidence below this threshold are logged and skipped.
    /// </summary>
    public const double ConfidenceThreshold = 0.8;
}
```

---

### `Address` — `DocumentProcessing.Core`

Postal address used in invoice data.

```csharp
public record Address(
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country
);
```

---

### `MonetaryAmount` — `DocumentProcessing.Core`

A monetary value with currency denomination.

```csharp
public record MonetaryAmount(
    decimal Amount,
    string Currency
);
```

---

### `InvoiceItem` — `DocumentProcessing.Core`

A single line item on an invoice.

```csharp
public record InvoiceItem(
    string ProductCode,
    string Description,
    decimal Quantity,
    MonetaryAmount UnitPrice,
    MonetaryAmount Total,
    MonetaryAmount? Tax = null
);
```

---

### `Invoice` — `DocumentProcessing.Core`

Extracted invoice data. All dates are ISO 8601 strings (YYYY-MM-DD).

```csharp
public record Invoice(
    string InvoiceId,
    string CustomerName,
    string VendorName,
    string InvoiceDate,
    string DueDate,
    Address CustomerAddress,
    MonetaryAmount InvoiceTotal,
    IReadOnlyList<InvoiceItem> Items,
    string? PurchaseOrder = null,
    MonetaryAmount? Subtotal = null,
    MonetaryAmount? TotalTax = null,
    string? PaymentTerm = null
);
```

---

### `ValidationResult` — `DocumentProcessing.Core`

Output of invoice or contract validation steps.

```csharp
public record ValidationResult(
    bool IsValid,
    IReadOnlyList<string> Messages
);
```

**Notes**:
- For invoice validation: per-rule status flags (`HasInvoiceId`, `AllItemsValid`) may be added as optional properties or returned within `Messages`.
- For contract validation: similar per-clause flags.
- In the skeleton phase, the hardcoded `ValidationResult` has `IsValid = true` and an empty messages list.

---

### `ContractData` — `DocumentProcessing.Core`

Extracted contract data. Dates are ISO 8601 strings (YYYY-MM-DD).

```csharp
public record ContractData(
    IReadOnlyList<string> Parties,
    string EffectiveDate,
    IReadOnlyList<string> KeyObligations,
    string? ExpirationDate = null,
    string? RenewalTerms = null,
    string? ExitClause = null,
    string? GoverningLaw = null
);
```

---

### `DocumentSegment` — `DocumentProcessing.Core`

A detected sub-document within a bounded/continuous document.

```csharp
public record DocumentSegment(
    int SegmentIndex,
    int PageStart,
    int PageEnd,
    string DetectedType
);
```

**DetectedType values**: Same classification strings as `ClassificationTypes`.

---

### `BoundaryDetectionResult` — `DocumentProcessing.Core`

Output of the boundary detection activity for bounded/continuous documents.

```csharp
public record BoundaryDetectionResult(
    IReadOnlyList<DocumentSegment> Segments
);
```

---

### `GeneralDocumentField` — `DocumentProcessing.Core`

A single extracted name-value pair from a general document.

```csharp
public record GeneralDocumentField(
    string Name,
    string Value
);
```

---

### `GeneralDocumentData` — `DocumentProcessing.Core`

Structured data extracted from a general/schema-driven document.

```csharp
public record GeneralDocumentData(
    string SchemaName,
    IReadOnlyList<GeneralDocumentField> Fields
);
```

---

### `WorkflowResult` — `DocumentProcessing.Core`

Aggregated output of a per-folder sub-orchestration or the root batch orchestration.

```csharp
public record WorkflowResult(
    IReadOnlyList<string> Messages,
    IReadOnlyList<string> Errors
)
{
    public static WorkflowResult Empty =>
        new WorkflowResult([], []);
}
```

**Usage**:
- `Messages`: informational outcomes (e.g., "Processed invoice doc123.pdf successfully")
- `Errors`: failures and skips that did not prevent overall completion (e.g., "Skipped doc456.pdf: confidence 0.62 below threshold")

---

## Entity Relationships

```
DocumentBatchRequest
  └── ContainerName ──► GetDocumentFolders ──► DocumentFolders
                                                  └── folders[]: DocumentFolder
                                                        ├── ContainerName
                                                        ├── Name
                                                        └── DocumentFileNames[]
                                                              │
                                                              ▼ ClassifyDocument
                                                        ConfidenceResult<Classifications>
                                                              └── PageClassifications[]: PageClassification
                                                                    ├── Classification (Invoice|Contract|BoundedDocument|General|Email|None)
                                                                    ├── ImageRangeStart?
                                                                    └── ImageRangeEnd?
                                                                          │
                            ┌─────────────────────────────────────────────┤
                            │                                             │
                       Invoice path                             BoundedDocument path
                  ConfidenceResult<Invoice>              BoundaryDetectionResult
                       └── Invoice                             └── Segments[]: DocumentSegment
                             ├── InvoiceId                           ├── SegmentIndex
                             ├── CustomerName                        ├── PageStart
                             ├── VendorName                          ├── PageEnd
                             ├── InvoiceDate                         └── DetectedType
                             ├── DueDate
                             ├── CustomerAddress: Address
                             ├── InvoiceTotal: MonetaryAmount
                             └── Items[]: InvoiceItem
                                   ├── ProductCode
                                   ├── Description
                                   ├── Quantity
                                   ├── UnitPrice: MonetaryAmount
                                   ├── Total: MonetaryAmount
                                   └── Tax?: MonetaryAmount

WorkflowResult (aggregated per sub-orchestration and root)
  ├── Messages[]
  └── Errors[]
```

---

## Placeholder Implementations (Skeleton Phase)

| Activity | Hardcoded Return Value |
|---|---|
| `GetDocumentFolders` | `DocumentFolders` with two `DocumentFolder` entries, each with one document path |
| `ClassifyDocument` | `ConfidenceResult<Classifications>` with `OverallConfidence = 1.0`, one `PageClassification` with `Classification = "Invoice"` |
| `ExtractInvoice` | `ConfidenceResult<Invoice>` with `OverallConfidence = 1.0` and a hardcoded `Invoice` with all required fields populated |
| `ValidateInvoice` | `ValidationResult` with `IsValid = true` and empty `Messages` |
| `ExtractContract` | `ConfidenceResult<ContractData>` with `OverallConfidence = 1.0` and a hardcoded `ContractData` |
| `ValidateContract` | `ValidationResult` with `IsValid = true` and empty `Messages` |
| `DetectBoundaries` | `BoundaryDetectionResult` with two `DocumentSegment` entries covering distinct page ranges |
| `ExtractGeneralDocument` | `ConfidenceResult<GeneralDocumentData>` with `OverallConfidence = 1.0` and two sample `GeneralDocumentField` entries |
| `PersistResult` | Returns `true` without performing any I/O |
