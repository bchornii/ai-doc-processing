# Research: Durable Functions Workflow Foundation

**Feature**: `001-durable-functions-workflow`
**Date**: 2026-06-30
**Status**: Complete — all NEEDS CLARIFICATION items resolved

---

## Topic 1: Orchestrator Code Style — Function-Based vs. Class-Based

**Decision**: Use the **class-based approach** with the `Microsoft.DurableTask.Generators` source generator for orchestrators and activities.

**Rationale**:
- Class-based orchestrators and activities use the `[DurableTask]` attribute and inherit from `TaskOrchestrator<TInput, TOutput>` / `TaskActivity<TInput, TOutput>`, which enables proper constructor-based dependency injection in activities.
- The source generator emits the required `[Function]` and trigger-binding boilerplate, keeping orchestrator and activity classes clean.
- This style makes activity classes independently testable as plain .NET classes without requiring the Functions host.
- Aligns with the Engineering Constitution requirement that activity functions be "independently testable."

**Alternatives considered**:
- Function-based (static methods with `[Function]` + `[OrchestrationTrigger]`/`[ActivityTrigger]`): Simpler setup but no DI support in activities; harder to unit test.

**References**:
- [Azure Durable Functions — Class-based syntax](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-isolated-overview)
- Skill file: `.github/skills/durable-functions-dotnet/SKILL.md`

---

## Topic 2: NuGet Package Selection

**Decision**: The following packages are required for the `DocumentProcessing.Functions` project:

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.Azure.Functions.Worker` | `2.*` | Isolated worker host |
| `Microsoft.Azure.Functions.Worker.Sdk` | `2.*` | Build tools for Functions |
| `Microsoft.Azure.Functions.Worker.Extensions.Http` | `3.*` | HTTP trigger |
| `Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore` | `1.*` | ASP.NET Core integration for HTTP triggers |
| `Microsoft.Azure.Functions.Worker.Extensions.DurableTask` | `1.*` | Durable Functions bindings |
| `Microsoft.Azure.Functions.Worker.Extensions.DurableTask.AzureManaged` | `*` | Azure Durable Task Scheduler backend |
| `Microsoft.Azure.Functions.Worker.Extensions.Storage.Queues` | `6.*` | Queue trigger for `documents` queue |
| `Microsoft.Azure.Functions.Worker.ApplicationInsights` | `*` | Application Insights telemetry integration |
| `Microsoft.DurableTask.Generators` | `*` | Source generator for class-based orchestrators/activities |
| `Azure.Identity` | `1.*` | Managed Identity authentication |

**Rationale**: Packages align with the DTS ADR (`ADR-0002`) and the skill file guidance. `Microsoft.Azure.Functions.Worker.Extensions.DurableTask.AzureManaged` provides the DTS storage provider.

---

## Topic 3: Unit Testing Strategy for Durable Functions

**Decision**: Use **xUnit** with **NSubstitute** for mocking. Test orchestrators using the Durable Functions isolated worker test support (`Microsoft.DurableTask.Testing` if available) or mock `TaskOrchestrationContext` directly; test activities as plain classes via their service interfaces.

**Rationale**:
- Activity implementations are plain .NET classes in `DocumentProcessing.Infrastructure` — no Functions host required for testing.
- Orchestrators can be tested by mocking `TaskOrchestrationContext` and verifying `CallActivityAsync` / `CallSubOrchestrationAsync` invocations and sequencing.
- NSubstitute is preferred over Moq due to simpler async mocking syntax and no `Setup`/`Returns` boilerplate.

**Alternatives considered**:
- Integration tests against the DTS local emulator: deferred to a subsequent phase; too heavy for the skeleton validation requirements.

---

## Topic 4: Layer Assignment for Orchestrators

**Decision**: Orchestrator classes (`ProcessDocumentBatchWorkflow`, `ProcessDocumentWorkflow`, `ProcessDocumentSegmentWorkflow`) reside in **`DocumentProcessing.Functions`** (the host project), NOT in `DocumentProcessing.Application`.

**Rationale**:
- Orchestrators must be registered as Azure Functions (i.e., they emit `[Function]` attributes via the source generator). The source generator resolves correctly only when the class is in the project that has the Functions SDK references.
- Orchestrators are workflow coordinators, not business logic holders. The constitution treats them as the "thin entry point" for workflow coordination — analogous to how HTTP triggers are thin entry points for HTTP requests.
- Activity SERVICE interfaces (ports) live in `DocumentProcessing.Application`. Activity implementations live in `DocumentProcessing.Infrastructure`. Orchestrators in `DocumentProcessing.Functions` reference application-layer ports to delegate routing decisions.

---

## Topic 5: Confidence Threshold Placement

**Decision**: The confidence threshold constant (`ConfidenceThreshold = 0.8`) is defined as a `public const double` in **`DocumentProcessing.Core`**, in a static class `DocumentProcessingConstants` (or on the `Classifications` type itself). Referenced symbolically by all orchestrators. Never appears as an inline literal.

**Rationale**: Domain constants belong in the domain layer. The threshold is a business rule — a confidence below 0.8 is insufficient for automated processing. Placing it in Core ensures orchestrators and any future domain services reference the same authoritative value.

---

## Topic 6: Correlation ID Propagation

**Decision**: The HTTP trigger extracts `X-Correlation-Id` from the request headers. If absent, a new `Guid` is generated via `Guid.NewGuid()` in the trigger (trigger code is NOT subject to determinism constraints — only orchestrators are). The correlation ID is passed as part of the orchestration input and propagated to all activity inputs and log scopes.

**Rationale**:
- Triggers run outside orchestration replay — `Guid.NewGuid()` is safe.
- Propagating the correlation ID in the orchestration input (not reading it from `context` mid-orchestration) satisfies the determinism rule.
- Every activity receives `correlationId` as part of its input or via a shared input base type.

---

## Topic 7: Local Development Setup

**Decision**: Local development requires two emulators:
1. **Azurite** (Azure Storage emulator) — provides local Queue Storage (for the `documents` queue trigger) and local Blob Storage.
2. **DTS local emulator** (Docker-based) — provides the Durable Task Scheduler backend on `http://localhost:8080`.

