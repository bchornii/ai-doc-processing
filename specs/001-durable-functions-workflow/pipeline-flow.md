# AI Document Processing Pipeline — Orchestration Flow

This document describes the complete orchestrator and activity execution flow, intended as a reference for implementing the pipeline in .NET.

## Triggers

The root orchestrator can be started via two triggers:

| Trigger | Details |
|---|---|
| **HTTP POST** | `POST /api/process-documents` with body `{ "container_name": "documents" }` |
| **Storage Queue** | Message on the `documents` queue with body `{ "container_name": "documents" }` (Base64-encoded) |

---

## Root Orchestrator: `ProcessDocumentBatchWorkflow`

```
[Trigger: HTTP or Queue]
        │
        ▼
1. Activity: GetDocumentFolders
   - Lists all root-level folders in the blob container
   - Filters for PDF files only (regex: .*\.(pdf)$)
   - Input:  { container_name }
   - Output: DocumentFolders { folders: [ DocumentFolder ] }
        │
        ▼
2. Fan-out (parallel) → Sub-Orchestrator: ProcessDocumentWorkflow
   - One sub-orchestration per folder
   - All folders are processed in parallel (Task.WhenAll equivalent)
   - Input:  DocumentFolder { container_name, name, document_file_names[] }
   - Output: WorkflowResult (aggregated messages + errors per folder)
        │
        ▼
3. Fan-in: aggregate all sub-orchestration results into root WorkflowResult
```

---

## Sub-Orchestrator: `ProcessDocumentWorkflow` (per folder, runs in parallel)

For **each PDF file** in the folder, the following sequence executes:

```
For each document file in folder:
│
├── 1. Activity: ClassifyDocument
│      Input:  { container_name, blob_name, classification_definitions }
│              classifications: "Invoice" | "Email" | "None"
│      Uses:   Azure OpenAI GPT-4o (vision/multimodal)
│      Output: ConfidenceResult<Classifications>
│                → page_classifications[]: { classification, image_range_start, image_range_end }
│                → overall_confidence (float, 0.0–1.0)
│
│      ── if classification failed or null → log error, continue to next file
│
├── 2. Activity: WriteBytesToBlob  (store classification result)
│      Input:  { storage_account, container_name, blob_name: "{doc}.Classification.json", content }
│      Output: bool (success)
│
│      ── if write failed → log error, continue to next file
│      ── if overall_confidence < 0.8 → log error, continue to next file
│      ── if no page_classifications → log message, continue to next file
│
│   For each page_classification in page_classifications:
│   │
│   ├── [If classification == "Invoice"]:
│   │   │
│   │   ├── 3. Activity: ExtractInvoice
│   │   │      Input:  { container_name, blob_name, page_range_start, page_range_end }
│   │   │      Uses:   Azure AI Document Intelligence (prebuilt-layout) for OCR text
│   │   │              + Azure OpenAI GPT-4o for structured extraction (multimodal)
│   │   │      Output: ConfidenceResult<Invoice>
│   │   │                → data: Invoice (structured fields)
│   │   │                → overall_confidence (float, 0.0–1.0)
│   │   │
│   │   │      ── if extraction failed or null → log error, continue to next page_classification
│   │   │
│   │   ├── 4. Activity: WriteBytesToBlob  (store extracted invoice data)
│   │   │      Blob name: "{doc}.{start}-{end}.Data.json"
│   │   │
│   │   │      ── if write failed → log error, continue to next page_classification
│   │   │      ── if overall_confidence < 0.8 → log error, continue to next page_classification
│   │   │
│   │   ├── 5. Activity: ValidateInvoice
│   │   │      Input:  { name, data: Invoice }
│   │   │      Logic:  Checks invoice_id present
│   │   │              Checks items[].product_code, quantity, total present
│   │   │      Output: ValidationResult { is_valid, messages[], status flags }
│   │   │
│   │   └── 6. Activity: WriteBytesToBlob  (store validation result)
│   │          Blob name: "{doc}.{start}-{end}.Validation.json"
│   │
│   └── [If classification != "Invoice"]:
│          Log message: "Skipping {classification} document", continue
```

---

## Activities Summary

| # | Activity | Azure Service | Input | Output |
|---|---|---|---|---|
| 1 | `GetDocumentFolders` | Azure Blob Storage | `DocumentBatchRequest` | `DocumentFolders` |
| 2 | `ClassifyDocument` | Azure OpenAI GPT-4o | container, blob, classification definitions | `ConfidenceResult<Classifications>` |
| 3 | `WriteBytesToBlob` | Azure Blob Storage | storage account, container, blob name, content bytes | `bool` |
| 4 | `ExtractInvoice` | Azure Document Intelligence + Azure OpenAI GPT-4o | container, blob, page range | `ConfidenceResult<Invoice>` |
| 5 | `ValidateInvoice` | None (pure logic) | invoice name, `Invoice` data | `WorkflowResult` |

> `WriteBytesToBlob` is called 3 times per invoice: after classification, after extraction, and after validation.

---

## Key Models

### `DocumentFolder`
```
container_name  string
name            string   (folder name in blob container)
document_file_names  string[]  (list of PDF blob paths)
```

### `ConfidenceResult<T>`
```
data                T        (the typed result, e.g. Invoice or Classifications)
overall_confidence  float    (0.0–1.0; derived from OpenAI logprobs + Document Intelligence scores)
```

### `Classifications`
```
page_classifications  PageClassification[]
  └── classification       string  ("Invoice" | "Email" | "None")
      image_range_start    int?
      image_range_end      int?
```

### `Invoice` (extracted fields)
```
invoice_id          string
customer_name       string
customer_address    Address
vendor_name         string
invoice_date        string (YYYY-MM-DD)
due_date            string (YYYY-MM-DD)
invoice_total       MonetaryAmount
items[]
  └── product_code  string
      description   string
      quantity      float
      unit_price    MonetaryAmount
      total         MonetaryAmount
      tax           MonetaryAmount?
... (additional optional fields: purchase_order, subtotal, total_tax, payment_term, etc.)
```

---

## Confidence Threshold

A threshold of **0.8** is applied after both classification and extraction:

- Below threshold → log error, skip further processing for that document/page range
- At or above threshold → continue to next step

Confidence is calculated by combining:
- **Azure OpenAI** `logprobs` on structured output tokens
- **Azure AI Document Intelligence** per-word confidence scores (for extraction)

---

## Output Blobs Written per Document

For a document `folder/invoice.pdf` classified as an Invoice spanning pages 1–3:

| Blob | Contents |
|---|---|
| `folder/invoice.pdf.Classification.json` | `ConfidenceResult<Classifications>` |
| `folder/invoice.pdf.1-3.Data.json` | `ConfidenceResult<Invoice>` |
| `folder/invoice.pdf.1-3.Validation.json` | `WorkflowResult` (validation outcome) |

---

## .NET Implementation Notes

| Python concept | .NET Durable Functions equivalent |
|---|---|
| `context.call_activity(name, input)` | `context.CallActivityAsync<T>("ActivityName", input)` |
| `context.call_sub_orchestrator(name, input)` | `context.CallSubOrchestratorAsync<T>("OrchestratorName", input)` |
| `context.task_all([...tasks])` | `Task.WhenAll(tasks)` |
| `yield` (generator-based) | `await` |
| Blueprints | Function classes with `[Function]` attribute |
| `DefaultAzureCredential` | `DefaultAzureCredential` from `Azure.Identity` |
