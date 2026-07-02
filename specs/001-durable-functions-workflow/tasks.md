---

description: "Task list for Durable Functions Workflow Foundation"
---

# Tasks: Durable Functions Workflow Foundation

**Feature**: `001-durable-functions-workflow`
**Generated**: 2026-07-01
**Input**: spec.md (P1–P5, FR-004 first-match routing), plan.md, data-model.md, research.md, contracts/http-api.md

---

## Phase 1: Setup

**Goal**: Bootstrap all project files, package references, host configuration, and local development settings. All work in this phase must compile before Phase 2 begins.

**Independent Test**: `dotnet build` succeeds across all five source projects, the unit test project, and the integration test project with no errors.

- [X] T001 Create src/DocumentProcessing.Functions/DocumentProcessing.Functions.csproj — isolated worker Functions project targeting net10.0 with OutputType=Exe; add PackageReference entries for Microsoft.Azure.Functions.Worker (2.*), Microsoft.Azure.Functions.Worker.Sdk (2.*), Microsoft.Azure.Functions.Worker.Extensions.Http (3.*), Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore (1.*), Microsoft.Azure.Functions.Worker.Extensions.DurableTask (1.*), Microsoft.Azure.Functions.Worker.Extensions.DurableTask.AzureManaged, Microsoft.Azure.Functions.Worker.Extensions.Storage.Queues (6.*), Microsoft.Azure.Functions.Worker.ApplicationInsights, Microsoft.DurableTask.Generators, Azure.Identity (1.*); add ProjectReference to Application, Infrastructure, Core, and Contracts projects
- [X] T002 [P] Create src/DocumentProcessing.Contracts/DocumentProcessing.Contracts.csproj — class library targeting net10.0 with no project dependencies and no extra NuGet packages (uses only BCL System.Text.Json)
- [X] T003 [P] Update Directory.Packages.props with all required NuGet package versions: Microsoft.Azure.Functions.Worker 2.x, Microsoft.Azure.Functions.Worker.Sdk 2.x, Microsoft.Azure.Functions.Worker.Extensions.Http 3.x, Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore 1.x, Microsoft.Azure.Functions.Worker.Extensions.DurableTask 1.x, Microsoft.Azure.Functions.Worker.Extensions.DurableTask.AzureManaged latest, Microsoft.Azure.Functions.Worker.Extensions.Storage.Queues 6.x, Microsoft.Azure.Functions.Worker.ApplicationInsights latest, Microsoft.DurableTask.Generators latest, Azure.Identity 1.x, xunit latest, xunit.runner.visualstudio latest, Microsoft.NET.Test.Sdk latest, NSubstitute latest, coverlet.collector latest
- [X] T004 [P] Update src/DocumentProcessing.Application/DocumentProcessing.Application.csproj — add ProjectReference entries for DocumentProcessing.Core and DocumentProcessing.Contracts; confirm TargetFramework=net10.0
- [X] T005 [P] Update src/DocumentProcessing.Infrastructure/DocumentProcessing.Infrastructure.csproj — add ProjectReference entries for DocumentProcessing.Application and DocumentProcessing.Core; confirm TargetFramework=net10.0
- [X] T006 [P] Create tests/DocumentProcessing.UnitTests/DocumentProcessing.UnitTests.csproj — xUnit test project targeting net10.0; add PackageReference for xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, NSubstitute, coverlet.collector; add ProjectReference entries for all five source projects (Functions, Application, Infrastructure, Core, Contracts)
- [X] T067 [P] Create tests/DocumentProcessing.IntegrationTests/DocumentProcessing.IntegrationTests.csproj — xUnit test project targeting net10.0 with IsPackable=false; add PackageReference for xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, Microsoft.Azure.Functions.Worker.TestServer (latest) for in-process host testing; add ProjectReference entries for all five source projects; this project hosts the local smoke test (T065) that requires DTS emulator and Azurite to be running
- [X] T007 [P] Add DocumentProcessing.Functions, DocumentProcessing.Contracts, DocumentProcessing.UnitTests, and DocumentProcessing.IntegrationTests projects to AiDocumentProcessing.slnx
- [X] T008 Create src/DocumentProcessing.Functions/Program.cs — configure isolated worker host with AddAzureFunctionsWorker(); configure DTS backend via UseDurableTaskScheduler() reading DTS_CONNECTION_STRING and TASKHUB_NAME from environment; add AddApplicationInsightsTelemetryWorkerService(); leave placeholder comments for DI activity registrations (to be filled in Phase 2 T036)
- [X] T009 [P] Create src/DocumentProcessing.Functions/host.json — set extensionBundle v4, set logging:logLevel:Default to Information; note: Durable Functions activity retry policies are configured per-call in orchestrator code via TaskOptions (see T040, T048, T050), not in host.json
- [X] T010 [P] Create src/DocumentProcessing.Functions/local.settings.template.json with keys: FUNCTIONS_WORKER_RUNTIME=dotnet-isolated, AzureWebJobsStorage=UseDevelopmentStorage=true, DTS_CONNECTION_STRING=Endpoint=http://localhost:8080;Authentication=None, TASKHUB_NAME=default; add the line `src/DocumentProcessing.Functions/local.settings.json` to root .gitignore (create the .gitignore entry; do not merely verify it)

