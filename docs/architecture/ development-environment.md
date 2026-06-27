# Development Environment

## Purpose

This repository uses a Development Container (Dev Container) to provide a reproducible, isolated, and consistent development environment across all contributors.

The goal is to ensure that every developer works with the same SDKs, command-line tools, and operating system configuration regardless of their host operating system.

The Dev Container is considered part of the source code and should evolve alongside the application.

---

# Design Principles

## 1. Infrastructure as Code

The development environment is treated as infrastructure.

Changes to the development environment should be committed, reviewed, and versioned just like application code.

Developers should never be required to manually install project-specific tools.

---

## 2. Separation of Responsibilities

The development environment is composed of several independent layers.

### Dockerfile

Responsible for operating system customization.

Examples:

* Linux packages
* Native libraries
* OCR engines
* Certificates
* Custom repositories

The Dockerfile should never contain project initialization logic.

---

### Dev Container Features

Responsible for installing standard developer tooling.

Examples:

* Azure CLI
* Azure Developer CLI (azd)
* Git
* GitHub CLI
* PowerShell

Whenever an official Feature exists, it should be preferred over implementing the installation manually inside the Dockerfile.

---

### post-create.sh

Responsible for repository initialization.

Examples:

* `dotnet restore`
* `dotnet tool restore`
* repository bootstrap

This script should not install operating system software.

---

### VS Code Customizations

Responsible only for IDE configuration.

Examples:

* Extensions
* Workspace settings
* Editor behavior

These settings should never affect the container operating system.

---

# Current Development Environment

Current development environment includes:

* .NET 10 SDK
* Git
* GitHub CLI
* Azure CLI
* Azure Developer CLI (azd)
* PowerShell

The environment intentionally excludes project-specific tooling that is not yet required.

Additional tooling should only be introduced when the project actually depends on it.

---

# Why Dockerfile Instead of Only Using an Image?

The project currently uses a minimal Dockerfile even though it performs no additional customization.

This establishes ownership of the development image from the beginning.

Future operating system customization can be added without changing the overall Dev Container architecture.

---

# Why Features Instead of Dockerfile?

Whenever possible, standard development tooling should be installed through Dev Container Features.

Advantages include:

* maintained by the Dev Container ecosystem
* reusable
* standardized
* easier to update
* less custom Docker maintenance

The Dockerfile should only be used when Features cannot satisfy the requirement.

---

# Why postCreateCommand?

Repository initialization belongs outside of the Docker image.

This keeps the Docker image reusable while allowing repository-specific initialization after the container is created.

Typical responsibilities include:

* restoring NuGet packages
* restoring local .NET tools
* project bootstrap

---

# Development Environment Lifecycle

The Dev Container lifecycle consists of four stages.

1. Docker image is built.
2. Container is created.
3. Repository initialization runs (`postCreateCommand`).
4. Developer starts working.

Application source code is never baked into the Docker image.

Instead, the repository is bind-mounted into the running container.

---

# Design Guidelines

When introducing a new tool, use the following decision process.

| Requirement               | Preferred Location    |
| ------------------------- | --------------------- |
| Native Linux package      | Dockerfile            |
| Standard developer tool   | Dev Container Feature |
| Repository initialization | post-create.sh        |
| IDE behavior              | VS Code customization |

---

# Future Evolution

As the solution evolves, the development environment is expected to grow with it.

Potential future additions include:

* Azure Functions Core Tools
* Azurite
* Docker Compose support
* OCR libraries
* AI development tooling
* OpenTelemetry utilities
* Additional VS Code extensions

New tooling should only be introduced when there is a concrete project requirement.

---

# Engineering Principles

* Keep the Dockerfile focused on operating system concerns.
* Prefer official Dev Container Features over custom installation logic.
* Keep project initialization outside of the Docker image.
* Every entry in `devcontainer.json` should have a documented purpose.
* Avoid unnecessary tooling. The development environment should remain minimal while fully supporting the project.

## Local .NET Tools

This repository uses a local .NET Tool Manifest (`.config/dotnet-tools.json`) to ensure all developers and CI environments use the same CLI tool versions.

The following tools are managed locally:

| Tool | Purpose |
|------|---------|
| dotnet-format | Code formatting |
| dotnet-reportgenerator-globaltool | Test coverage reports |
| dotnet-outdated-tool | NuGet dependency analysis |
| dotnet-ef | Entity Framework Core CLI |

Local tools are restored automatically by:

- `.devcontainer/post-create.sh`
- `bootstrap.sh`
- `bootstrap.ps1`

Developers should avoid installing project-specific tools globally.