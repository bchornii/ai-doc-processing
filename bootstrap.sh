#!/usr/bin/env bash

set -e

echo "========================================="
echo "Bootstrapping Development Environment"
echo "========================================="

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
echo "Bootstrap completed successfully."