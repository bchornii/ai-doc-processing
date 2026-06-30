---

description: "Task list template for feature implementation"
---

# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Organization**: Tasks are grouped by discipline area. Every feature MUST include tasks in Testing, Documentation, and deployment validation.

## Format: `[ID] [P?] [Area] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Area]**: Discipline area abbreviation (INFRA, BE, AI, STORE, SEARCH, SEC, OBS, TEST, DOCS)
- Include exact file paths in descriptions

## Path Conventions

- **Source**: `src/` at repository root
- **Tests**: `tests/` at repository root
- **Infra / IaC**: `infra/`
- **Docs**: `docs/`
- Adjust paths based on plan.md structure

<!--
  ============================================================================
  IMPORTANT: The tasks below are SAMPLE TASKS for illustration purposes only.

  The /speckit.tasks command MUST replace these with actual tasks derived from:
  - Feature requirements in plan.md
  - User stories in spec.md
  - Entities in data-model.md
  - Endpoints / contracts in contracts/

  Every generated tasks.md MUST contain at least one task in each of the
  Testing, Documentation, and deployment validation sections.

  DO NOT keep these sample tasks in the generated tasks.md file.
  ============================================================================
-->

---

## Infrastructure

**Purpose**: Provisioning, configuration, and deployment pipeline for all cloud resources required by this feature.

- [ ] T001 [INFRA] Add / update Bicep modules for [resource] in `infra/modules/[resource].bicep`
- [ ] T002 [P] [INFRA] Update `azure.yaml` with new service or resource bindings
- [ ] T003 [P] [INFRA] Add environment variable / app-settings entries to `infra/main.bicep`
- [ ] T004 [INFRA] Validate `azd provision` completes without errors in a target environment

**Checkpoint**: All cloud resources are provisioned and reachable before downstream work begins.

---

## Backend

**Purpose**: Application logic, API endpoints, domain models, and service integrations.

- [ ] T005 [P] [BE] Create domain model `[Entity]` in `src/[Project]/Models/[Entity].cs`
- [ ] T006 [P] [BE] Define service interface `I[Service]` in `src/[Project]/Abstractions/I[Service].cs`
- [ ] T007 [BE] Implement `[Service]` in `src/[Project]/Services/[Service].cs` (depends on T006)
- [ ] T008 [BE] Register `[Service]` in DI container in `src/[Project]/Program.cs`
- [ ] T009 [BE] Implement [endpoint / function trigger] in `src/[Project]/[Location]/[File].cs`
- [ ] T010 [BE] Add input validation and structured error responses

**Checkpoint**: Backend compiles and core logic is exercisable locally before AI / storage / search wiring.

---

## AI

**Purpose**: AI model integration, prompt engineering, document intelligence, and inference pipelines.

- [ ] T011 [AI] Define prompt template for [scenario] in `src/[Project]/Prompts/[name].prompty`
- [ ] T012 [AI] Implement AI client wrapper in `src/[Project]/AI/[AIClient].cs`
- [ ] T013 [AI] Wire [Azure OpenAI / Document Intelligence / other] SDK in `src/[Project]/AI/[Pipeline].cs`
- [ ] T014 [P] [AI] Add retry / rate-limit handling to AI client
- [ ] T015 [AI] Validate end-to-end inference with a representative sample document

**Checkpoint**: AI pipeline returns correct structured output for at least one sample before storage/search integration.

---

## Storage

**Purpose**: Blob storage, queues, databases, and data persistence concerns.

- [ ] T016 [STORE] Define storage schema / container structure for [entity] per `data-model.md`
- [ ] T017 [STORE] Implement repository `[Entity]Repository` in `src/[Project]/Data/[Entity]Repository.cs`
- [ ] T018 [P] [STORE] Add connection string / managed identity configuration in `src/[Project]/Program.cs`
- [ ] T019 [STORE] Implement queue producer / consumer for [workflow step] in `src/[Project]/Messaging/[File].cs`
- [ ] T020 [STORE] Validate round-trip read/write against provisioned storage account

**Checkpoint**: Data persists and is retrievable correctly end-to-end.

---

## Search

**Purpose**: Indexing, search index schema, query logic, and semantic/vector search configuration.

- [ ] T021 [SEARCH] Define Azure AI Search index schema for [entity] in `infra/search/[index].json`
- [ ] T022 [SEARCH] Implement index management service in `src/[Project]/Search/[IndexService].cs`
- [ ] T023 [SEARCH] Implement query service in `src/[Project]/Search/[QueryService].cs`
- [ ] T024 [P] [SEARCH] Configure semantic configuration / vector profile in index schema
- [ ] T025 [SEARCH] Validate search returns expected results for representative queries

**Checkpoint**: Search index is populated and returns ranked results before observability and security hardening.

---

## Security

**Purpose**: Authentication, authorization, secret management, and threat mitigation.