---

## Phase 2: Foundational

**Goal**: All domain entities, orchestration input models, activity port interfaces, and placeholder implementations exist and the DI container wires them together. The application can start and placeholder activities return hardcoded typed results.

**Independent Test**: `dotnet build` succeeds; placeholder activity classes can be instantiated and return hardcoded results when called directly (no host required).

### Core Domain Entities (`src/DocumentProcessing.Core/`)

- [X] T011 [P] Create src/DocumentProcessing.Core/DocumentProcessingConstants.cs — public static class with `public const double ConfidenceThreshold = 0.8` and XML doc comment per FR-006
- [X] T012 [P] Create src/DocumentProcessing.Core/ClassificationTypes.cs — public static class with const string fields: Invoice, Contract, BoundedDocument, General, Email, None per data-model.md
- [X] T013 [P] Create src/DocumentProcessing.Core/DocumentFolder.cs (record: ContainerName, Name, IReadOnlyList<string> DocumentFileNames) and src/DocumentProcessing.Core/DocumentFolders.cs (record: IReadOnlyList<DocumentFolder> Folders) per data-model.md
- [X] T014 [P] Create src/DocumentProcessing.Core/ConfidenceResult.cs — generic record ConfidenceResult<T>(T Data, double OverallConfidence) per data-model.md
- [X] T015 [P] Create src/DocumentProcessing.Core/PageClassification.cs (record: string Classification, int? ImageRangeStart, int? ImageRangeEnd) and src/DocumentProcessing.Core/Classifications.cs (record: IReadOnlyList<PageClassification> PageClassifications) per data-model.md
- [X] T016 [P] Create src/DocumentProcessing.Core/Address.cs (record: Street, City, State, PostalCode, Country), src/DocumentProcessing.Core/MonetaryAmount.cs (record: decimal Amount, string Currency), src/DocumentProcessing.Core/InvoiceItem.cs (record: ProductCode, Description, decimal Quantity, MonetaryAmount UnitPrice, MonetaryAmount Total, MonetaryAmount? Tax), and src/DocumentProcessing.Core/Invoice.cs (record with required and optional fields) per data-model.md
- [X] T017 [P] Create src/DocumentProcessing.Core/ValidationResult.cs (record: bool IsValid, IReadOnlyList<string> Messages, bool? HasInvoiceId, bool? AllItemsValid — nullable per-rule status flags; null means the rule was not evaluated by this validator) and src/DocumentProcessing.Core/ContractData.cs (record: IReadOnlyList<string> Parties, string EffectiveDate, IReadOnlyList<string> KeyObligations, optional ExpirationDate/RenewalTerms/ExitClause/GoverningLaw) per data-model.md
- [X] T018 [P] Create src/DocumentProcessing.Core/DocumentSegment.cs (record: int SegmentIndex, int PageStart, int PageEnd, string DetectedType), src/DocumentProcessing.Core/BoundaryDetectionResult.cs (record: IReadOnlyList<DocumentSegment> Segments), src/DocumentProcessing.Core/GeneralDocumentField.cs (record: string Name, string Value), and src/DocumentProcessing.Core/GeneralDocumentData.cs (record: string SchemaName, IReadOnlyList<GeneralDocumentField> Fields) per data-model.md
- [X] T019 [P] Create src/DocumentProcessing.Core/WorkflowResult.cs — record(IReadOnlyList<string> Messages, IReadOnlyList<string> Errors) with static WorkflowResult Empty property returning new instance with empty lists per data-model.md

### Contracts

- [X] T020 [P] Create src/DocumentProcessing.Contracts/DocumentBatchRequest.cs — public record with `[JsonPropertyName("container_name")] public string ContainerName` init property per data-model.md

### Orchestration Input Models (`src/DocumentProcessing.Functions/Models/`)

