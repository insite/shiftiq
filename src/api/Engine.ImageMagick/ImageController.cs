using System.Text;

using Microsoft.AspNetCore.Mvc;

using NetVips;
using NetVips.Extensions;

using Shift.Common;
using Shift.Common.Integration.ImageMagick;
using Shift.Constant;

namespace Engine.ImageMagick;

using SysDrawBitmap = System.Drawing.Bitmap;
using SysDrawImageFormat = System.Drawing.Imaging.ImageFormat;

[ApiController]
public class ImageController(IMonitor monitor) : ControllerBase
{
    private readonly IMonitor _monitor = monitor;

    [HttpPost(Endpoints.GetImageInfo)]
    [ProducesResponseType<ImageInfo>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [EndpointName("imageInfo")]
    public async Task<ActionResult<ImageInfo>> GetImageInfoAsync()
    {
        try
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                data = ms.ToArray();
            }

            if (data.IsEmpty())
                return BadRequest("Image is undefined.");

            using (var image = TryLoadImage(data))
            {
                return Ok(new ImageInfo
                {
                    Height = (uint)image.Height,
                    Width = (uint)image.Width,
                    Format = ImageFormat.Known,
                    ImageType = GetImageType(image),
                    ColorSpace = GetColorSpace(image),
                    PixelsPerInch = image.Xres * 25.4
                });
            }
        }
        catch (ApplicationError apperr)
        {
            if (apperr.Message == "Unknown format")
                return Ok(new ImageInfo
                {
                    Height = 0,
                    Width = 0,
                    Format = ImageFormat.Unknown,
                    ImageType = ImageType.Null,
                    ColorSpace = ColorSpace.Undefined,
                    PixelsPerInch = 0,
                });

            return BadRequest(apperr);
        }
        catch (Exception ex)
        {
            _monitor.Error(ex.Message);

            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost(Endpoints.AdjustImage)]
    [ProducesResponseType<byte[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [EndpointName("adjustImage")]
    public IActionResult AdjustImage([FromForm] AdjustImageModel model)
    {
        try
        {
            if (model.Image == null || model.Image.Length == 0)
                return BadRequest("Image is undefined.");

            if (model.Settings == null || model.Settings.MaxWidth <= 0 || model.Settings.MaxHeight <= 0 || !Enum.IsDefined(model.Settings.OutputType))
                return BadRequest("Settings is undefined.");

            byte[] imageData, resultData;
            var messages = new List<string>();

            using (var inputStream = model.Image.OpenReadStream())
            {
                using (var outputStream = new MemoryStream())
                {
                    AdjustImage(inputStream, outputStream, model.Settings, messages);

                    imageData = outputStream.ToArray();
                }
            }

            using (var fileStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(fileStream, Encoding.UTF8, true))
                {
                    writer.Write("MAGICK");

                    writer.Write(messages.Count);

                    foreach (var message in messages)
                        writer.Write(message.EmptyIfNull());

                    writer.Write(imageData.Length);
                    writer.Write(imageData);
                }

                resultData = fileStream.ToArray();
            }

            return File(resultData, "application/octet-stream");
        }
        catch (ApplicationError apperr)
        {
            return BadRequest(apperr);
        }
        catch (Exception ex)
        {
            _monitor.Error(ex.Message);

            return StatusCode(500, ex.Message);
        }
    }

    #region Methods

    private static Image TryLoadImage(byte[] data, VOption? options = null)
    {
        if (!IsBmp(data))
        {
            try
            {
                return Image.NewFromBuffer(data, kwargs: options);
            }
            catch (VipsException vipex)
            {
                if (!vipex.Message.StartsWith("unable to load from buffer\r\nVipsForeignLoad: buffer is not in a known format"))
                    throw;
            }
        }

        if (OperatingSystem.IsWindows())
            return TryLoadWithSystemDrawing(data);

        throw ApplicationError.Create("Unknown format");
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static Image TryLoadWithSystemDrawing(byte[] data)
    {
        try
        {
            using (var ms = new MemoryStream(data))
            {
                using (var bitmap = new SysDrawBitmap(ms))
                {
                    var image = bitmap.ToVips();

                    return image.Mutate(mutable =>
                    {
                        var formatGuid = bitmap.RawFormat.Guid;
                        var formatName = "Unknown";

                        if (formatGuid == SysDrawImageFormat.Jpeg.Guid)
                            formatName = "JPEG";
                        else if (formatGuid == SysDrawImageFormat.Png.Guid)
                            formatName = "PNG";
                        else if (formatGuid == SysDrawImageFormat.Gif.Guid)
                            formatName = "GIF";
                        else if (formatGuid == SysDrawImageFormat.Bmp.Guid)
                            formatName = "BMP";
                        else if (formatGuid == SysDrawImageFormat.Tiff.Guid)
                            formatName = "TIFF";
                        else if (formatGuid == SysDrawImageFormat.Icon.Guid)
                            formatName = "ICO";
                        else if (formatGuid == SysDrawImageFormat.Wmf.Guid)
                            formatName = "WMF";
                        else if (formatGuid == SysDrawImageFormat.Emf.Guid)
                            formatName = "EMF";

                        mutable.Set(GValue.GStrType, "sysdraw-format", formatName);
                    });
                }
            }
        }
        catch (ArgumentException argex)
        {
            if (argex.Message == "Parameter is not valid.")
                throw ApplicationError.Create("Unknown format");

            throw;
        }
    }

    private static void AdjustImage(Stream inputStream, Stream outputStream, AdjustImageSettings settings, List<string> messages)
    {
        byte[] inputData;
        using (var ms = new MemoryStream())
        {
            inputStream.CopyTo(ms);
            inputData = ms.ToArray();
        }

        var imgOptions = IsGif(inputData)
            ? new VOption { { "n", -1 } }
            : null;

        using (var image = TryLoadImage(inputData, imgOptions))
        {
            var currentType = GetImageType(image);
            var originalWidth = image.Width;
            var originalHeight = image.PageHeight;
            var isNeedResize = originalWidth > settings.MaxWidth || originalHeight > settings.MaxHeight;
            var isChangeType = currentType != settings.OutputType;

            if (isChangeType)
                messages.Add($"Warning: image type changed ({currentType.GetName()} -> {settings.OutputType.GetName()})");

            if (!isNeedResize && !isChangeType)
            {
                outputStream.Write(inputData, 0, inputData.Length);
                return;
            }

            var processedImage = RemoveMetadata(image);

            if (isNeedResize)
            {
                processedImage = ResizeImage(processedImage, settings);

                messages.Add($"Warning: image resized ({originalWidth}x{originalHeight} -> {settings.MaxWidth}x{settings.MaxHeight})");
            }

            WriteImage(processedImage, outputStream, settings);
        }
    }

    private static void WriteImage(Image processedImage, Stream outputStream, AdjustImageSettings settings)
    {
        byte[] outputData;

        if (settings.OutputType == ImageType.Gif)
        {
            outputData = processedImage.GifsaveBuffer();
        }
        else if (settings.OutputType == ImageType.Jpeg)
        {
            outputData = processedImage.JpegsaveBuffer(
                q: 85,
                interlace: true,
                optimizeCoding: true,
                subsampleMode: Enums.ForeignSubsample.Off,
                keep: Enums.ForeignKeep.None
            );
        }
        else if (settings.OutputType == ImageType.Png)
        {
            outputData = processedImage.PngsaveBuffer(
                compression: 9,
                interlace: false,
                filter: Enums.ForeignPngFilter.All
            );
        }
        else
        {
            throw new ApplicationException($"Unexpected image output type: {settings.OutputType}");
        }

        outputStream.Write(outputData, 0, outputData.Length);
    }

    private static Image ResizeImage(Image image, AdjustImageSettings settings)
    {
        if (settings.Crop)
        {
            var nPages = image.Contains("n-pages") ? (int)image.Get("n-pages") : 1;
            if (nPages > 1)
                return ResizeAnimatedWithCrop(image, settings.MaxWidth, settings.MaxHeight, nPages);
        }

        return image.ThumbnailImage(
            width: settings.MaxWidth,
            height: settings.MaxHeight,
            size: Enums.Size.Down,
            crop: settings.Crop ? Enums.Interesting.Centre : null
        );
    }

    private static Image ResizeAnimatedWithCrop(Image image, int maxWidth, int maxHeight, int nPages)
    {
        Image result;

        var originalWidth = image.Width;
        var pageHeight = image.PageHeight;
        var frames = new List<Image>();

        try
        {
            for (var i = 0; i < nPages; i++)
            {
                var frame = image.Crop(0, i * pageHeight, originalWidth, pageHeight);
                var resizedFrame = frame.ThumbnailImage(
                    width: maxWidth,
                    height: maxHeight,
                    size: Enums.Size.Down,
                    crop: Enums.Interesting.Centre
                );

                frames.Add(resizedFrame);
            }

            result = Image.Arrayjoin(frames.ToArray(), across: 1);

            result = result.Mutate(mutable =>
            {
                mutable.Set(GValue.GIntType, "page-height", maxHeight);

                if (image.Contains("delay"))
                    mutable.Set(GValue.ArrayIntType, "delay", image.Get("delay"));

                if (image.Contains("loop"))
                    mutable.Set(GValue.GIntType, "loop", image.Get("loop"));
            });
        }
        finally
        {
            foreach (var frame in frames)
                frame.Dispose();
        }

        return result;
    }

    private static bool IsGif(byte[] data)
    {
        return data[0] == 'G'
            && data[1] == 'I'
            && data[2] == 'F'
            && data[3] == '8'
            && (data[4] == '7' || data[4] == '9')
            && data[5] == 'a';
    }

    private static bool IsBmp(byte[] data)
    {
        return data[0] == 'B'
            && data[1] == 'M';
    }

    private static readonly string[] MetadataFields =
    {
            "exif-data", "exif-ifd0-Orientation", "exif-ifd2-ExifVersion",
            "icc-profile-data", "xmp-data", "iptc-data",
            "gif-comment", "png-comment", "jpeg-comment"
        };

    private static Image RemoveMetadata(Image image)
    {
        var result = image.Copy();

        foreach (var field in MetadataFields)
        {
            if (result.Contains(field))
                result = result.Mutate(mutable => mutable.Remove(field));
        }

        return result;
    }

    private static Shift.Common.Integration.ImageMagick.ColorSpace GetColorSpace(Image img)
    {
        var interpretation = img.Interpretation;

        if (interpretation == Enums.Interpretation.Bw ||
            interpretation == Enums.Interpretation.Grey16)
            return Shift.Common.Integration.ImageMagick.ColorSpace.Gray;

        if (interpretation == Enums.Interpretation.Error)
            return Shift.Common.Integration.ImageMagick.ColorSpace.Undefined;

        return Shift.Common.Integration.ImageMagick.ColorSpace.Other;
    }

    private static ImageType GetImageType(Image image)
    {
        try
        {
            var loader = (image.Contains("sysdraw-format")
                ? image.Get("sysdraw-format")
                : image.Get("vips-loader")) as string;

            if (loader.IsEmpty())
                return ImageType.Null;

            loader = loader!.ToLowerInvariant();

            if (loader.Contains("gif"))
                return ImageType.Gif;

            if (loader.Contains("jpeg") || loader.Contains("jpg"))
                return ImageType.Jpeg;

            if (loader.Contains("png"))
                return ImageType.Png;

            if (loader.Contains("tiff") || loader.Contains("tif"))
                return ImageType.Tiff;

            if (loader.Contains("bmp"))
                return ImageType.Bmp;

            return ImageType.Null;
        }
        catch
        {
            return ImageType.Null;
        }
    }

    #endregion
}
