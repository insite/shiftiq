# Simple script to test with a small image first

param(
    [Parameter(Mandatory=$true)]
    [string]$ApiEndpoint,
    
    [Parameter(Mandatory=$false)]
    [hashtable]$Headers = @{}
)

Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Net.Http

# Method 1: Create a simple test image and save it properly
try {
    Write-Host "Creating a small test image..." -ForegroundColor Cyan
    
    # Create bitmap
    $width = 400
    $height = 300
    $bitmap = New-Object System.Drawing.Bitmap($width, $height, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    
    # Set high quality
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    
    # Draw a simple pattern
    $graphics.FillRectangle([System.Drawing.Brushes]::LightBlue, 0, 0, $width, $height)
    $graphics.FillEllipse([System.Drawing.Brushes]::Red, 50, 50, 100, 100)
    $graphics.FillRectangle([System.Drawing.Brushes]::Green, 200, 100, 150, 80)
    
    # Add text
    $font = New-Object System.Drawing.Font("Arial", 16, [System.Drawing.FontStyle]::Bold)
    $graphics.DrawString("TEST IMAGE", $font, [System.Drawing.Brushes]::Black, 150, 200)
    
    # Save to memory stream first (avoids file system issues)
    $memoryStream = New-Object System.IO.MemoryStream
    
    # Get JPEG encoder with quality setting
    $jpegCodec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object { $_.MimeType -eq "image/jpeg" }
    $encoderParams = New-Object System.Drawing.Imaging.EncoderParameters(1)
    $qualityParam = New-Object System.Drawing.Imaging.EncoderParameter([System.Drawing.Imaging.Encoder]::Quality, 90L)
    $encoderParams.Param[0] = $qualityParam
    
    # Save to memory stream
    $bitmap.Save($memoryStream, $jpegCodec, $encoderParams)
    $imageBytes = $memoryStream.ToArray()
    
    Write-Host "Created test image: $($imageBytes.Length) bytes" -ForegroundColor Green
    
    # Clean up drawing objects
    $graphics.Dispose()
    $bitmap.Dispose()
    $font.Dispose()
    $memoryStream.Dispose()
    
    # Also save to file for verification
    [System.IO.File]::WriteAllBytes("C:\base\repo\insite\code\test\imagemagick\test_image.jpg", $imageBytes)
    Write-Host "Saved test image to: test_image.jpg" -ForegroundColor Yellow
    
    # Now try to upload this test image
    Write-Host "`nTesting upload with small image..." -ForegroundColor Cyan
    
    $httpClient = New-Object System.Net.Http.HttpClient
    $multipartContent = New-Object System.Net.Http.MultipartFormDataContent
    
    # Add custom headers
    foreach ($key in $Headers.Keys) {
        $httpClient.DefaultRequestHeaders.Add($key, $Headers[$key])
    }
    
    # Add the test image
    $byteArrayContent = New-Object System.Net.Http.ByteArrayContent(,$imageBytes)
    $byteArrayContent.Headers.ContentType = [System.Net.Http.Headers.MediaTypeHeaderValue]::Parse("image/jpeg")
    $multipartContent.Add($byteArrayContent, "Image", "C:\base\repo\insite\code\test\imagemagick\test_image.jpg")
    
    # Add minimal settings
    $outputTypeContent = New-Object System.Net.Http.StringContent("Jpeg")
    $multipartContent.Add($outputTypeContent, "Settings.OutputType")
    
    $cropContent = New-Object System.Net.Http.StringContent("false")
    $multipartContent.Add($cropContent, "Settings.Crop")
    
    $maxWidthContent = New-Object System.Net.Http.StringContent("800")
    $multipartContent.Add($maxWidthContent, "Settings.MaxWidth")
    
    $maxHeightContent = New-Object System.Net.Http.StringContent("600")
    $multipartContent.Add($maxHeightContent, "Settings.MaxHeight")
    
    # Send request
    Write-Host "Sending request to: $ApiEndpoint" -ForegroundColor Cyan
    $response = $httpClient.PostAsync($ApiEndpoint, $multipartContent).Result
    
    # Handle response
    $responseContent = $response.Content.ReadAsByteArrayAsync().Result
    $responseContentType = if ($response.Content.Headers.ContentType) { 
        $response.Content.Headers.ContentType.MediaType 
    } else { 
        "unknown" 
    }
    
    if ($response.IsSuccessStatusCode) {
        Write-Host "✓ TEST UPLOAD SUCCESSFUL! (Status: $($response.StatusCode))" -ForegroundColor Green
        
        if ($responseContentType -like "image/*") {
            [System.IO.File]::WriteAllBytes("C:\base\repo\insite\code\test\imagemagick\processed_test_image.jpg", $responseContent)
            Write-Host "Saved processed image to: processed_test_image.jpg" -ForegroundColor Green
        } else {
            $responseText = [System.Text.Encoding]::UTF8.GetString($responseContent)
            Write-Host "Response: $responseText" -ForegroundColor Yellow
        }
        
        Write-Host "`n=== SUCCESS! Your API endpoint works fine ===" -ForegroundColor Green
        Write-Host "The issue is with your large destiny.jpg file (6803x3900 pixels)" -ForegroundColor Yellow
        Write-Host "Solutions:" -ForegroundColor Yellow
        Write-Host "1. Resize the image before uploading (use external tool like GIMP, Photoshop)" -ForegroundColor Yellow
        Write-Host "2. Ask server admin to increase ImageMagick resource limits" -ForegroundColor Yellow
        Write-Host "3. Try a different image processing approach on server" -ForegroundColor Yellow
        
    } else {
        $errorContent = [System.Text.Encoding]::UTF8.GetString($responseContent)
        Write-Host "✗ TEST UPLOAD FAILED: $($response.StatusCode) - $($response.ReasonPhrase)" -ForegroundColor Red
        Write-Host "Response: $errorContent" -ForegroundColor Red
        
        if ($errorContent -like "*NoDecodeDelegateForThisImageFormat*") {
            Write-Host "`n=== Server Configuration Issue ===" -ForegroundColor Red
            Write-Host "The server's ImageMagick installation has problems." -ForegroundColor Yellow
            Write-Host "Even a simple test image fails - this is a server-side issue." -ForegroundColor Yellow
        }
    }
    
} catch {
    Write-Error "Error creating/uploading test image: $($_.Exception.Message)"
    Write-Host "Full exception: $($_.Exception)" -ForegroundColor Red
} finally {
    if ($multipartContent) { $multipartContent.Dispose() }
    if ($httpClient) { $httpClient.Dispose() }
}

Write-Host "`nIf test image works, try with a resized version of destiny.jpg" -ForegroundColor Cyan
Write-Host "You can resize destiny.jpg using:" -ForegroundColor Cyan
Write-Host "- Online tools like https://www.iloveimg.com/resize-image" -ForegroundColor Yellow
Write-Host "- Desktop apps like GIMP (free) or Photoshop" -ForegroundColor Yellow
Write-Host "- Command line: magick destiny.jpg -resize 2048x2048> smaller_destiny.jpg" -ForegroundColor Yellow