`local.settings.json` (git-ignored; `local.settings.template.json` committed):
```json
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "DTS_CONNECTION_STRING": "Endpoint=http://localhost:8080;Authentication=None",
    "TASKHUB_NAME": "default"
  }
}
```

**Rationale**: FR-010 requires the solution to be executable locally without any deployed cloud resources. Both emulators are the official Microsoft local development path for their respective services.

---

## Topic 8: RFC 9457 Problem Details Response

**Decision**: HTTP 400 responses use `Microsoft.AspNetCore.Mvc.ProblemDetails` serialized to JSON with `Content-Type: application/problem+json`. Fields: `title` (string), `status` (400), `detail` (string describing the validation failure).

**Rationale**: FR-014 mandates RFC 9457 conformance. ASP.NET Core's built-in `ProblemDetails` type satisfies this contract without additional dependencies. The `Extensions.Http.AspNetCore` package enables `HttpRequest`-based responses in the isolated worker, making `ProblemDetails` easy to return.

---

## All NEEDS CLARIFICATION Items: Resolved

| Item | Resolution |
|---|---|
| Orchestrator style | Class-based with source generator |
| NuGet package list | Defined (Topic 2) |
| Unit testing approach | xUnit + NSubstitute; orchestrators via mocked context; activities as plain classes |
| Layer for orchestrators | `DocumentProcessing.Functions` |
| Confidence threshold location | `DocumentProcessing.Core` — `DocumentProcessingConstants.ConfidenceThreshold` |
| Correlation ID generation | In trigger (safe); propagated as orchestration input |
| Local emulators | Azurite + DTS Docker emulator |
| HTTP 400 format | `ProblemDetails` (`application/problem+json`) |