- [X] T021 [P] Create src/DocumentProcessing.Functions/Models/ProcessDocumentBatchInput.cs — record with DocumentBatchRequest Request and string CorrelationId; this is the TInput for ProcessDocumentBatchWorkflow
- [X] T022 [P] Create src/DocumentProcessing.Functions/Models/ProcessDocumentFolderInput.cs — record with DocumentFolder Folder and string CorrelationId; this is the TInput for ProcessDocumentWorkflow
- [X] T023 [P] Create src/DocumentProcessing.Functions/Models/DocumentSegmentInput.cs — record with DocumentSegment Segment, string ContainerName, string FolderName, string BlobName, and string CorrelationId; BlobName is required by ProcessDocumentSegmentWorkflow to pass the source document path to ClassifyDocument and Extract activity calls; this is the TInput for ProcessDocumentSegmentWorkflow

### Activity Port Interfaces (`src/DocumentProcessing.Application/Activities/`)

- [X] T024 [P] Create src/DocumentProcessing.Application/Activities/IGetDocumentFoldersActivity.cs — interface with `Task<DocumentFolders> ExecuteAsync(DocumentBatchRequest request, string correlationId, CancellationToken ct)` and src/DocumentProcessing.Application/Activities/IClassifyDocumentActivity.cs — interface with `Task<ConfidenceResult<Classifications>> ExecuteAsync(string containerName, string blobName, string correlationId, CancellationToken ct)`
- [X] T025 [P] Create src/DocumentProcessing.Application/Activities/IPersistResultActivity.cs — interface with `Task<bool> ExecuteAsync(string containerName, string blobName, byte[] content, string correlationId, CancellationToken ct)`
- [X] T026 [P] Create src/DocumentProcessing.Application/Activities/IExtractInvoiceActivity.cs — interface with `Task<ConfidenceResult<Invoice>> ExecuteAsync(string containerName, string blobName, int? pageStart, int? pageEnd, string correlationId, CancellationToken ct)` and src/DocumentProcessing.Application/Activities/IValidateInvoiceActivity.cs — interface with `Task<ValidationResult> ExecuteAsync(string invoiceName, Invoice data, string correlationId, CancellationToken ct)`
- [X] T027 [P] Create src/DocumentProcessing.Application/Activities/IExtractContractActivity.cs — interface with `Task<ConfidenceResult<ContractData>> ExecuteAsync(string containerName, string blobName, int? pageStart, int? pageEnd, string correlationId, CancellationToken ct)` and src/DocumentProcessing.Application/Activities/IValidateContractActivity.cs — interface with `Task<ValidationResult> ExecuteAsync(string contractName, ContractData data, string correlationId, CancellationToken ct)`
- [X] T028 [P] Create src/DocumentProcessing.Application/Activities/IDetectBoundariesActivity.cs — interface with `Task<BoundaryDetectionResult> ExecuteAsync(string containerName, string blobName, string correlationId, CancellationToken ct)` and src/DocumentProcessing.Application/Activities/IExtractGeneralDocumentActivity.cs — interface with `Task<ConfidenceResult<GeneralDocumentData>> ExecuteAsync(string containerName, string blobName, int? pageStart, int? pageEnd, string correlationId, CancellationToken ct)`

### Placeholder Implementations (`src/DocumentProcessing.Infrastructure/Activities/`)

