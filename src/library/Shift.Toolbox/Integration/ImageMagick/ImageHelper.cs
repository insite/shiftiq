using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;

using Shift.Common;
using Shift.Common.Integration.ImageMagick;

namespace Shift.Toolbox
{
    public static class ImageHelper
    {
        private static ApiSettings _apiSettings;

        public static void Initialize(ApiSettings apiSettings)
        {
            _apiSettings = apiSettings;
        }

        public static ImageInfo ReadInfo(Stream inputStream)
        {
            var client = new ImageMagickClient(_apiSettings);
            return client.GetImageInfo(inputStream);
        }

        public static void AdjustImage(
            Stream inputStream,
            Stream outputStream,
            ImageType outputType,
            bool crop,
            List<string> messages,
            int maxWidth = 2550,
            int maxHeight = 1024)
        {
            if (messages == null)
                messages = new List<string>();

            var client = new ImageMagickClient(_apiSettings);

            var result = client.AdjustImage(inputStream, new AdjustImageSettings
            {
                Crop = crop,
                MaxWidth = maxWidth,
                MaxHeight = maxHeight,
                OutputType = outputType
            });

            messages.AddRange(result.Messages);

            using (var writer = new BinaryWriter(outputStream, Encoding.UTF8, true))
            {
                writer.Write(result.ImageData);
                writer.Flush();
            }
        }

        public static bool Exists(Uri imageUri)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);

                    var response = TaskRunner.RunSync(async () =>
                        await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, imageUri))
                    );

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                // If any exception occurs (network issues, invalid URI, etc.), consider it as not existing
                return false;
            }
        }

        public static bool Exists(string imagePath, AppSettings settings, string subdomain)
        {
            return Exists(new Uri(CreateAbsoluteUrl(imagePath, settings, subdomain)));
        }

        public static string CreateAbsoluteUrl(string value, AppSettings settings, string subdomain)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            value = value.TrimStart('~');

            if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                return value;

            if (value.StartsWith("/"))
            {
                var domain = settings.Security.Domain;

                var environment = settings.Environment;

                var baseUrl = UrlHelper.GetAbsoluteUrl(domain, environment, "/", subdomain);

                return new Uri(new Uri(baseUrl), value).ToString();
            }

            return null;
        }

        public static bool IsImage(string extension) => extension != null && FileExtension.IsImage(extension);

        public static bool IsNeedResize(Image source, int maxWidth, int maxHeight) => source.Width > maxWidth || source.Height > maxHeight;

        public static Image ResizeImage(Image source, int maxWidth, int maxHeight)
        {
            var isWidth = source.Width > maxWidth;
            var isHeight = source.Height > maxHeight;

            if (!isWidth && !isHeight)
                return null;

            var ratio = isWidth && !isHeight || isWidth && source.Width > source.Height
                ? (double)maxWidth / source.Width
                : (double)maxHeight / source.Height;

            return ScaleImage(source, ratio);
        }

        public static Image ScaleImage(Image source, double ratio)
        {
            var width = (int)(source.Width * ratio);
            var height = (int)(source.Height * ratio);

            var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmp.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            using (var grPhoto = Graphics.FromImage(bmp))
            {
                grPhoto.Clear(System.Drawing.Color.Transparent);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                using (var imageAttributes = new ImageAttributes())
                {
                    imageAttributes.SetWrapMode(WrapMode.TileFlipXY);

                    grPhoto.DrawImage(
                        source,
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        0, 0, source.Width, source.Height,
                        GraphicsUnit.Pixel,
                        imageAttributes);
                }
            }

            return bmp;
        }

    }
}