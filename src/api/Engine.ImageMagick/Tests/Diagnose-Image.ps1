# Diagnostic script to test image upload issues

param(
    [Parameter(Mandatory=$true)]
    [string]$ApiEndpoint,
    
    [Parameter(Mandatory=$true)]
    [string]$ImagePath,
    
    [Parameter(Mandatory=$false)]
    [hashtable]$Headers = @{}
)

# Check if file exists
if (-not (Test-Path $ImagePath)) {
    Write-Error "File not found: $ImagePath"
    exit 1
}

Write-Host "=== IMAGE DIAGNOSTICS ===" -ForegroundColor Cyan

# 1. Basic file info
$fileInfo = Get-Item $ImagePath
Write-Host "File: $($fileInfo.Name)" -ForegroundColor Yellow
Write-Host "Size: $($fileInfo.Length) bytes" -ForegroundColor Yellow
Write-Host "Extension: $($fileInfo.Extension)" -ForegroundColor Yellow

# 2. Check if it's a valid image using .NET
try {
    Add-Type -AssemblyName System.Drawing
    $img = [System.Drawing.Image]::FromFile($ImagePath)
    Write-Host "Image Format: $($img.RawFormat.Guid)" -ForegroundColor Green
    Write-Host "Dimensions: $($img.Width) x $($img.Height)" -ForegroundColor Green
    Write-Host "Pixel Format: $($img.PixelFormat)" -ForegroundColor Green
    $img.Dispose()
    Write-Host "✓ Image is valid and readable by .NET" -ForegroundColor Green
} catch {
    Write-Host "✗ Image cannot be read by .NET: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "This might be the source of the problem." -ForegroundColor Red
}

# 3. Check file header (magic bytes)
$bytes = Get-Content $ImagePath -Encoding Byte -TotalCount 10
$header = ($bytes | ForEach-Object { $_.ToString("X2") }) -join " "
Write-Host "File header (hex): $header" -ForegroundColor Yellow

$headerCheck = switch -Regex ($header) {
    "^FF D8 FF" { "JPEG" }
    "^89 50 4E 47" { "PNG" }
    "^47 49 46 38" { "GIF" }
    "^42 4D" { "BMP" }
    "^52 49 46 46.*57 45 42 50" { "WebP" }
    default { "Unknown/Corrupted" }
}
Write-Host "Header indicates: $headerCheck format" -ForegroundColor Yellow

# 4. Try different upload approaches
Write-Host "`n=== TESTING UPLOAD APPROACHES ===" -ForegroundColor Cyan

# Test 1: Simple file upload without settings
Write-Host "`nTest 1: Simple file upload (no settings)" -ForegroundColor Yellow
try {
    Add-Type -AssemblyName System.Net.Http
    
    $httpClient = New-Object System.Net.Http.HttpClient
    $multipartContent = New-Object System.Net.Http.MultipartFormDataContent
    
    # Add custom headers
    foreach ($key in $Headers.Keys) {
        $httpClient.DefaultRequestHeaders.Add($key, $Headers[$key])
    }
    
    $imageBytes = [System.IO.File]::ReadAllBytes($ImagePath)
    $fileName = [System.IO.Path]::GetFileName($ImagePath)
    
    # Simple image upload only
    $byteArrayContent = New-Object System.Net.Http.ByteArrayContent(,$imageBytes)
    $byteArrayContent.Headers.ContentType = [System.Net.Http.Headers.MediaTypeHeaderValue]::Parse("image/jpeg")
    $multipartContent.Add($byteArrayContent, "Image", $fileName)
    
    $response = $httpClient.PostAsync($ApiEndpoint, $multipartContent).Result
    $responseContent = $response.Content.ReadAsStringAsync().Result
    
    if ($response.IsSuccessStatusCode) {
        Write-Host "✓ Simple upload successful" -ForegroundColor Green
    } else {
        Write-Host "✗ Simple upload failed: $($response.StatusCode)" -ForegroundColor Red
        Write-Host "Response: $responseContent" -ForegroundColor Red
    }
    
    $httpClient.Dispose()
    $multipartContent.Dispose()
    
} catch {
    Write-Host "✗ Simple upload exception: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Upload with explicit image/jpeg content type
Write-Host "`nTest 2: Upload with different content types" -ForegroundColor Yellow

$contentTypes = @("image/jpeg", "application/octet-stream", "image/png")

foreach ($contentType in $contentTypes) {
    try {
        $httpClient = New-Object System.Net.Http.HttpClient
        $multipartContent = New-Object System.Net.Http.MultipartFormDataContent
        
        foreach ($key in $Headers.Keys) {
            $httpClient.DefaultRequestHeaders.Add($key, $Headers[$key])
        }
        
        $imageBytes = [System.IO.File]::ReadAllBytes($ImagePath)
        $fileName = [System.IO.Path]::GetFileName($ImagePath)
        
        $byteArrayContent = New-Object System.Net.Http.ByteArrayContent(,$imageBytes)
        $byteArrayContent.Headers.ContentType = [System.Net.Http.Headers.MediaTypeHeaderValue]::Parse($contentType)
        $multipartContent.Add($byteArrayContent, "Image", $fileName)
        
        # Add minimal settings
        $outputTypeContent = New-Object System.Net.Http.StringContent("Jpeg")
        $multipartContent.Add($outputTypeContent, "Settings.OutputType")
        
        $response = $httpClient.PostAsync($ApiEndpoint, $multipartContent).Result
        $responseContent = $response.Content.ReadAsStringAsync().Result
        
        if ($response.IsSuccessStatusCode) {
            Write-Host "✓ Upload with $contentType successful" -ForegroundColor Green
        } else {
            Write-Host "✗ Upload with $contentType failed: $($response.StatusCode)" -ForegroundColor Red
            if ($responseContent.Length -lt 200) {
                Write-Host "Response: $responseContent" -ForegroundColor Red
            }
        }
        
        $httpClient.Dispose()
        $multipartContent.Dispose()
        
    } catch {
        Write-Host "✗ Upload with $contentType exception: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n=== RECOMMENDATIONS ===" -ForegroundColor Cyan

if ($headerCheck -eq "Unknown/Corrupted") {
    Write-Host "• The file appears to be corrupted or in an unsupported format" -ForegroundColor Yellow
    Write-Host "• Try with a different image file" -ForegroundColor Yellow
}

Write-Host "• Try converting the image to a standard JPEG format first" -ForegroundColor Yellow
Write-Host "• Check if the server's ImageMagick installation supports this image format" -ForegroundColor Yellow
Write-Host "• Test with a small, simple JPEG image (e.g., 100x100 pixels)" -ForegroundColor Yellow

Write-Host "`nScript completed." -ForegroundColor Cyan