- [X] T029 [P] Create src/DocumentProcessing.Infrastructure/Activities/PlaceholderGetDocumentFoldersActivity.cs — implements IGetDocumentFoldersActivity; returns DocumentFolders with two hardcoded DocumentFolder entries each containing one document path per FR-005
- [X] T030 [P] Create src/DocumentProcessing.Infrastructure/Activities/PlaceholderClassifyDocumentActivity.cs — implements IClassifyDocumentActivity; returns ConfidenceResult<Classifications>(new Classifications([new PageClassification(ClassificationTypes.Invoice, 1, 1)]), 1.0) per FR-005
- [X] T031 [P] Create src/DocumentProcessing.Infrastructure/Activities/PlaceholderPersistResultActivity.cs — implements IPersistResultActivity; returns true without performing any I/O per FR-005
- [X] T032 [P] Create src/DocumentProcessing.Infrastructure/Activities/PlaceholderExtractInvoiceActivity.cs — implements IExtractInvoiceActivity; returns hardcoded ConfidenceResult<Invoice> with OverallConfidence=1.0 and all required Invoice fields populated per FR-005; and src/DocumentProcessing.Infrastructure/Activities/PlaceholderValidateInvoiceActivity.cs — implements IValidateInvoiceActivity; returns ValidationResult(IsValid=true, Messages=[], HasInvoiceId=true, AllItemsValid=true) per FR-005 (nullable per-rule flags are set to true for a fully valid hardcoded result)
- [X] T033 [P] Create src/DocumentProcessing.Infrastructure/Activities/PlaceholderExtractContractActivity.cs — implements IExtractContractActivity; returns hardcoded ConfidenceResult<ContractData> with OverallConfidence=1.0 per FR-005; and src/DocumentProcessing.Infrastructure/Activities/PlaceholderValidateContractActivity.cs — implements IValidateContractActivity; returns ValidationResult(IsValid=true, []) per FR-005
- [X] T034 [P] Create src/DocumentProcessing.Infrastructure/Activities/PlaceholderDetectBoundariesActivity.cs — implements IDetectBoundariesActivity; returns BoundaryDetectionResult with two DocumentSegment entries covering distinct non-overlapping page ranges (e.g. pages 1–3 and 4–6) per FR-005
- [X] T035 [P] Create src/DocumentProcessing.Infrastructure/Activities/PlaceholderExtractGeneralDocumentActivity.cs — implements IExtractGeneralDocumentActivity; returns hardcoded ConfidenceResult<GeneralDocumentData> with OverallConfidence=1.0 and two GeneralDocumentField sample entries per FR-005

### DI Registration

- [X] T036 Register all nine IXxxActivity → PlaceholderXxxActivity mappings in src/DocumentProcessing.Functions/Program.cs using services.AddScoped<IGetDocumentFoldersActivity, PlaceholderGetDocumentFoldersActivity>() pattern for all activity interfaces defined in T024–T028

---

## Phase 3: User Story 1 — HTTP Trigger (P1)

**Story Goal**: `POST /api/process-documents` returns HTTP 202 immediately with a Durable Functions management URL payload; invalid requests return HTTP 400 (RFC 9457 Problem Details); no orchestration is started for invalid requests.

**Independent Test**: Post `{ "container_name": "documents" }` to the local endpoint, receive HTTP 202, poll `statusQueryGetUri` to `Completed`, verify output is a non-null WorkflowResult.

- [ ] T037 [US1] Create src/DocumentProcessing.Functions/Orchestrations/ProcessDocumentBatchWorkflow.cs — class-based orchestrator decorated with [DurableTask], inheriting TaskOrchestrator<ProcessDocumentBatchInput, WorkflowResult>; stub body returns WorkflowResult.Empty; use context.CreateReplaySafeLogger() for all logging; this enables HTTP trigger compilation and initial status polling
- [ ] T038 [US1] Implement src/DocumentProcessing.Functions/Triggers/ProcessDocumentsHttpTrigger.cs — HTTP trigger on route "process-documents" accepting POST; deserialize request body to DocumentBatchRequest; return HTTP 400 with Content-Type: application/problem+json and ProblemDetails body (title, status=400, detail) if ContainerName is null or empty and do not start orchestration (FR-014); extract X-Correlation-Id header or generate new Guid.NewGuid().ToString() if absent (FR-009); start ProcessDocumentBatchWorkflow via DurableTaskClient.ScheduleNewOrchestrationInstanceAsync with new ProcessDocumentBatchInput(request, correlationId); return HTTP 202 via DurableTaskClient.CreateCheckStatusResponseAsync (FR-001); log orchestration instanceId and correlationId as structured properties (FR-008)

---

## Phase 4: User Story 2 — Root Batch Orchestration Fan-Out/Fan-In (P2)

**Story Goal**: The root orchestration calls GetDocumentFolders, fans out one ProcessDocumentWorkflow sub-orchestration per folder, fans in via Task.WhenAll, and aggregates results into a single WorkflowResult.

**Independent Test**: Start an orchestration; verify root output WorkflowResult contains one entry per folder returned by the placeholder folder discovery step.

