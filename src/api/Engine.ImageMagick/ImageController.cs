using System.Text;

using ImageMagick;

using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Common.Integration.ImageMagick;
using Shift.Constant;

namespace Engine.ImageMagick;

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

            var imgInfo = new MagickImageInfo(data);

            return Ok(new ImageInfo
            {
                Height = imgInfo.Height,
                Width = imgInfo.Width,
                Format = imgInfo.Format == MagickFormat.Unknown ? ImageFormat.Unknown : ImageFormat.Known,
                ImageType = GetImageType(imgInfo),
                ColorSpace = GetColorSpace(imgInfo),
                PixelsPerInch = GetImageDensity(imgInfo),
            });
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

    public static void AdjustImage(Stream inputStream, Stream outputStream, AdjustImageSettings settings, List<string> messages)
    {
        var imgInfo = new MagickImageInfo(inputStream);

        inputStream.Seek(0, SeekOrigin.Begin);

        var currentType = GetImageType(imgInfo);
        var isNeedResize = imgInfo.Width > settings.MaxWidth || imgInfo.Height > settings.MaxHeight;
        var isChangeType = currentType != settings.OutputType;

        if (isChangeType)
            messages.Add($"Warning: image type changed ({currentType.GetName()} -> {settings.OutputType.GetName()})");

        if (!isNeedResize && !isChangeType)
        {
            inputStream.CopyTo(outputStream);
        }
        else if (settings.OutputType == ImageType.Gif)
        {
            using (var collection = new MagickImageCollection(inputStream))
            {
                collection.Coalesce();

                foreach (MagickImage image in collection)
                {
                    Resize(imgInfo, image, settings.MaxWidth, settings.MaxHeight, isNeedResize, settings.Crop, messages);

                    image.Strip();
                    image.Settings.Interlace = Interlace.NoInterlace;
                }

                collection.Write(outputStream);
            }
        }
        else if (settings.OutputType == ImageType.Jpeg)
        {
            using (var image = new MagickImage(inputStream))
            {
                Resize(imgInfo, image, settings.MaxWidth, settings.MaxHeight, isNeedResize, settings.Crop, messages);
                WriteJpeg(image, outputStream);
            }
        }
        else if (settings.OutputType == ImageType.Png)
        {
            using (var image = new MagickImage(inputStream))
            {
                Resize(imgInfo, image, settings.MaxWidth, settings.MaxHeight, isNeedResize, settings.Crop, messages);
                WritePng(image, outputStream);
            }
        }
        else
            throw ApplicationError.Create("Unexpexted image output type: {0}", settings.OutputType);
    }

    private static void Resize(IMagickImageInfo info, MagickImage image, int width, int height, bool isResizeNeeded, bool isCropNeeded, List<string> messages)
    {
        if (!isResizeNeeded)
            return;

        image.FilterType = FilterType.Triangle;
        image.Settings.SetDefine("filter:support", "2");

        if (isCropNeeded)
        {
            var resizeRatio = (decimal)height / width;
            var imageRatio = (decimal)image.Height / image.Width;

            if (imageRatio > resizeRatio)
                image.Crop(image.Width, (uint)Math.Round(resizeRatio * image.Width), Gravity.Center);
            else
                image.Crop((uint)Math.Round(image.Height / resizeRatio), image.Height, Gravity.Center);
        }

        image.AdaptiveResize((uint)width, (uint)height);

        messages.Add($"Warning: image resized ({info.Width}x{info.Height} -> {width}x{height})");
    }

    private static void WriteJpeg(MagickImage image, Stream outputStream)
    {
        const int MaxQuality = 85;

        image.Strip();
        image.Settings.Interlace = Interlace.Plane;
        image.Format = MagickFormat.Pjpeg;

        if (image.Quality == 0 || image.Quality > (MaxQuality + 1))
            image.Quality = MaxQuality;

        image.Settings.SetDefine(MagickFormat.Jpeg, "fancy-upsampling", "off");
        image.Settings.SetDefine(MagickFormat.Jpeg, "dct-method", "float");
        image.Settings.SetDefine(MagickFormat.Jpeg, "optimize-coding", "true");

        image.Write(outputStream);
    }

    private static void WritePng(MagickImage image, Stream outputStream)
    {
        image.Strip();
        image.Settings.Interlace = Interlace.NoInterlace;

        if (GetImageType(image.Format) != ImageType.Png)
            image.Format = MagickFormat.Png;

        image.Settings.SetDefine(MagickFormat.Png, "exclude-chunks", "all");
        image.Settings.SetDefine(MagickFormat.Png, "include-chunks", "tRNS,gAMA");
        image.Settings.SetDefine(MagickFormat.Png, "compression-filter", "5");
        image.Settings.SetDefine(MagickFormat.Png, "compression-level", "9");
        image.Settings.SetDefine(MagickFormat.Png, "compression-strategy", "1");

        if (image.HasAlpha && image.IsOpaque)
            image.HasAlpha = false;

        image.Write(outputStream);
    }
    private static Shift.Common.Integration.ImageMagick.ColorSpace GetColorSpace(IMagickImageInfo imgInfo)
    {
        if (imgInfo.ColorSpace == global::ImageMagick.ColorSpace.Gray)
            return Shift.Common.Integration.ImageMagick.ColorSpace.Gray;

        if (imgInfo.ColorSpace == global::ImageMagick.ColorSpace.Undefined)
            return Shift.Common.Integration.ImageMagick.ColorSpace.Undefined;

        return Shift.Common.Integration.ImageMagick.ColorSpace.Other;
    }

    private static ImageType GetImageType(IMagickImageInfo imgInfo) => GetImageType(imgInfo.Format);

    private static ImageType GetImageType(MagickFormat format)
    {
        if (format == MagickFormat.Bmp
            || format == MagickFormat.Bmp2
            || format == MagickFormat.Bmp3)
            return ImageType.Bmp;

        if (format == MagickFormat.Gif
            || format == MagickFormat.Gif87)
            return ImageType.Gif;

        if (format == MagickFormat.Jpe
            || format == MagickFormat.Jpeg
            || format == MagickFormat.Jpg
            || format == MagickFormat.Jps
            || format == MagickFormat.Pjpeg)
            return ImageType.Jpeg;

        if (format == MagickFormat.Png
            || format == MagickFormat.Png00
            || format == MagickFormat.Png8
            || format == MagickFormat.Png24
            || format == MagickFormat.Png32
            || format == MagickFormat.Png48
            || format == MagickFormat.Png64)
            return ImageType.Png;

        if (format == MagickFormat.Tif
            || format == MagickFormat.Tiff
            || format == MagickFormat.Tiff64)
            return ImageType.Tiff;

        return ImageType.Null;
    }

    private static double GetImageDensity(IMagickImageInfo imgInfo)
    {
        var density = imgInfo.Density;
        if (density != null)
        {
            if (density.Units == DensityUnit.PixelsPerCentimeter)
                density = density.ChangeUnits(DensityUnit.PixelsPerInch);

            return density.X;
        }

        return -1;
    }

    #endregion
}
