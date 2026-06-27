$ErrorActionPreference = "Stop"

Write-Host "========================================="
Write-Host "Bootstrapping Development Environment"
Write-Host "========================================="
Write-Host ""

Write-Host "Restoring NuGet packages..."
dotnet restore

if (Test-Path ".config/dotnet-tools.json") {
    Write-Host ""
    Write-Host "Restoring local .NET tools..."
    dotnet tool restore
}

Write-Host ""
Write-Host "Installed local tools:"
dotnet tool list

Write-Host ""
Write-Host "Bootstrap completed successfully."