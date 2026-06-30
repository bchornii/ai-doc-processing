# Feature Specification: [FEATURE NAME]

**Feature Branch**: `[###-feature-name]`
**Created**: [DATE]
**Status**: Draft
**Input**: User description: "$ARGUMENTS"

## User Scenarios & Testing *(mandatory)*

<!--
  IMPORTANT: User stories should be PRIORITIZED as user journeys ordered by importance.
  Each user story/journey must be INDEPENDENTLY TESTABLE - meaning if you implement just ONE of them,
  you should still have a viable MVP (Minimum Viable Product) that delivers value.

  Assign priorities (P1, P2, P3, etc.) to each story, where P1 is the most critical.
  Think of each story as a standalone slice of functionality that can be:
  - Developed independently
  - Tested independently
  - Deployed independently
  - Demonstrated to users independently
-->

### User Story 1 - [Brief Title] (Priority: P1)

[Describe this user journey in plain language]

**Why this priority**: [Explain the value and why it has this priority level]

**Independent Test**: [Describe how this can be tested independently - e.g., "Can be fully tested by [specific action] and delivers [specific value]"]

**Acceptance Scenarios**:

1. **Given** [initial state], **When** [action], **Then** [expected outcome]
2. **Given** [initial state], **When** [action], **Then** [expected outcome]

---

### User Story 2 - [Brief Title] (Priority: P2)

[Describe this user journey in plain language]

**Why this priority**: [Explain the value and why it has this priority level]

**Independent Test**: [Describe how this can be tested independently]

**Acceptance Scenarios**:

1. **Given** [initial state], **When** [action], **Then** [expected outcome]

---

### User Story 3 - [Brief Title] (Priority: P3)

[Describe this user journey in plain language]

**Why this priority**: [Explain the value and why it has this priority level]

**Independent Test**: [Describe how this can be tested independently]

**Acceptance Scenarios**:

1. **Given** [initial state], **When** [action], **Then** [expected outcome]

---

[Add more user stories as needed, each with an assigned priority]

### Edge Cases

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right edge cases.
-->

- What happens when [boundary condition]?
- How does system handle [error scenario]?

## Requirements *(mandatory)*

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right functional requirements.
-->

### Functional Requirements

- **FR-001**: System MUST [specific capability, e.g., "allow users to create accounts"]
- **FR-002**: System MUST [specific capability, e.g., "validate email addresses"]
- **FR-003**: Users MUST be able to [key interaction, e.g., "reset their password"]
- **FR-004**: System MUST [data requirement, e.g., "persist user preferences"]
- **FR-005**: System MUST [behavior, e.g., "log all security events"]

*Example of marking unclear requirements:*

- **FR-006**: System MUST authenticate users via [NEEDS CLARIFICATION: auth method not specified - email/password, SSO, OAuth?]
- **FR-007**: System MUST retain user data for [NEEDS CLARIFICATION: retention period not specified]

### Key Entities *(include if feature involves data)*

- **[Entity 1]**: [What it represents, key attributes without implementation]
- **[Entity 2]**: [What it represents, relationships to other entities]

## Azure Services Involved *(mandatory for cloud features)*

<!--
  ACTION REQUIRED: List all Azure services this feature depends on.
  Include both existing services and any newly introduced services.
-->

- **Primary Services**: [e.g., Azure Functions, Azure Storage, Azure Service Bus, Azure AI Foundry, Azure Key Vault]
- **Supporting Services**: [e.g., Application Insights, Azure Monitor, Event Grid]
- **Service Responsibilities**:
  - [Service Name]: [Why it is needed for this feature]
  - [Service Name]: [Why it is needed for this feature]
- **Regional/Residency Constraints**: [Any region, data residency, or sovereignty requirements]

## Security Considerations *(mandatory)*

<!--
  ACTION REQUIRED: Describe feature-specific security controls and data protections.
-->

- **Data Classification**: [Public/Internal/Confidential/Restricted]
- **Data Protection**: [Encryption at rest/in transit requirements, sensitive data handling]
- **Secret Handling**: [How secrets are avoided or retrieved securely, e.g., Key Vault references]
- **Threat Considerations**: [Primary abuse/misuse scenarios and mitigations]
- **Compliance Needs**: [Regulatory or policy requirements, if any]

## Identity and RBAC *(mandatory)*

<!--
  ACTION REQUIRED: Define authentication and authorization model for all actors.
-->

