# PowerShell script to POST to AdjustImage endpoint with multipart form data

param(
    [Parameter(Mandatory=$true)]
    [string]$ApiEndpoint,
    
    [Parameter(Mandatory=$true)]
    [string]$ImagePath,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputType = "Jpeg",  # ImageType enum value
    
    [Parameter(Mandatory=$false)]
    [bool]$Crop = $false,
    
    [Parameter(Mandatory=$false)]
    [int]$MaxWidth = 800,
    
    [Parameter(Mandatory=$false)]
    [int]$MaxHeight = 600,
    
    [Parameter(Mandatory=$false)]
    [hashtable]$Headers = @{}
)

# Check if file exists
if (-not (Test-Path $ImagePath)) {
    Write-Error "File not found: $ImagePath"
    exit 1
}

Add-Type -AssemblyName System.Net.Http

try {
    Write-Host "Preparing to upload $(Split-Path $ImagePath -Leaf) to $ApiEndpoint..." -ForegroundColor Cyan
    Write-Host "Settings: OutputType=$OutputType, Crop=$Crop, MaxWidth=$MaxWidth, MaxHeight=$MaxHeight" -ForegroundColor Yellow
    
    # Create HttpClient and MultipartFormDataContent
    $httpClient = New-Object System.Net.Http.HttpClient
    $multipartContent = New-Object System.Net.Http.MultipartFormDataContent
    
    # Add custom headers to HttpClient
    foreach ($key in $Headers.Keys) {
        $httpClient.DefaultRequestHeaders.Add($key, $Headers[$key])
    }
    
    # Read image file and create file content
    $imageBytes = [System.IO.File]::ReadAllBytes($ImagePath)
    $fileName = [System.IO.Path]::GetFileName($ImagePath)
    $fileExtension = [System.IO.Path]::GetExtension($ImagePath).ToLower()
    
    # Determine content type
    $contentType = switch ($fileExtension) {
        ".jpg"  { "image/jpeg" }
        ".jpeg" { "image/jpeg" }
        ".png"  { "image/png" }
        ".gif"  { "image/gif" }
        ".bmp"  { "image/bmp" }
        ".webp" { "image/webp" }
        default { "application/octet-stream" }
    }
    
    # Add the image file (maps to IFormFile Image property)
    # Use comma operator to prevent array expansion
    $byteArrayContent = New-Object System.Net.Http.ByteArrayContent(,$imageBytes)
    $byteArrayContent.Headers.ContentType = [System.Net.Http.Headers.MediaTypeHeaderValue]::Parse($contentType)
    $multipartContent.Add($byteArrayContent, "Image", $fileName)
    
    # Add Settings properties as string content
    $outputTypeContent = New-Object System.Net.Http.StringContent($OutputType)
    $multipartContent.Add($outputTypeContent, "Settings.OutputType")
    
    $cropContent = New-Object System.Net.Http.StringContent($Crop.ToString().ToLower())
    $multipartContent.Add($cropContent, "Settings.Crop")
    
    $maxWidthContent = New-Object System.Net.Http.StringContent($MaxWidth.ToString())
    $multipartContent.Add($maxWidthContent, "Settings.MaxWidth")
    
    $maxHeightContent = New-Object System.Net.Http.StringContent($MaxHeight.ToString())
    $multipartContent.Add($maxHeightContent, "Settings.MaxHeight")
    
    # Send the request
    Write-Host "Sending request..." -ForegroundColor Cyan
    $response = $httpClient.PostAsync($ApiEndpoint, $multipartContent).Result
    
    # Read response
    $responseContent = $response.Content.ReadAsByteArrayAsync().Result
    $responseContentType = if ($response.Content.Headers.ContentType) { 
        $response.Content.Headers.ContentType.MediaType 
    } else { 
        "unknown" 
    }
    
    if ($response.IsSuccessStatusCode) {
        Write-Host "Request successful! (Status: $($response.StatusCode))" -ForegroundColor Green
        
        # Handle different response types
        if ($responseContentType -like "image/*") {
            Write-Host "Received image response (Content-Type: $responseContentType, Size: $($responseContent.Length) bytes)" -ForegroundColor Yellow
            
            # Save the processed image
            $outputFileName = "adjusted_" + $fileName
            [System.IO.File]::WriteAllBytes($outputFileName, $responseContent)
            Write-Host "Saved processed image to: $outputFileName" -ForegroundColor Green
            
        } elseif ($responseContentType -like "application/json" -or $responseContentType -like "text/*") {
            $responseText = [System.Text.Encoding]::UTF8.GetString($responseContent)
            Write-Host "Response:" -ForegroundColor Yellow
            
            if ($responseContentType -like "application/json") {
                try {
                    $jsonResponse = $responseText | ConvertFrom-Json
                    $jsonResponse | ConvertTo-Json -Depth 5
                } catch {
                    Write-Host $responseText
                }
            } else {
                Write-Host $responseText
            }
        } else {
            # Handle binary response (likely processed image)
            Write-Host "Received binary response (Content-Type: $responseContentType, Size: $($responseContent.Length) bytes)" -ForegroundColor Yellow
            
            # Determine file extension based on settings or content type
            $outputExtension = switch ($OutputType.ToLower()) {
                "jpeg" { ".jpg" }
                "jpg" { ".jpg" }
                "png" { ".png" }
                "gif" { ".gif" }
                "bmp" { ".bmp" }
                "webp" { ".webp" }
                default { ".jpg" }
            }
            
            # Create output filename in the same folder as input file
            $inputDirectory = [System.IO.Path]::GetDirectoryName($ImagePath)
            $baseName = [System.IO.Path]::GetFileNameWithoutExtension($ImagePath)
            $outputFileName = Join-Path $inputDirectory ("processed_" + $baseName + $outputExtension)
            
            # Save the binary response
            [System.IO.File]::WriteAllBytes($outputFileName, $responseContent)
            Write-Host "Saved processed image to: $outputFileName" -ForegroundColor Green
            
            # Display file info
            $outputFileInfo = Get-Item $outputFileName
            Write-Host "Output file size: $($outputFileInfo.Length) bytes" -ForegroundColor Cyan
        }
        
    } else {
        $errorContent = [System.Text.Encoding]::UTF8.GetString($responseContent)
        Write-Error "HTTP Error: $($response.StatusCode) - $($response.ReasonPhrase)"
        Write-Host "Response Body: $errorContent" -ForegroundColor Red
        exit 1
    }
    
} catch [System.AggregateException] {
    # Handle HttpClient async exceptions
    $innerException = $_.Exception.InnerException
    if ($innerException -is [System.Net.Http.HttpRequestException]) {
        Write-Error "HTTP Request failed: $($innerException.Message)"
    } else {
        Write-Error "Request failed: $($innerException.Message)"
    }
    exit 1
} catch {
    Write-Error "Request failed: $($_.Exception.Message)"
    if ($_.ErrorDetails) {
        Write-Host "Error Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
    exit 1
} finally {
    # Clean up resources
    if ($multipartContent) { $multipartContent.Dispose() }
    if ($httpClient) { $httpClient.Dispose() }
}

Write-Host "Script completed." -ForegroundColor Cyan