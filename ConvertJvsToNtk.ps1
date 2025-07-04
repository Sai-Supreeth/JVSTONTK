# Simple Java to C# converter script

# Create directories if they don't exist
if (-not (Test-Path "output")) { New-Item -ItemType Directory -Path "output" | Out-Null }
if (-not (Test-Path "converted")) { New-Item -ItemType Directory -Path "converted" | Out-Null }

# Find the converter
$converter = ".\out\JvsToNtkConverter.exe"
if (-not (Test-Path $converter)) {
    Write-Host "Converter not found. Please build the project first." -ForegroundColor Red
    exit
}

# Get all Java files
$files = Get-ChildItem -Path "input" -Filter "*.java"
if ($files.Count -eq 0) {
    Write-Host "No Java files found in input folder." -ForegroundColor Yellow
    exit
}

# Convert each file
foreach ($file in $files) {
    $output = "output\$($file.BaseName).cs"
    Write-Host "Converting: $($file.Name) -> $($file.BaseName).cs" -ForegroundColor Cyan
    
    & $converter $file.FullName $output
    
    if ($LASTEXITCODE -eq 0) {
        Move-Item $file.FullName "converted\$($file.Name)"
        Write-Host "  Success!" -ForegroundColor Green
    } else {
        Write-Host "  Failed!" -ForegroundColor Red
    }
}

Write-Host "Conversion complete!" -ForegroundColor Green
