# POST a JPG to get image file information

param(
    [Parameter(Mandatory=$true)]
    [string]$ApiEndpoint,
    
    [Parameter(Mandatory=$true)]
    [string]$FilePath,
    
    [Parameter(Mandatory=$false)]
    [hashtable]$Headers = @{}
)

# Check if file exists and is a JPG
if (-not (Test-Path $FilePath)) {
    Write-Error "File not found: $FilePath"
    exit 1
}

$fileExtension = [System.IO.Path]::GetExtension($FilePath).ToLower()
if ($fileExtension -ne ".jpg" -and $fileExtension -ne ".jpeg") {
    Write-Warning "File extension is $fileExtension, not .jpg or .jpeg"
}

try {
    # Read the file as bytes
    $fileBytes = [System.IO.File]::ReadAllBytes($FilePath)
    $fileName = [System.IO.Path]::GetFileName($FilePath)

    $uploadHeaders = @{
        'Content-Type' = 'image/jpeg'
        'Content-Length' = $fileBytes.Length
    }
    
    # Add any additional headers
    foreach ($key in $Headers.Keys) {
        $uploadHeaders[$key] = $Headers[$key]
    }
    
    $response = Invoke-RestMethod -Uri $ApiEndpoint -Method Post -Body $fileBytes -Headers $uploadHeaders
    
    Write-Host "Upload successful!" -ForegroundColor Green
    Write-Host "Response:" -ForegroundColor Yellow
    $response | ConvertTo-Json -Depth 3
    
} catch {
    Write-Error "Upload failed: $($_.Exception.Message)"
}