- **Workload Identity**: [Managed Identity type and scope]
- **Human Access Model**: [Developer/operator access patterns]
- **RBAC Roles Required**:
  - [Principal]: [Role] on [Scope] because [reason]
  - [Principal]: [Role] on [Scope] because [reason]
- **Least Privilege Plan**: [How permissions are minimized and reviewed]

## Observability Requirements *(mandatory)*

<!--
  ACTION REQUIRED: Define logs, metrics, traces, and alerting needed to operate the feature.
-->

- **Structured Logging**: [Key events and required fields/correlation IDs]
- **Distributed Tracing**: [Spans that must exist across components]
- **Metrics**: [SLIs/SLO-related metrics, throughput, error rates, latency]
- **Dashboards/Workbooks**: [Operational views to be created/updated]
- **Alerting**: [Conditions, thresholds, and responders]

## Performance Expectations *(mandatory)*

<!--
  ACTION REQUIRED: State measurable non-functional performance targets.
-->

- **Latency Targets**: [p50/p95/p99 targets for core operations]
- **Throughput Targets**: [Expected sustained and peak load]
- **Scalability Expectations**: [Auto-scale behavior, concurrency assumptions]
- **Resilience Targets**: [Retry behavior, timeout budgets, recovery objectives]

## Cost Considerations *(mandatory)*

<!--
  ACTION REQUIRED: Estimate and manage feature cost impact.
-->

- **Cost Drivers**: [Requests, storage, tokens, compute time, egress, etc.]
- **Estimated Cost Impact**: [Low/Medium/High plus rough estimate if known]
- **Cost Controls**: [Budgets, quotas, scaling limits, caching, retention policies]
- **Cost Monitoring Plan**: [How spend and anomalies will be tracked]

## Infrastructure Changes *(mandatory if infrastructure is affected)*

<!--
  ACTION REQUIRED: Describe IaC changes needed to ship this feature.
-->

- **Bicep Changes**: [Modules/resources to add/update/remove]
- **Environment Impact**: [Dev/Test/Prod differences and rollout sequencing]
- **Configuration Changes**: [App settings, feature flags, networking, policy]
- **Deployment Dependencies**: [Ordering constraints and prerequisites]

## Operational Impact *(mandatory)*

<!--
  ACTION REQUIRED: Describe ongoing operational responsibilities introduced by this feature.
-->

- **Runbook Updates**: [New or updated operational procedures]
- **Support Impact**: [Expected incident classes and support readiness]
- **On-call Considerations**: [Alert volume, escalation path, ownership]
- **Backup/Restore or Recovery Changes**: [Operational recovery implications]

## Risks *(mandatory)*

<!--
  ACTION REQUIRED: Identify key delivery and runtime risks with mitigations.
-->

- **Risk**: [Description]
  - **Likelihood**: [Low/Medium/High]
  - **Impact**: [Low/Medium/High]
  - **Mitigation**: [Plan]
  - **Contingency**: [Fallback]

- **Risk**: [Description]
  - **Likelihood**: [Low/Medium/High]
  - **Impact**: [Low/Medium/High]
  - **Mitigation**: [Plan]
  - **Contingency**: [Fallback]

## Out of Scope *(mandatory)*

<!--
  ACTION REQUIRED: Clearly define what this feature does not include.
  This prevents scope creep and improves planning accuracy.
-->

- [Explicitly excluded item]
- [Explicitly excluded item]
- [Deferred item for future phase]

## Success Criteria *(mandatory)*

<!--
  ACTION REQUIRED: Define measurable success criteria.
  These must be technology-agnostic and measurable.
-->

### Measurable Outcomes

- **SC-001**: [Measurable metric, e.g., "Users can complete account creation in under 2 minutes"]
- **SC-002**: [Measurable metric, e.g., "System handles 1000 concurrent users without degradation"]
- **SC-003**: [User satisfaction metric, e.g., "90% of users successfully complete primary task on first attempt"]
- **SC-004**: [Business metric, e.g., "Reduce support tickets related to [X] by 50%"]

## Assumptions

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right assumptions based on reasonable defaults
  chosen when the feature description did not specify certain details.
-->

- [Assumption about target users, e.g., "Users have stable internet connectivity"]
- [Assumption about scope boundaries, e.g., "Mobile support is out of scope for v1"]
- [Assumption about data/environment, e.g., "Existing authentication system will be reused"]
- [Dependency on existing system/service, e.g., "Requires access to the existing user profile API"]
