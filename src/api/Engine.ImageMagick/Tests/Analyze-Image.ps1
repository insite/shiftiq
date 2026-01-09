# Analyze the response content to see what's actually being returned

param(
    [Parameter(Mandatory=$true)]
    [string]$ResponseFilePath
)

if (-not (Test-Path $ResponseFilePath)) {
    Write-Error "File not found: $ResponseFilePath"
    exit 1
}

Write-Host "=== RESPONSE FILE ANALYSIS ===" -ForegroundColor Cyan

# Get basic file info
$fileInfo = Get-Item $ResponseFilePath
Write-Host "File: $($fileInfo.Name)" -ForegroundColor Yellow
Write-Host "Size: $($fileInfo.Length) bytes" -ForegroundColor Yellow

# Read the first 100 bytes to analyze content
$bytes = [System.IO.File]::ReadAllBytes($ResponseFilePath) | Select-Object -First 100
$header = ($bytes | Select-Object -First 20 | ForEach-Object { $_.ToString("X2") }) -join " "
Write-Host "File header (first 20 bytes): $header" -ForegroundColor Yellow

# Check if it looks like a known image format
$headerCheck = switch -Regex ($header) {
    "^FF D8 FF" { "JPEG" }
    "^89 50 4E 47" { "PNG" }
    "^47 49 46 38" { "GIF" }
    "^42 4D" { "BMP" }
    "^52 49 46 46.*57 45 42 50" { "WebP" }
    default { "Unknown binary format" }
}
Write-Host "Header indicates: $headerCheck format" -ForegroundColor $(if ($headerCheck -ne "Unknown binary format") { "Green" } else { "Red" })

# Check if it's text content (like JSON error or HTML)
$firstBytes = [System.Text.Encoding]::UTF8.GetString($bytes[0..99])
if ($firstBytes -match '^[\x20-\x7E\s]+$') {  # Printable ASCII characters
    Write-Host "`nContent appears to be TEXT:" -ForegroundColor Yellow
    Write-Host $firstBytes -ForegroundColor White
    
    # Try to read entire file as text
    $fullText = [System.IO.File]::ReadAllText($ResponseFilePath)
    Write-Host "`nFull response content:" -ForegroundColor Yellow
    Write-Host $fullText -ForegroundColor White
    
    # Check if it's JSON
    try {
        $json = $fullText | ConvertFrom-Json
        Write-Host "`nParsed as JSON:" -ForegroundColor Green
        $json | ConvertTo-Json -Depth 5
    } catch {
        Write-Host "`nNot valid JSON" -ForegroundColor Red
    }
    
} else {
    Write-Host "`nContent appears to be BINARY data" -ForegroundColor Yellow
    
    if ($headerCheck -eq "Unknown binary format") {
        Write-Host "This doesn't appear to be a standard image format" -ForegroundColor Red
        
        # Show more of the header
        $extendedHeader = ($bytes | ForEach-Object { $_.ToString("X2") }) -join " "
        Write-Host "Extended header (first 100 bytes):" -ForegroundColor Yellow
        Write-Host $extendedHeader -ForegroundColor White
    }
}

# Try to detect if it's a valid image using .NET
Write-Host "`n=== IMAGE VALIDATION TEST ===" -ForegroundColor Cyan
try {
    Add-Type -AssemblyName System.Drawing
    $image = [System.Drawing.Image]::FromFile($ResponseFilePath)
    Write-Host "✓ Valid image detected!" -ForegroundColor Green
    Write-Host "Format: $($image.RawFormat.Guid)" -ForegroundColor Green
    Write-Host "Dimensions: $($image.Width) x $($image.Height)" -ForegroundColor Green
    Write-Host "Pixel Format: $($image.PixelFormat)" -ForegroundColor Green
    $image.Dispose()
} catch {
    Write-Host "✗ Not a valid image file: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== RECOMMENDATIONS ===" -ForegroundColor Cyan

if ($headerCheck -eq "Unknown binary format" -and $firstBytes -match '^[\x20-\x7E\s]+$') {
    Write-Host "• The server returned text content, likely an error message" -ForegroundColor Yellow
    Write-Host "• Check the full response content above for error details" -ForegroundColor Yellow
} elseif ($headerCheck -eq "Unknown binary format") {
    Write-Host "• The server returned unknown binary data" -ForegroundColor Yellow
    Write-Host "• This might be a proprietary format or corrupted data" -ForegroundColor Yellow
    Write-Host "• Check server logs for more details" -ForegroundColor Yellow
} elseif ($headerCheck -ne "JPEG" -and $headerCheck -ne "Unknown binary format") {
    Write-Host "• The server returned a $headerCheck image instead of JPEG" -ForegroundColor Yellow
    Write-Host "• Try renaming the file with the correct extension" -ForegroundColor Yellow
} else {
    Write-Host "• File appears to be valid - try opening with different image viewers" -ForegroundColor Yellow
}

Write-Host "`nAnalysis completed." -ForegroundColor Cyan