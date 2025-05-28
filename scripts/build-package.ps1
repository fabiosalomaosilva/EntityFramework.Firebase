#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Script para criar e validar pacotes NuGet localmente

.DESCRIPTION
    Este script executa o processo completo de build, teste e criação de pacotes NuGet,
    simulando o que acontece no GitHub Actions.

.PARAMETER Version
    Versão do pacote a ser criado (ex: 1.0.0)

.PARAMETER Configuration
    Configuração de build (Debug ou Release). Padrão: Release

.PARAMETER SkipTests
    Pula a execução dos testes

.PARAMETER OutputPath
    Caminho onde os pacotes serão salvos. Padrão: ./artifacts/packages

.EXAMPLE
    .\build-package.ps1 -Version "1.0.0"

.EXAMPLE
    .\build-package.ps1 -Version "1.0.0-beta.1" -SkipTests
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    [Parameter(Mandatory = $false)]
    [switch]$SkipTests,

    [Parameter(Mandatory = $false)]
    [string]$OutputPath = "./artifacts/packages"
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

# Verify we're in the right directory
if (-not (Test-Path "EntityFramework.Firebase.sln")) {
    Write-Error "EntityFramework.Firebase.sln not found. Please run this script from the repository root."
    exit 1
}

Write-Step "Starting build process for version $Version"

# Create output directory
New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null
$OutputPath = Resolve-Path $OutputPath

Write-Step "Cleaning previous builds"
dotnet clean EntityFramework.Firebase.sln --configuration $Configuration

Write-Step "Restoring NuGet packages"
dotnet restore EntityFramework.Firebase.sln
if ($LASTEXITCODE -ne 0) {
    Write-Error "Package restore failed"
    exit $LASTEXITCODE
}

Write-Step "Building solution"
dotnet build EntityFramework.Firebase.sln --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit $LASTEXITCODE
}

if (-not $SkipTests) {
    Write-Step "Running tests"
    dotnet test EntityFramework.Firebase.sln --configuration $Configuration --no-build --verbosity normal
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed"
        exit $LASTEXITCODE
    }
} else {
    Write-Warning "Skipping tests"
}

Write-Step "Creating NuGet package"
dotnet pack src/EfCore.FirestoreProvider/EfCore.FirestoreProvider.csproj `
    --configuration $Configuration `
    --no-build `
    --output $OutputPath `
    /p:PackageVersion=$Version `
    /p:AssemblyVersion=$Version `
    /p:FileVersion=$Version `
    /p:InformationalVersion=$Version

if ($LASTEXITCODE -ne 0) {
    Write-Error "Package creation failed"
    exit $LASTEXITCODE
}

Write-Step "Validating package"
$packageFile = Get-ChildItem "$OutputPath\EfCore.FirestoreProvider.$Version.nupkg" -ErrorAction SilentlyContinue

if ($packageFile) {
    Write-Success "Package created successfully: $($packageFile.FullName)"
    Write-Host ""
    Write-Host "Package Information:"
    Write-Host "  Name: $($packageFile.Name)"
    Write-Host "  Size: $([math]::Round($packageFile.Length / 1KB, 2)) KB"
    Write-Host "  Path: $($packageFile.FullName)"

    # Display package contents
    Write-Step "Package contents:"
    $tempDir = New-TemporaryFile | ForEach-Object { Remove-Item $_; New-Item -ItemType Directory -Path $_ }
    try {
        Expand-Archive -Path $packageFile.FullName -DestinationPath $tempDir.FullName
        Get-ChildItem $tempDir.FullName -Recurse | ForEach-Object {
            $relativePath = $_.FullName.Substring($tempDir.FullName.Length + 1)
            if ($_.PSIsContainer) {
                Write-Host "  📁 $relativePath" -ForegroundColor Yellow
            } else {
                Write-Host "  📄 $relativePath" -ForegroundColor Gray
            }
        }
    } finally {
        Remove-Item $tempDir.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
} else {
    Write-Error "Package file not found after creation"
    exit 1
}

Write-Success "Build completed successfully!"
Write-Host ""
Write-Host "Next steps:"
Write-Host "1. Test the package locally by installing it in a test project"
Write-Host "2. If everything looks good, create a git tag and push to trigger the release workflow:"
Write-Host "   git tag v$Version"
Write-Host "   git push origin v$Version"