- [ ] T026 [SEC] Enforce managed identity / workload identity for all service-to-service calls
- [ ] T027 [P] [SEC] Store secrets in Azure Key Vault; remove any hard-coded credentials
- [ ] T028 [P] [SEC] Apply least-privilege RBAC role assignments in `infra/roles.bicep`
- [ ] T029 [SEC] Validate that no sensitive values appear in logs or API responses
- [ ] T030 [SEC] Run OWASP / static analysis scan and remediate findings

**Checkpoint**: No credentials in code; all inter-service calls use managed identity.

---

## Observability

**Purpose**: Structured logging, distributed tracing, metrics, and alerting.

- [ ] T031 [OBS] Add structured logging to all new services using `ILogger<T>`
- [ ] T032 [P] [OBS] Emit custom metrics / events to Application Insights for [key operation]
- [ ] T033 [P] [OBS] Add correlation ID propagation across service boundaries
- [ ] T034 [OBS] Create / update dashboard or workbook for [feature] KPIs in `infra/monitoring/`
- [ ] T035 [OBS] Configure alert rule for error-rate threshold on [operation]

**Checkpoint**: Key operations are traceable end-to-end in Application Insights before release.

---

## Testing

**Purpose**: Unit, integration, contract, and end-to-end tests. Tests MUST be written before or alongside implementation; red-green cycle required.

> **NOTE: Ensure tests FAIL before the corresponding implementation is in place.**

- [ ] T036 [P] [TEST] Unit tests for `[Service]` in `tests/[Project].UnitTests/Services/[Service]Tests.cs`
- [ ] T037 [P] [TEST] Unit tests for `[Entity]Repository` in `tests/[Project].UnitTests/Data/[Entity]RepositoryTests.cs`
- [ ] T038 [P] [TEST] Unit tests for AI pipeline in `tests/[Project].UnitTests/AI/[Pipeline]Tests.cs`
- [ ] T039 [TEST] Integration tests for [endpoint] in `tests/[Project].IntegrationTests/[Scenario]Tests.cs`
- [ ] T040 [TEST] End-to-end smoke test: submit sample document → verify output in storage/search
- [ ] T041 [P] [TEST] Contract / schema validation tests for [API contract] in `tests/[Project].ContractTests/`

**Checkpoint**: All tests pass in CI. Coverage meets threshold defined in plan.md.

---

## Documentation

**Purpose**: Architecture decision records, README updates, API docs, and runbooks. Required for every feature.

- [ ] T042 [P] [DOCS] Update `README.md` with feature overview and local run instructions
- [ ] T043 [P] [DOCS] Add / update architecture decision record in `docs/architecture/adr/[####-title].md`
- [ ] T044 [P] [DOCS] Document new environment variables / configuration in `docs/[feature]-configuration.md`
- [ ] T045 [DOCS] Add API / contract reference to `docs/[feature]-api.md` (generated from contracts/)
- [ ] T046 [DOCS] Write operational runbook for [feature] in `docs/runbooks/[feature].md`
- [ ] T047 [DOCS] Update `docs/architecture/development-environment.md` if local setup changed

**Checkpoint**: A new developer can set up and understand the feature using only the updated docs.

---

## Dependencies & Execution Order

### Area Dependencies

| Area | Depends On | Can Parallelize With |
|---|---|---|
| **Infrastructure** | — | Security (RBAC) |
| **Backend** | Infrastructure (provisioned) | AI, Storage, Search |
| **AI** | Infrastructure | Backend (after interfaces defined), Storage |
| **Storage** | Infrastructure | Backend (after interfaces defined), AI |
| **Search** | Infrastructure, Storage | Backend |
| **Security** | Infrastructure | Backend, AI, Storage, Search |
| **Observability** | Backend, AI, Storage | Security |
| **Testing** | Each area's implementation | Run as each area completes |
| **Documentation** | All implementation complete | Can draft in parallel; finalize last |

### Parallel Opportunities

- All `[P]`-marked tasks within an area can run simultaneously
- Infrastructure, Security (RBAC), and Documentation drafting can start in parallel from day one
- Backend, AI, and Storage implementation can proceed in parallel once Infrastructure is provisioned
- Testing tasks for a completed area can begin immediately — do not wait for all areas to finish

---

## Deployment Validation Checklist

Every feature MUST pass these checks before the feature is considered done:

- [ ] `azd provision` completes without errors against a clean environment
- [ ] `azd deploy` completes and the function / service starts successfully
- [ ] End-to-end smoke test (T040) passes against the deployed environment
- [ ] No secrets or sensitive values in logs (T029)
- [ ] All CI pipeline jobs green (build, test, lint, security scan)
- [ ] Runbook reviewed and accurate (T046)

---

## Notes

- `[P]` = different files, no shared state — safe to run in parallel
- Every feature ships with Testing, Documentation, and deployment validation tasks — these are not optional
- Write tests before or alongside implementation; never after
- Commit after each area's checkpoint to enable bisecting if issues arise
- Adjust area sections: omit areas not applicable to the feature, add sub-sections for complex areas
