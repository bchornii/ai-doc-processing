# ADR 0001: Use Dev Containers for a Repeatable IaC Development Environment

- Status: Accepted
- Date: 2026-06-27
- Decision Makers: Engineering Team

## Context

The project requires a consistent local development environment across contributors and CI-aligned workflows. We currently rely on multiple tools (for example .NET SDK, Azure CLI, and supporting utilities), and machine-specific setup differences can lead to:

- onboarding friction and setup drift,
- inconsistent behavior across operating systems,
- "works on my machine" issues,
- harder reproducibility when debugging or validating changes.

We need a repeatable approach that treats development environment setup as Infrastructure as Code (IaC), with source-controlled configuration and minimal manual steps.

## Decision

We will use Dev Containers (`.devcontainer`) as the standard development environment for this repository.

The devcontainer configuration will be committed to source control and treated as IaC for developer workstations. It will define:

- base image and runtime dependencies,
- required SDKs and CLI tools,
- editor extensions and workspace settings where needed,
- bootstrap and post-create behavior for reliable setup.

## Alternatives Considered

1. Manual host setup documentation only.
- Pros: no container overhead.
- Cons: high drift risk, setup inconsistency, and onboarding cost.

2. Per-OS setup scripts without containerization.
- Pros: can automate some setup.
- Cons: still susceptible to host differences and dependency conflicts.

3. VM-based development environments.
- Pros: stronger isolation.
- Cons: heavier resource usage and higher operational overhead than devcontainers.

## Consequences

### Positive

- Reproducible development environments across contributors.
- Faster onboarding with reduced local setup effort.
- Lower configuration drift and fewer environment-related defects.
- Better alignment between local development and automated pipelines.

### Negative / Trade-offs

- Additional container startup and image build time.
- Developers need Docker/compatible container runtime support.
- Periodic maintenance of devcontainer definition and base images.

## Implementation Notes

- Keep devcontainer definitions versioned and reviewed like application code.
- Update documentation to make devcontainer usage the default path for contributors.
- Revisit this decision if team constraints (performance, tooling, security) materially change.
