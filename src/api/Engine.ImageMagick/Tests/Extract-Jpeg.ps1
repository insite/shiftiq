# Extract JPEG from ImageMagick MIFF format

param(
    [Parameter(Mandatory=$true)]
    [string]$MiffFilePath,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath = $null
)

if (-not (Test-Path $MiffFilePath)) {
    Write-Error "File not found: $MiffFilePath"
    exit 1
}

Write-Host "=== MIFF TO JPEG EXTRACTOR ===" -ForegroundColor Cyan

try {
    # Read the entire MIFF file
    $miffBytes = [System.IO.File]::ReadAllBytes($MiffFilePath)
    Write-Host "MIFF file size: $($miffBytes.Length) bytes" -ForegroundColor Yellow
    
    # Look for JPEG header (FF D8 FF)
    $jpegStart = -1
    for ($i = 0; $i -lt ($miffBytes.Length - 2); $i++) {
        if ($miffBytes[$i] -eq 0xFF -and $miffBytes[$i + 1] -eq 0xD8 -and $miffBytes[$i + 2] -eq 0xFF) {
            $jpegStart = $i
            Write-Host "Found JPEG header at offset: $jpegStart" -ForegroundColor Green
            break
        }
    }
    
    if ($jpegStart -eq -1) {
        Write-Error "No JPEG header found in MIFF file"
        exit 1
    }
    
    # Look for JPEG end marker (FF D9)
    $jpegEnd = -1
    for ($i = $jpegStart + 2; $i -lt ($miffBytes.Length - 1); $i++) {
        if ($miffBytes[$i] -eq 0xFF -and $miffBytes[$i + 1] -eq 0xD9) {
            $jpegEnd = $i + 1  # Include the end marker
            Write-Host "Found JPEG end marker at offset: $jpegEnd" -ForegroundColor Green
            break
        }
    }
    
    if ($jpegEnd -eq -1) {
        # If we can't find the end marker, assume the rest of the file is JPEG
        $jpegEnd = $miffBytes.Length - 1
        Write-Host "JPEG end marker not found, using end of file" -ForegroundColor Yellow
    }
    
    # Extract the JPEG data
    $jpegLength = $jpegEnd - $jpegStart + 1
    $jpegBytes = New-Object byte[] $jpegLength
    [Array]::Copy($miffBytes, $jpegStart, $jpegBytes, 0, $jpegLength)
    
    Write-Host "Extracted JPEG size: $($jpegBytes.Length) bytes" -ForegroundColor Green
    
    # Determine output filename
    if (-not $OutputPath) {
        $directory = [System.IO.Path]::GetDirectoryName($MiffFilePath)
        $baseName = [System.IO.Path]::GetFileNameWithoutExtension($MiffFilePath)
        $OutputPath = Join-Path $directory ($baseName + "_extracted.jpg")
    }
    
    # Save the extracted JPEG
    [System.IO.File]::WriteAllBytes($OutputPath, $jpegBytes)
    Write-Host "Saved extracted JPEG to: $OutputPath" -ForegroundColor Green
    
    # Verify the extracted JPEG
    try {
        Add-Type -AssemblyName System.Drawing -ErrorAction SilentlyContinue
        $image = [System.Drawing.Image]::FromFile($OutputPath)
        Write-Host "✓ Extracted JPEG is valid!" -ForegroundColor Green
        Write-Host "Dimensions: $($image.Width) x $($image.Height)" -ForegroundColor Green
        Write-Host "Format: $($image.RawFormat)" -ForegroundColor Green
        $image.Dispose()
        
        # Show file size comparison
        $extractedSize = (Get-Item $OutputPath).Length
        Write-Host "`nSize comparison:" -ForegroundColor Cyan
        Write-Host "Original MIFF: $($miffBytes.Length) bytes" -ForegroundColor Yellow
        Write-Host "Extracted JPEG: $extractedSize bytes" -ForegroundColor Yellow
        Write-Host "MIFF overhead: $(($miffBytes.Length - $extractedSize)) bytes" -ForegroundColor Yellow
        
    } catch {
        Write-Error "Extracted file is not a valid JPEG: $($_.Exception.Message)"
    }
    
    # Show MIFF metadata (the text part before JPEG)
    Write-Host "`n=== MIFF METADATA ===" -ForegroundColor Cyan
    $metadataBytes = $miffBytes[0..($jpegStart - 1)]
    $metadataText = [System.Text.Encoding]::ASCII.GetString($metadataBytes) -replace '[^\x20-\x7E\r\n]', '?'
    Write-Host $metadataText -ForegroundColor Gray
    
} catch {
    Write-Error "Error extracting JPEG: $($_.Exception.Message)"
    exit 1
}

Write-Host "`nExtraction completed successfully!" -ForegroundColor Green