- [ ] T039 [P] [US2] Create src/DocumentProcessing.Functions/Activities/GetDocumentFoldersActivity.cs — class-based activity with [DurableTask], inheriting TaskActivity<ProcessDocumentBatchInput, DocumentFolders>; inject IGetDocumentFoldersActivity via constructor; call service.ExecuteAsync(input.Request, input.CorrelationId); log activity start and complete with instanceId and correlationId as structured properties (FR-008)
- [ ] T040 [US2] Replace ProcessDocumentBatchWorkflow stub body in src/DocumentProcessing.Functions/Orchestrations/ProcessDocumentBatchWorkflow.cs with fan-out/fan-in logic: (1) call GetDocumentFoldersActivity via context.CallActivityAsync with TaskOptions(new RetryPolicy(maxNumberOfAttempts: 3, firstRetryInterval: TimeSpan.FromSeconds(5), backoffCoefficient: 2.0)); (2) if Folders is empty return WorkflowResult.Empty immediately; (3) fan-out: build list of CallSubOrchestrationAsync<WorkflowResult>(nameof(ProcessDocumentWorkflow), new ProcessDocumentFolderInput(folder, input.CorrelationId)) for each folder; (4) fan-in: await Task.WhenAll; (5) aggregate all WorkflowResult messages and errors into a single root WorkflowResult; use context.CreateReplaySafeLogger() for all log calls (FR-007)

---

## Phase 5: User Story 3 — Per-Folder Type Dispatch (P3)

**Story Goal**: Each per-folder sub-orchestration iterates documents, classifies each, persists the classification, then routes to the correct processing chain (Invoice, Contract, BoundedDocument, General, Email/None) based on classification and confidence threshold. All chains are placeholder implementations.

**Independent Test**: Inspect execution history of a completed per-folder sub-orchestration; verify the correct activity sequence appears for each document type (Invoice, Contract, General).

- [ ] T041 [P] [US3] Create src/DocumentProcessing.Functions/Activities/ClassifyDocumentActivity.cs — [DurableTask] TaskActivity<ProcessDocumentFolderInput, ConfidenceResult<Classifications>>; inject IClassifyDocumentActivity; pass input.Folder.ContainerName, blobName, input.CorrelationId to service; log start/complete with structured correlationId and instanceId
- [ ] T042 [P] [US3] Create src/DocumentProcessing.Functions/Activities/PersistResultActivity.cs — [DurableTask] TaskActivity accepting a composite input record (containerName, blobName, content as byte[], correlationId); inject IPersistResultActivity; orchestration caller must serialize the result to UTF-8 byte[] before passing; log start/complete with structured properties
- [ ] T043 [P] [US3] Create src/DocumentProcessing.Functions/Activities/ExtractInvoiceActivity.cs — [DurableTask] TaskActivity with appropriate composite input; inject IExtractInvoiceActivity; log structured properties
- [ ] T044 [P] [US3] Create src/DocumentProcessing.Functions/Activities/ValidateInvoiceActivity.cs — [DurableTask] TaskActivity with composite input (invoiceName, Invoice data, correlationId); inject IValidateInvoiceActivity; log structured properties
- [ ] T045 [P] [US3] Create src/DocumentProcessing.Functions/Activities/ExtractContractActivity.cs — [DurableTask] TaskActivity with composite input; inject IExtractContractActivity; log structured properties; and src/DocumentProcessing.Functions/Activities/ValidateContractActivity.cs — [DurableTask] TaskActivity; inject IValidateContractActivity; log structured properties
- [ ] T046 [P] [US3] Create src/DocumentProcessing.Functions/Activities/DetectBoundariesActivity.cs — [DurableTask] TaskActivity with composite input (containerName, blobName, correlationId); inject IDetectBoundariesActivity; log structured properties; and src/DocumentProcessing.Functions/Activities/ExtractGeneralDocumentActivity.cs — [DurableTask] TaskActivity with composite input; inject IExtractGeneralDocumentActivity; log structured properties
- [ ] T047 [US3] Create src/DocumentProcessing.Functions/Orchestrations/ProcessDocumentSegmentWorkflow.cs — [DurableTask] TaskOrchestrator<DocumentSegmentInput, WorkflowResult> stub returning WorkflowResult.Empty; enables ProcessDocumentWorkflow BoundedDocument branch to compile; full implementation in Phase 7 (T050)
- [ ] T048 [US3] Implement src/DocumentProcessing.Functions/Orchestrations/ProcessDocumentWorkflow.cs — [DurableTask] TaskOrchestrator<ProcessDocumentFolderInput, WorkflowResult>; for each documentFileName in input.Folder.DocumentFileNames: (1) ClassifyDocument → PersistResult (classification JSON as UTF-8 byte[]); (2) if result is null: log error, add error to WorkflowResult, continue; (3) if OverallConfidence < DocumentProcessingConstants.ConfidenceThreshold: log skip, add error entry, continue (FR-004); (4) route by first matching PageClassification in result.Data.PageClassifications — check in priority order Invoice, Contract, BoundedDocument, General, Email, None — and execute only the chain for the first match (first-match priority-order semantics per FR-004); use TaskOptions(new RetryPolicy(maxNumberOfAttempts: 3, firstRetryInterval: TimeSpan.FromSeconds(5), backoffCoefficient: 2.0)) on every CallActivityAsync call: Invoice→ExtractInvoice+PersistResult+ValidateInvoice+PersistResult; Contract→ExtractContract+PersistResult+ValidateContract+PersistResult; BoundedDocument→DetectBoundaries; if Segments.Count==0 add error and continue; else fan-out CallSubOrchestrationAsync(ProcessDocumentSegmentWorkflow, new DocumentSegmentInput(segment, folder.ContainerName, folder.Name, blobName, correlationId)) per segment+Task.WhenAll fan-in+PersistResult; General→ExtractGeneralDocument+PersistResult; Email/None→log and continue without error; aggregate and return WorkflowResult; use context.CreateReplaySafeLogger() throughout; MUST NOT read DateTime.UtcNow, Guid.NewGuid, or any I/O inline (FR-007)

