# Development Container

## Purpose

Provides a reproducible development environment for the AI Document Processing solution.

## Responsibilities

The Dev Container provides:

- .NET SDK
- Azure CLI
- Azure Developer CLI
- PowerShell
- Git
- GitHub CLI

## Dockerfile

Owns operating system customization.

Examples:

- apt packages
- native libraries
- certificates

## post-create.sh

Owns repository initialization.

Examples:

- dotnet restore
- dotnet tool restore

## Features

Own standard development tooling.

Examples:

- Azure CLI
- Git
- PowerShell

## Rebuilding

After changing:

- Dockerfile
- Features

Run:

Dev Containers: Rebuild Container

Changing only source code does not require rebuilding.