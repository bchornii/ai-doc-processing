# ADR 0002: Use Azure Durable Task Scheduler as the Durable Functions State Backend

- Status: Accepted
- Date: 2026-06-30
- Decision Makers: Engineering Team

## Context

Azure Durable Functions requires a durable state backend (task hub) to persist orchestration history, manage activity scheduling, and support replay. Two backend options are available:

- **Azure Storage backend** (default): Uses Azure Storage tables, queues, and blobs. Mature and widely used, but introduces poll-based scheduling, higher latency, and management overhead for large-scale workloads.
- **Azure Durable Task Scheduler (DTS)**: A first-party managed backend announced by Microsoft as the recommended replacement for the Azure Storage backend. Provides push-based scheduling, lower latency, built-in task hub management, and eliminates the need to manage storage resources for orchestration state.

This decision was initially captured as a clarification in the feature specification for feature `001-durable-functions-workflow`:

> Q: Which Durable Functions storage backend should be used? → A: Azure Durable Task Scheduler (DTS) — the recommended backend replacing Azure Storage. Local development uses the DTS local emulator (Docker-based); cloud deployments use a provisioned DTS resource.

## Decision

We will use **Azure Durable Task Scheduler (DTS)** as the durable orchestration state backend for all Durable Functions workloads in this repository.

For local development, the DTS local emulator (Docker image provided by Microsoft) will be used. Cloud deployments will provision a DTS resource via Bicep. The task hub name will be configurable via App Settings and consistent across all environments.

## Alternatives Considered

### Azure Storage Backend (Default)

- **Pro**: Zero additional infrastructure; uses existing Azure Storage account; mature, battle-tested.
- **Con**: Poll-based scheduling introduces latency; managing storage artifacts (tables, queues, blobs) for orchestration state adds operational complexity; Microsoft guidance recommends migrating to DTS for new workloads.
- **Rejected** because: Microsoft now recommends DTS for new Durable Functions workloads, and DTS provides lower latency and simpler operational model.

### Netherite (FASTER-based backend)

- **Pro**: Very high throughput for event-sourced workloads.
- **Con**: Requires Azure Event Hubs provisioning; higher operational complexity; not the primary Microsoft-recommended path for general workloads.
- **Rejected** because: Adds significant infrastructure complexity without a clear throughput requirement at this stage.

## Consequences

**Positive**:
- Lower orchestration scheduling latency compared to Azure Storage backend.
- No need to manage storage tables and queues for orchestration state.
- Built-in task hub management via the DTS resource.
- Aligns with current Microsoft guidance for new Durable Functions workloads.

**Negative / Trade-offs**:
- Requires a DTS resource to be provisioned in cloud environments (additional Azure resource and cost).
- DTS local emulator must be running during local development (adds a Docker dependency alongside Azurite).
- DTS is a newer service; there is a small risk of behavioural divergence between the local emulator and the cloud service (mitigated by integration testing against a real DTS resource in CI).

**Neutral**:
- The Durable Functions SDK integration is unchanged; only the backend provider configuration changes.
- The task hub name is configurable and does not need to change between environments.

## References

- [Azure Durable Task Scheduler documentation](https://learn.microsoft.com/azure/azure-functions/durable/durable-task-scheduler/durable-task-scheduler)
- [Durable Functions storage providers overview](https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-storage-providers)
- Engineering Constitution Section IX — Architecture Decision Discipline
