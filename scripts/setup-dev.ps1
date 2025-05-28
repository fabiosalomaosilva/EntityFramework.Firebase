#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Script para configurar o ambiente de desenvolvimento

.DESCRIPTION
    Este script configura o ambiente de desenvolvimento, instalando ferramentas necessárias
    e configurando o projeto para desenvolvimento local.

.PARAMETER InstallTools
    Instala ferramentas globais do .NET necessárias para desenvolvimento

.PARAMETER SetupEmulator
    Configura e inicia o emulador do Firestore (requer Docker)

.EXAMPLE
    .\setup-dev.ps1 -InstallTools -SetupEmulator
#>

param(
    [Parameter(Mandatory = $false)]
    [switch]$InstallTools,

    [Parameter(Mandatory = $false)]
    [switch]$SetupEmulator
)

$ErrorActionPreference = "Stop"

# Colors for output
$Red = "`e[31m"
$Green = "`e[32m"
$Yellow = "`e[33m"
$Blue = "`e[34m"
$Reset = "`e[0m"

function Write-Step {
    param([string]$Message)
    Write-Host "${Blue}==>${Reset} $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "${Green}✓${Reset} $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "${Yellow}⚠${Reset} $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "${Red}✗${Reset} $Message" -ForegroundColor Red
}

Write-Step "Setting up EntityFramework.Firebase development environment"

# Verify we're in the right directory
if (-not (Test-Path "EntityFramework.Firebase.sln")) {
    Write-Error "EntityFramework.Firebase.sln not found. Please run this script from the repository root."
    exit 1
}

# Check .NET version
Write-Step "Checking .NET version"
$dotnetVersion = dotnet --version
Write-Host "  .NET SDK Version: $dotnetVersion"

if ($dotnetVersion -lt "8.0.0") {
    Write-Error ".NET 8.0 or later is required"
    exit 1
}

Write-Success ".NET SDK version is compatible"

if ($InstallTools) {
    Write-Step "Installing global .NET tools"

    # Install GitVersion
    Write-Host "  Installing GitVersion..."
    dotnet tool install --global GitVersion.Tool --version 5.*

    # Install NuGet package validator
    Write-Host "  Installing NuGet package validator..."
    dotnet tool install --global Meziantou.Framework.NuGetPackageValidation.Tool

    # Install Entity Framework tools
    Write-Host "  Installing Entity Framework tools..."
    dotnet tool install --global dotnet-ef

    # Install coverage tools
    Write-Host "  Installing coverage tools..."
    dotnet tool install --global dotnet-reportgenerator-globaltool

    Write-Success "Global tools installed successfully"
}

# Restore packages
Write-Step "Restoring NuGet packages"
dotnet restore EntityFramework.Firebase.sln
if ($LASTEXITCODE -ne 0) {
    Write-Error "Package restore failed"
    exit $LASTEXITCODE
}

# Build solution
Write-Step "Building solution"
dotnet build EntityFramework.Firebase.sln --configuration Debug
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit $LASTEXITCODE
}

Write-Success "Solution built successfully"

if ($SetupEmulator) {
    Write-Step "Setting up Firestore emulator"

    # Check if Docker is available
    try {
        docker --version | Out-Null
        Write-Success "Docker is available"
    } catch {
        Write-Warning "Docker is not available. Firestore emulator requires Docker."
        Write-Host "Please install Docker Desktop and try again."
        return
    }

    # Create docker-compose file for Firestore emulator
    $dockerComposeContent = @"
version: '3.8'
services:
  firestore-emulator:
    image: gcr.io/google.com/cloudsdktool/google-cloud-cli:emulators
    ports:
      - "8080:8080"
      - "4000:4000"
    command: >
      sh -c "gcloud emulators firestore start --host-port=0.0.0.0:8080 --rules=/dev/null"
    environment:
      - FIRESTORE_EMULATOR_HOST=localhost:8080
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080"]
      interval: 30s
      timeout: 10s
      retries: 3
"@

    $dockerComposeContent | Out-File -FilePath "docker-compose.yml" -Encoding UTF8
    Write-Success "Docker Compose file created"

    Write-Host ""
    Write-Host "To start the Firestore emulator, run:"
    Write-Host "  docker-compose up -d firestore-emulator"
    Write-Host ""
    Write-Host "To stop the emulator, run:"
    Write-Host "  docker-compose down"
    Write-Host ""
    Write-Host "The emulator will be available at: http://localhost:8080"
    Write-Host "Emulator UI will be available at: http://localhost:4000"
}

# Run tests
Write-Step "Running tests to verify setup"
dotnet test EntityFramework.Firebase.sln --configuration Debug --verbosity normal --logger "console;verbosity=minimal"

if ($LASTEXITCODE -eq 0) {
    Write-Success "All tests passed"
} else {
    Write-Warning "Some tests failed, but this might be expected if external dependencies are not available"
}

Write-Success "Development environment setup completed!"
Write-Host ""
Write-Host "Next steps:"
Write-Host "1. Open the solution in your preferred IDE"
Write-Host "2. Start coding!"
Write-Host "3. Run tests with: dotnet test"
Write-Host "4. Build packages with: .\scripts\build-package.ps1 -Version 1.0.0-dev"

if ($SetupEmulator) {
    Write-Host "5. Start Firestore emulator with: docker-compose up -d firestore-emulator"
}
