# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: [e.g., C# .NET 10 isolated worker or NEEDS CLARIFICATION]
**Primary Dependencies**: [e.g., Azure Functions, Durable Functions, Azure SDKs, DI/Options packages]
**Storage**: [e.g., Cosmos DB, Azure SQL, Blob Storage, Table Storage, Queue Storage or N/A]
**Testing**: [e.g., xUnit, integration tests against local emulators/Testcontainers, contract tests]
**Target Platform**: [e.g., Azure Functions (Linux Consumption/Premium), Container Apps, AKS]
**Project Type**: [cloud-native service/workflow/event-driven app]
**Performance Goals**: [e.g., orchestrations/day, activity latency SLOs, queue drain time]
**Constraints**: [e.g., cold-start tolerance, idempotency requirements, retry/backoff limits, cost cap]
**Scale/Scope**: [e.g., documents/day, peak concurrency, regions, tenants]

## Architecture Changes

**Current Architecture**: [Describe the existing Azure/application architecture relevant to this change]

**Proposed Architecture**: [Describe the updated architecture and interaction flow]

**Impacted Components**:
- [Function app(s), orchestrator(s), activity functions, APIs, queues, topics, storage accounts]
- [Domain/application/infrastructure modules impacted]
- [External dependencies/services impacted]

**Architecture Diagram Updates**: [List diagram files that must be updated and what changes are required]

## Durable Functions Orchestration

**Orchestration Pattern**: [e.g., chaining, fan-out/fan-in, async HTTP API, monitor, human interaction]

**Orchestrator(s)**:
- [Name]: [Responsibility, trigger/start condition, completion criteria]

**Activity Functions**:
- [Name]: [Input/Output contract, idempotency strategy, timeout/retry policy]

**State and Determinism Considerations**:
- [Deterministic code constraints addressed]
- [Checkpointing/state persistence expectations]
- [Duplicate message and replay handling]

**Error Handling and Compensation**:
- [Transient fault strategy]
- [Compensating actions/saga rollback steps]
- [Poison message/dead-letter handling]

## Infrastructure (Bicep)

**Modules to Add/Update**:
- [infra/modules/...]
- [infra/main.bicep or environment-specific composition files]

**Resources**:
- [Function App plan, Storage Account, Key Vault, Application Insights, Log Analytics, Service Bus, etc.]

**Parameters and Secrets**:
- [New parameters]
- [Key Vault references / managed identity usage]

**Environment Strategy**:
- [dev/test/prod differences]
- [Regional strategy and redundancy]

**Validation**:
- [bicep build/lint/what-if commands and expected checks]

## Data Model

**Entities and Schemas**:
- [Entity/Table/Container]
- [Key fields, partition keys, indexes, retention]

**Data Lifecycle**:
- [Ingestion path]
- [Transformation/enrichment path]
- [Archival/deletion policy]

**Consistency and Migration Plan**:
- [Consistency model expectations]
- [Backward compatibility]
- [Migration/seeding steps]

## Security

**Identity and Access**:
- [Managed identity assignments]
- [RBAC roles required]
- [Least privilege notes]

**Secret and Key Management**:
- [Key Vault integration]
- [Secret rotation expectations]

**Network and Data Protection**:
- [Private endpoints/VNet integration]
- [Encryption at rest/in transit]
- [Data classification/PII handling]

**Threat Considerations**:
- [Abuse cases and mitigations]
- [Audit and compliance requirements]

## Monitoring

**Telemetry**:
- [Structured logs]
- [Distributed tracing correlation across orchestrator/activity/external calls]
- [Custom metrics]

**Dashboards and Alerts**:
- [Key dashboards]
- [Alert rules, thresholds, severity, on-call routing]

**Operational Runbooks**:
- [Incident triage steps]
- [Known failure signatures and remediation]

## Testing Strategy

**Unit Tests**:
- [Core domain logic, orchestrator helper logic, validation]

**Integration Tests**:
- [Azure SDK integration, storage/messaging workflows, host-level testing]

**End-to-End Tests**:
- [Happy path and failure path orchestration scenarios]

**Non-Functional Tests**:
- [Load, resilience/chaos, security scanning, cost/performance regression]

**Quality Gates**:
- [Required pass criteria in CI/CD before deployment]

## Deployment Strategy

**Release Approach**: [e.g., blue/green, canary, ring-based, incremental by environment]

**CI/CD Changes**:
- [Pipeline stages and approvals]
- [Infra deployment order vs app deployment order]

**Configuration Management**:
- [App settings/feature flags/environment variable strategy]

**Post-Deployment Verification**:
- [Smoke tests, health checks, telemetry verification]

## Rollback Strategy

**Rollback Triggers**:
- [Error budget burn, failed health checks, SLO breach, security concern]

**Rollback Steps**:
1. [Traffic shift or version revert]
2. [Infrastructure rollback/forward-fix decision]
3. [Data rollback/compensation process]

**Recovery Validation**:
- [How rollback success is verified]
- [Communications and incident documentation requirements]

## Risks

| Risk | Impact | Likelihood | Mitigation | Owner |
|------|--------|------------|------------|-------|
| [e.g., Durable orchestration replay side effects] | [H/M/L] | [H/M/L] | [Idempotency + deterministic code review] | [Team/Role] |
| [e.g., Throughput bottleneck in downstream service] | [H/M/L] | [H/M/L] | [Backpressure + queue scaling + rate limits] | [Team/Role] |
| [e.g., IaC drift across environments] | [H/M/L] | [H/M/L] | [what-if checks + policy + drift detection] | [Team/Role] |
| [e.g., Secret exposure risk] | [H/M/L] | [H/M/L] | [Managed identity + Key Vault references] | [Team/Role] |

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

[Gates determined based on constitution file]

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete Azure
  solution layout for this feature. Delete unused paths and expand the selected
  structure with real directories for application, infrastructure, tests, and
  operational documentation.
-->

```text
src/
├── [Domain].Application/
├── [Domain].Contracts/
├── [Domain].Core/
├── [Domain].Functions/
└── [Domain].Infrastructure/

infra/
├── main.bicep
├── modules/
└── environments/

tests/
├── [Domain].UnitTests/
├── [Domain].IntegrationTests/
└── [Domain].ContractTests/

docs/
├── architecture/
└── runbooks/

# [REMOVE IF UNUSED] Example: supporting web/API surface
web/
└── [Portal or UI]

# [REMOVE IF UNUSED] Example: additional worker or integration service
src/
└── [Domain].Worker/
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
