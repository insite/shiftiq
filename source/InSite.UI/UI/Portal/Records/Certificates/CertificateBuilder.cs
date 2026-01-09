using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;

using Microsoft.Reporting.WebForms;

using Shift.Sdk.UI;

namespace InSite.UI.Portal.Records.Certificates
{
    public class CertificateBuilder
    {
        #region Classes

        private class PdfReportDataItem
        {
            #region Properties

            public byte[] Image { get; }

            #endregion

            #region Construction

            public PdfReportDataItem(byte[] image)
            {
                Image = image;
            }

            #endregion
        }

        #endregion

        #region Properties (instance)

        public Graphics Graphics { get; }

        public Bitmap BackgroundBitmap { get; }

        public CertificateVariables Variables { get; }

        #endregion

        #region Construction

        private CertificateBuilder(Graphics gfx, Bitmap bgBmp, CertificateVariables variables)
        {
            Graphics = gfx;
            BackgroundBitmap = bgBmp;
            Variables = variables ?? new CertificateVariables();
        }

        #endregion

        #region Methods

        public static void SaveImage(
            string outputPath, string imagePath,
            IEnumerable<CertificateBaseElement> elements,
            CertificateVariables variables = null,
            int? maxWidth = null, int? maxHeight = null)
        {
            var outputPhysPath = HttpContext.Current.Server.MapPath(outputPath);

            var directory = Path.GetDirectoryName(outputPhysPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            CreateImageInternal(
                bmp => bmp.Save(outputPhysPath, ImageFormat.Png),
                imagePath, elements, variables, maxWidth, maxHeight);
        }

        public static byte[] CreateImage(
            string imagePath,
            IEnumerable<CertificateBaseElement> elements,
            CertificateVariables variables = null,
            int? maxWidth = null, int? maxHeight = null)
        {
            byte[] result = null;

            CreateImageInternal(
                bmp =>
                {
                    using (var ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);

                        result = ms.ToArray();
                    }
                },
                imagePath, elements, variables, maxWidth, maxHeight);

            return result;
        }

        public static byte[] CreatePdf(
            string imagePath,
            IEnumerable<CertificateBaseElement> elements,
            CertificateVariables variables = null)
        {
            const string deviceInfo = @"
<DeviceInfo>
    <OutputFormat>PDF</OutputFormat>
    <PageWidth>10.5in</PageWidth>
    <PageHeight>8.0in</PageHeight>
    <MarginTop>0.5in</MarginTop>
    <MarginLeft>0.5in</MarginLeft>
    <MarginRight>0.5in</MarginRight>
    <MarginBottom>0.5in</MarginBottom>
</DeviceInfo>";

            var report = new LocalReport
            {
                ReportPath = HttpContext.Current.Server.MapPath("~/UI/Portal/Records/Certificates/Certificate.rdlc")
            };
            report.DataSources.Add(new ReportDataSource(
                "Certificates",
                new[] { new PdfReportDataItem(CreateImage(imagePath, elements, variables)) }));

            return report.Render("PDF", deviceInfo, out string mimeType, out string encoding, out string filenameExtension, out string[] streamids, out Warning[] warnings);
        }

        private static void CreateImageInternal(
            Action<Bitmap> save,
            string imagePath,
            IEnumerable<CertificateBaseElement> elements,
            CertificateVariables variables,
            int? maxWidth, int? maxHeight)
        {
            try
            {
                using (var bmp = CertificateImageElement.LoadImage(imagePath))
                {
                    Render(bmp, elements, variables);

                    var c1 = !maxWidth.HasValue ? 1 : (decimal)maxWidth.Value / bmp.Width;
                    var c2 = !maxHeight.HasValue ? 1 : (decimal)maxHeight.Value / bmp.Height;
                    var cr = c1 < c2 ? c1 : c2;

                    if (cr < 1)
                    {
                        var scaledWidth = (int)(bmp.Width * cr);
                        var scaledHeight = (int)(bmp.Height * cr);

                        using (var scaledBmp = new Bitmap(scaledWidth, scaledHeight))
                        {
                            using (var gfx = Graphics.FromImage(scaledBmp))
                            {
                                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                gfx.DrawImage(bmp, new Rectangle(0, 0, scaledWidth, scaledHeight));
                            }

                            save(scaledBmp);
                        }
                    }
                    else
                        save(bmp);
                }
            }
            catch (ExternalException ex)
            {
                throw new Exception("Unable to build a certificate image.", ex);
            }
        }

        private static void Render(Bitmap bgBmp, IEnumerable<CertificateBaseElement> elements, CertificateVariables variables)
        {
            using (var gfx = Graphics.FromImage(bgBmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var builder = new CertificateBuilder(gfx, bgBmp, variables);

                foreach (var el in elements)
                    el.Render(builder);
            }
        }

        #endregion
    }
}