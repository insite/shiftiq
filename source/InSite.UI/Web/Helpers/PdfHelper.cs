using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;

using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

using Shift.Common;
using Shift.Constant;

namespace InSite.Web.Helpers
{
    public static class PdfHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public enum WatermarkPosition
        {
            Centered,
            Diagonal
        }

        public static byte[] Process(byte[] data, Action<PdfDocument> action)
        {
            using (var inStream = new MemoryStream(data))
            {
                using (var doc = PdfReader.Open(inStream, PdfDocumentOpenMode.Modify))
                {
                    action(doc);

                    using (var outStream = new MemoryStream())
                    {
                        doc.Save(outStream, true);

                        return outStream.ToArray();
                    }
                }
            }
        }

        public static void AddWatermark(PdfDocument document, XImage img, WatermarkPosition position = WatermarkPosition.Centered)
        {
            foreach (var page in document.Pages)
            {
                using (var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend))
                {
                    if (position == WatermarkPosition.Centered)
                    {
                        var pageWidth = page.Width.Point;
                        var pageHeight = page.Height.Point;

                        var imgWidth = pageWidth * 0.9;
                        var imgHeight = img.PixelHeight * (imgWidth / img.PixelWidth);

                        var x = (pageWidth - imgWidth) / 2;
                        var y = (pageHeight - imgHeight) / 2;

                        gfx.DrawImage(img, x, y, imgWidth, imgHeight);
                    }
                    else if (position == WatermarkPosition.Diagonal)
                    {
                        var state = gfx.Save();

                        gfx.TranslateTransform(page.Width.Point / 2, page.Height.Point / 2);
                        gfx.RotateTransform(-45);

                        var width = page.Width.Point * 0.9;
                        var height = img.PointHeight * (width / img.PointWidth);

                        gfx.DrawImage(img, -width / 2, -height / 2, width, height);

                        gfx.Restore(state);
                    }
                    else
                        throw ApplicationError.Create("Unexpected WatermarkPosition: {0}", position.GetName());
                }
            }
        }

        public static XImage LoadImageByUrl(string url, bool greyscale = false, double opacity = -1)
        {
            if (url.IsEmpty())
                return null;

            byte[] imgData;

            try
            {
                imgData = TaskRunner.RunSync(async () => await _httpClient.GetByteArrayAsync(url));
            }
            catch (HttpRequestException hrex)
            {
                if (hrex.Message == "Response status code does not indicate success: 404 (Not Found).")
                    return null;

                throw hrex;
            }

            using (var inStream = new MemoryStream(imgData))
            {
                using (var inImg = Image.FromStream(inStream))
                {
                    using (var outImg = ProcessImage(inImg, greyscale, opacity))
                    {
                        using (var outStream = new MemoryStream())
                        {
                            outImg.Save(outStream, ImageFormat.Png);
                            outStream.Position = 0;

                            return XImage.FromStream(outStream);
                        }
                    }
                }
            }
        }

        private static Image ProcessImage(Image inImg, bool greyscale, double opacity)
        {
            var hasTransform = false;
            var transMatrix = new float[5][];

            if (greyscale)
            {
                hasTransform = true;

                transMatrix[0] = new float[] { 0.299f, 0.299f, 0.299f, 0, 0 };
                transMatrix[1] = new float[] { 0.587f, 0.587f, 0.587f, 0, 0 };
                transMatrix[2] = new float[] { 0.114f, 0.114f, 0.114f, 0, 0 };
            }
            else
            {
                transMatrix[0] = new float[] { 1, 0, 0, 0, 0 };
                transMatrix[1] = new float[] { 0, 1, 0, 0, 0 };
                transMatrix[2] = new float[] { 0, 0, 1, 0, 0 };
            }

            if (opacity >= 0 && opacity <= 1)
            {
                hasTransform = true;

                transMatrix[3] = new float[] { 0, 0, 0, (float)opacity, 0 };
            }
            else
            {
                transMatrix[3] = new float[] { 0, 0, 0, 1, 0 };
            }

            if (!hasTransform)
                return inImg;

            transMatrix[4] = new float[] { 0, 0, 0, 0, 1 };

            var outImg = new Bitmap(inImg.Width, inImg.Height, PixelFormat.Format32bppArgb);

            using (var gfx = Graphics.FromImage(outImg))
            {
                var colorMatrix = new ColorMatrix(transMatrix);

                using (var imgAttrs = new ImageAttributes())
                {
                    imgAttrs.SetColorMatrix(colorMatrix);

                    gfx.DrawImage(inImg,
                        new Rectangle(0, 0, inImg.Width, inImg.Height),
                        0, 0, inImg.Width, inImg.Height,
                        GraphicsUnit.Pixel, imgAttrs);
                }
            }

            return outImg;
        }

        public static void SetReadOnly(PdfDocument document)
        {
            var security = document.SecuritySettings;

            security.PermitPrint = true;
            security.PermitFullQualityPrint = true;

            security.PermitAnnotations = false;
            security.PermitAssembleDocument = false;
            security.PermitExtractContent = false;
            security.PermitFormsFill = false;
            security.PermitModifyDocument = false;
        }
    }
}