---

## Phase 6: User Story 4 — Queue Trigger (P4)

**Story Goal**: A Base64-encoded `{ "container_name": "documents" }` message on the `documents` queue starts the same ProcessDocumentBatchWorkflow as the HTTP trigger.

**Independent Test**: Publish a valid Base64-encoded message to the local `documents` queue; verify a new root orchestration starts and reaches Completed status.

- [ ] T049 [US4] Implement src/DocumentProcessing.Functions/Triggers/ProcessDocumentsQueueTrigger.cs — Queue trigger on queue-name "documents"; receive message as string; Base64-decode and deserialize to DocumentBatchRequest; if decode/deserialize fails or ContainerName is empty, log error with correlationId and rethrow to trigger dead-lettering after retry exhaustion; generate correlationId via Guid.NewGuid().ToString(); start ProcessDocumentBatchWorkflow via DurableTaskClient.ScheduleNewOrchestrationInstanceAsync with ProcessDocumentBatchInput; log instanceId and correlationId as structured properties (FR-001a, FR-008)

---

## Phase 7: User Story 5 — BoundedDocument Boundary Detection and Per-Segment Processing (P5)

**Story Goal**: When a document is classified as BoundedDocument, boundary detection returns two hardcoded segments; each segment runs its own classification → extraction → persistence chain in a nested parallel fan-out; nested BoundedDocument classification falls back to the General path.

**Independent Test**: Start an orchestration where the placeholder classifier returns BoundedDocument; verify DetectBoundaries is called, two ProcessDocumentSegmentWorkflow sub-orchestrations start and complete, and results are aggregated.

- [ ] T050 [US5] Replace ProcessDocumentSegmentWorkflow stub body in src/DocumentProcessing.Functions/Orchestrations/ProcessDocumentSegmentWorkflow.cs with full implementation: (1) classify segment using input.BlobName and input.Segment page range (ImageRangeStart/ImageRangeEnd) via ClassifyDocument with TaskOptions retry policy (3 attempts, 5s first retry, backoff 2.0); (2) PersistResult (classification as UTF-8 byte[]); (3) if OverallConfidence < ConfidenceThreshold: add error, return; (4) route by first matching PageClassification — same priority order as ProcessDocumentWorkflow: Invoice→ExtractInvoice+PersistResult+ValidateInvoice+PersistResult; Contract→ExtractContract+PersistResult+ValidateContract+PersistResult; BoundedDocument→fall back to General path (nested BoundedDocument not supported per spec edge case); General→ExtractGeneralDocument+PersistResult; Email/None→log+skip; apply TaskOptions retry policy on every CallActivityAsync; (5) return WorkflowResult; use context.CreateReplaySafeLogger() (FR-007)

---

## Phase 8: Polish, Tests, Observability, and Documentation

**Goal**: All FR-013 unit tests pass; structured logging includes required context properties in every entry; local development documentation is updated.

**Independent Test**: `dotnet test` exits 0 with all tests passing; no test stubs or skips remain.

### Unit Tests — Orchestrations

