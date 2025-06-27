# PowerShell script to convert all JVS Java files in 'input' to C# (.cs) files in 'output' using JvsToNtkConverter.exe

param(
    [string]$InputDir = "input",
    [string]$OutputDir = "output"
)

# Ensure output directory exists
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
    Write-Host "Created output directory: $OutputDir" -ForegroundColor Green
}

# Find the converter executable
$exeCandidates = @(
    "JvsToNtkConverter.exe",
    ".\out\JvsToNtkConverter.exe",
    ".\JvsToNtkConverter\out\JvsToNtkConverter.exe",
    ".\bin\Release\net6.0\win-x64\publish\JvsToNtkConverter.exe",
    ".\bin\Release\net6.0\win-x64\JvsToNtkConverter.exe",
    ".\bin\Release\net6.0\publish\JvsToNtkConverter.exe",
    ".\bin\Release\net6.0\JvsToNtkConverter.exe"
)
$ConverterExe = $null
foreach ($candidate in $exeCandidates) {
    if (Test-Path $candidate) {
        $ConverterExe = $candidate
        break
    }
}

if (-not $ConverterExe) {
    Write-Host "Error: JvsToNtkConverter.exe not found in any of the following locations:" -ForegroundColor Red
    $exeCandidates | ForEach-Object { Write-Host "  $_" -ForegroundColor Yellow }
    Write-Host "Please build or publish the project first." -ForegroundColor Red
    Write-Host "Run: dotnet build -o out" -ForegroundColor Yellow
    return
}

# Convert all .java files in the input directory
$inputFiles = Get-ChildItem -Path $InputDir -Filter *.java -File
if ($inputFiles.Count -eq 0) {
    Write-Host "No .java files found in $InputDir." -ForegroundColor Yellow
    return
}

foreach ($file in $inputFiles) {
    $outputFile = Join-Path $OutputDir ($file.BaseName + ".cs")
    Write-Host "[Line-by-line] Converting: $($file.Name) -> $([System.IO.Path]::GetFileName($outputFile))" -ForegroundColor Cyan
    & $ConverterExe $file.FullName $outputFile
}

Write-Host "Conversion complete. Output .cs files are in: $OutputDir" -ForegroundColor Green 