#!/usr/bin/env bash

set -e

echo "========================================="
echo "AI Document Processing - Dev Container"
echo "========================================="

echo "Validating development environment..."
bash scripts/validate-dev-env.sh

echo ""
echo "Restoring NuGet packages..."
dotnet restore

if [ -f ".config/dotnet-tools.json" ]; then
    echo ""
    echo "Restoring local .NET tools..."
    dotnet tool restore
fi

echo ""
echo "Installed local tools:"
dotnet tool list

echo ""
echo "Environment is ready."