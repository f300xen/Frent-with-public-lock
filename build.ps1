# Stop the script immediately if any command returns an error
$ErrorActionPreference = "Stop"

# Define which configurations to build
$configs = @("Debug", "Release")

foreach ($config in $configs) {
    Write-Host "`n==========================================" -ForegroundColor Cyan
    Write-Host " STARTING BUILD: $config" -ForegroundColor Cyan
    Write-Host "=========================================="

    # 1. Clean previous build for this config
    $targetBinDir = ".\Frent\bin\$config"
    
    Write-Host "Cleaning $targetBinDir..." -ForegroundColor Gray
    if (Test-Path $targetBinDir) {
        Remove-Item -Path $targetBinDir -Recurse -Force
    }
    # Create the directory immediately so we can copy the generator into it
    New-Item -ItemType Directory -Force -Path $targetBinDir | Out-Null

    # 2. Build the Source Generator
    Write-Host "Building Generator..." -ForegroundColor Yellow
    dotnet build ".\Frent.Generator\Frent.Generator.csproj" -c $config

    # 3. Copy Generator DLL
    # The original script copied this to the main project bin folder manually.
    # We replicate this ensuring we grab the netstandard2.0 output.
    $genSourcePath = ".\Frent.Generator\bin\$config\netstandard2.0\Frent.Generator.dll"
    
    if (Test-Path $genSourcePath) {
        Write-Host "Copying Generator to Frent output..." -ForegroundColor Gray
        Copy-Item -Path $genSourcePath -Destination $targetBinDir
    } else {
        Write-Warning "Could not find Generator DLL at $genSourcePath"
    }

    # 4. Build Frent Core
    Write-Host "Building Frent..." -ForegroundColor Yellow
    # We don't use /p:Publish=true here as that is usually for NuGet packing.
    # Standard build is sufficient for local references.
    dotnet build ".\Frent\Frent.csproj" -c $config

    Write-Host "Success: $config build complete." -ForegroundColor Green
}

Write-Host "`nAll builds finished." -ForegroundColor Green
# Optional: Open the folder to show results
# Invoke-Item ".\Frent\bin\"