- [ ] T051 [P] Unit tests for ProcessDocumentBatchWorkflow — two-folder fan-out scenario: mock TaskOrchestrationContext to return two DocumentFolder results from GetDocumentFolders; verify two CallSubOrchestrationAsync calls are made and WorkflowResult is aggregated; file: tests/DocumentProcessing.UnitTests/Orchestrations/ProcessDocumentBatchWorkflowTests.cs (FR-013)
- [ ] T052 [P] Unit tests for ProcessDocumentBatchWorkflow — zero-folder scenario: mock TaskOrchestrationContext to return empty DocumentFolders; verify no sub-orchestration calls are made and root returns WorkflowResult.Empty; file: tests/DocumentProcessing.UnitTests/Orchestrations/ProcessDocumentBatchWorkflowTests.cs (FR-013)
- [ ] T053 [P] Unit tests for ProcessDocumentWorkflow — Invoice chain: mock context to return invoice classification (OverallConfidence=1.0, Classification=Invoice); verify ClassifyDocument → PersistResult → ExtractInvoice → PersistResult → ValidateInvoice → PersistResult call sequence; file: tests/DocumentProcessing.UnitTests/Orchestrations/ProcessDocumentWorkflowTests.cs (FR-013)
- [ ] T054 [P] Unit tests for ProcessDocumentWorkflow — Contract chain: mock context to return contract classification (OverallConfidence=1.0, Classification=Contract); verify correct activity sequence; file: tests/DocumentProcessing.UnitTests/Orchestrations/ProcessDocumentWorkflowTests.cs (FR-013)
- [ ] T055 [P] Unit tests for ProcessDocumentWorkflow — General chain: mock context to return general classification (OverallConfidence=1.0, Classification=General); verify ClassifyDocument → PersistResult → ExtractGeneralDocument → PersistResult sequence; file: tests/DocumentProcessing.UnitTests/Orchestrations/ProcessDocumentWorkflowTests.cs (FR-013)
- [ ] T056 [P] Unit tests for ProcessDocumentWorkflow — BoundedDocument with two segments: mock context to return BoundedDocument classification; mock DetectBoundaries to return two DocumentSegment entries; verify two CallSubOrchestrationAsync(ProcessDocumentSegmentWorkflow) calls and fan-in aggregation; file: tests/DocumentProcessing.UnitTests/Orchestrations/ProcessDocumentWorkflowTests.cs (FR-013)
- [ ] T057 [P] Unit tests for ProcessDocumentWorkflow — BoundedDocument with zero segments: mock DetectBoundaries to return empty Segments list; verify no sub-orchestrations started and an error entry is added to WorkflowResult; file: tests/DocumentProcessing.UnitTests/Orchestrations/ProcessDocumentWorkflowTests.cs (FR-013)
- [ ] T058 [P] Unit tests for ProcessDocumentWorkflow — confidence below threshold: mock ClassifyDocument to return OverallConfidence=0.5; verify no type-specific activities called and an error entry is added to WorkflowResult; file: tests/DocumentProcessing.UnitTests/Orchestrations/ProcessDocumentWorkflowTests.cs (FR-013)
- [ ] T059 [P] Unit tests for ProcessDocumentWorkflow — Email and None classifications: mock ClassifyDocument to return Email; verify no type-specific activities called and no error added; repeat for None; file: tests/DocumentProcessing.UnitTests/Orchestrations/ProcessDocumentWorkflowTests.cs (FR-013)

### Unit Tests — Activities and Triggers

- [ ] T060 [P] Unit tests for each of the nine placeholder activity implementations: instantiate PlaceholderGetDocumentFoldersActivity, PlaceholderClassifyDocumentActivity, PlaceholderPersistResultActivity, PlaceholderExtractInvoiceActivity, PlaceholderValidateInvoiceActivity, PlaceholderExtractContractActivity, PlaceholderValidateContractActivity, PlaceholderDetectBoundariesActivity, PlaceholderExtractGeneralDocumentActivity; call ExecuteAsync and assert return contracts match FR-005 hardcoded specifications; file: tests/DocumentProcessing.UnitTests/Activities/PlaceholderActivityTests.cs (FR-013)
- [ ] T061 [P] Unit tests for ProcessDocumentsHttpTrigger — invalid request (empty ContainerName) returns HTTP 400 with Content-Type: application/problem+json and ProblemDetails fields; valid request starts orchestration and returns HTTP 202; mock DurableTaskClient; file: tests/DocumentProcessing.UnitTests/Triggers/ProcessDocumentsHttpTriggerTests.cs (FR-014)

### Observability

- [ ] T062 Review all log call sites in src/DocumentProcessing.Functions/ across triggers, orchestrations, and activity wrappers; ensure every log entry includes orchestrationInstanceId and correlationId as structured log properties via ILogger scope or named parameters per FR-008; add missing ILogger.BeginScope calls or log property parameters where absent; verify CreateReplaySafeLogger() is used in all orchestrators per FR-007

### Documentation

