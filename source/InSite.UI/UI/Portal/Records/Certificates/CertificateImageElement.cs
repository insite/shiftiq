using System;
using System.ComponentModel;
using System.Drawing;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Sdk.UI;

namespace InSite.UI.Portal.Records.Certificates
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CertificateImageElement : CertificateBaseElement
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [DefaultValue(true)]
        [JsonProperty(PropertyName = "keepRatio", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool MaintainAspectRatio { get; set; } = true;

        [DefaultValue(ElementVerticalAlignment.Top)]
        [JsonConverter(typeof(JsonEnumConverter))]
        [JsonProperty(PropertyName = "v_align", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ElementVerticalAlignment VerticalAlignment { get; set; }

        [DefaultValue(ElementHorizontalAlignment.Left)]
        [JsonConverter(typeof(JsonEnumConverter))]
        [JsonProperty(PropertyName = "h_align", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ElementHorizontalAlignment HorizontalAlignment { get; set; }

        public override void Render(CertificateBuilder builder)
        {
            using (var img = LoadImage(Path))
            {
                var rectangle = Layout.CreateRectangle(builder.BackgroundBitmap.Size);

                int outX = rectangle.X,
                    outY = rectangle.Y,
                    outWidth = rectangle.Width,
                    outHeight = rectangle.Height;

                if (MaintainAspectRatio)
                {
                    if (img.Height > img.Width)
                        outWidth = (int)(img.Width * ((decimal)outHeight / img.Height));
                    else
                        outHeight = (int)(img.Height * ((decimal)outWidth / img.Width));
                }

                if (outWidth < rectangle.Width)
                {
                    if (HorizontalAlignment == ElementHorizontalAlignment.Center)
                        outX = (int)(outX + rectangle.Width / 2M - outWidth / 2M);
                    else if (HorizontalAlignment == ElementHorizontalAlignment.Right)
                        outX = outX + rectangle.Width - outWidth;
                }

                if (outHeight < rectangle.Height)
                {
                    if (VerticalAlignment == ElementVerticalAlignment.Middle)
                        outY = (int)(outY + rectangle.Height / 2M - outHeight / 2M);
                    else if (VerticalAlignment == ElementVerticalAlignment.Bottom)
                        outY = outY + rectangle.Height - outHeight;
                }

                img.SetResolution(builder.Graphics.DpiX, builder.Graphics.DpiY);
                builder.Graphics.DrawImage(img, new Rectangle(outX, outY, outWidth, outHeight));
            }
        }

        public static bool StartsWithAny(string text, string[] values)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (string value in values)
            {
                if (text.StartsWith(value, true, null))
                    return true;
            }

            return false;
        }

        internal static Bitmap LoadImage(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (StartsWithAny(path, new[] { "/library/", "~/library/" }))
            {
                var physicalPath = HttpContext.Current.Server.MapPath(path);
                return (Bitmap)Image.FromFile(physicalPath);
            }

            throw new Exception("File not found: " + path);
        }
    }
}