- [ ] T063 [P] Update docs/architecture/development-environment.md with DTS local emulator setup instructions: Docker pull command, docker run command exposing port 8080, required local.settings.json values (DTS_CONNECTION_STRING, TASKHUB_NAME), and Azurite startup command for queue and blob storage — per research.md Topic 7 and FR-010
- [ ] T064 [P] Unit tests for ProcessDocumentsQueueTrigger — (1) valid Base64-encoded `{ "container_name": "documents" }` message: verify ScheduleNewOrchestrationInstanceAsync is called once with a non-null ProcessDocumentBatchInput and instanceId + correlationId are logged as structured properties; (2) malformed/undecodable message: verify no orchestration is started, error is logged with correlationId, and the exception is rethrown; mock DurableTaskClient; file: tests/DocumentProcessing.UnitTests/Triggers/ProcessDocumentsQueueTriggerTests.cs (FR-001a, FR-008)
- [ ] T065 [P] Create a local integration smoke test that starts the Functions host in-process (using Microsoft.Azure.Functions.Worker.TestServer), posts `{ "container_name": "documents" }` to `POST /api/process-documents`, polls `statusQueryGetUri` until status is `Completed` or timeout (30 s), and asserts the output is a non-null WorkflowResult with zero errors — requires DTS local emulator and Azurite to be running; file: tests/DocumentProcessing.IntegrationTests/Smoke/BatchProcessingSmokeTest.cs (Constitution §VIII e2e)
- [ ] T066 [P] Create docs/architecture/adr/0003-fan-out-fan-in-with-nested-sub-orchestrations.md — ADR recording the decision to use Durable Functions fan-out/fan-in and nested sub-orchestrations for per-folder and per-segment parallel processing; capture context (batch document processing with heterogeneous document types and variable folder sizes), decision (fan-out/fan-in via CallSubOrchestrationAsync + Task.WhenAll), alternatives considered (sequential processing, custom queue-based fan-out), consequences (replay determinism constraints, DTS backend requirement), and reference Microsoft Durable Functions fan-out/fan-in pattern documentation (Constitution §IX ADR)

---

## Dependencies and Execution Order

### Phase Dependencies

| Phase | Depends On | Can Parallelize With |
|---|---|---|
| **Phase 1: Setup** | — | Nothing (must complete before Phase 2) |
| **Phase 2: Foundational** | Phase 1 complete (T001–T010, T067) | T011–T035 all parallelizable; T036 sequential |
| **Phase 3: US1** | Phase 2 complete (T036) | Phases 4–7 can draft after Phase 2 |
| **Phase 4: US2** | Phase 3 complete (T037–T038) | — |
| **Phase 5: US3** | Phase 4 complete (T039–T040) | Phase 6 (US4) |
| **Phase 6: US4** | Phase 4 complete (T039–T040) | Phase 5 (US3) |
| **Phase 7: US5** | Phase 5 complete (T041–T048) | Phase 8 Polish after each phase |
| **Phase 8: Polish** | T037–T050 all complete | T051–T067 all parallelizable |

### Parallel Execution Per Phase

**Phase 2**: T011–T035 and T067 may all execute simultaneously (different files, no cross-dependencies); T036 must follow T024–T035 and T008.

**Phase 5**: T041–T046 may execute simultaneously (different activity wrapper files); T047 depends on T022 (DocumentSegmentInput); T048 depends on T041–T047.

**Phase 8**: T051–T067 may all execute simultaneously.

### Story Completion Order

```
US1 (P1)
  └─► US2 (P2) — root orchestration needs HTTP trigger to validate
        └─► US3 (P3) — per-folder workflow needs root to call it
              ├─► US4 (P4) — can be implemented in parallel with US3 (shared orchestration)
              └─► US5 (P5) — segment workflow needs per-folder workflow to fan out
```

---

## Implementation Strategy

**MVP Scope (US1 + US2)**: Complete Phases 1–4 (T001–T040, T067). This gives a working HTTP trigger returning 202, a root orchestration fanning out to placeholder per-folder sub-orchestrations, and a polling status URL. Sufficient to validate the DTS local emulator integration end-to-end.

**Increment 2 (US3 + US4)**: Phases 5–6 (T041–T049). Adds first-match type-dispatch per-folder orchestration and queue trigger. All document types (Invoice, Contract, General, BoundedDocument skeleton) are routed correctly by priority order.

**Increment 3 (US5 + Polish)**: Phases 7–8 (T050–T067). Completes BoundedDocument nested fan-out; all FR-013 unit tests (T051–T061); queue trigger tests (T064); integration smoke test (T065); ADR-0003 (T066); observability review (T062); local dev documentation (T063).
