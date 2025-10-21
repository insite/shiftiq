using System;
using System.ComponentModel;
using System.Drawing;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Sdk.UI;

namespace InSite.UI.Portal.Records.Certificates
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CertificateTextElement : CertificateBaseElement
    {
        #region Properties

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [DefaultValue(ElementVerticalAlignment.Top)]
        [JsonConverter(typeof(JsonEnumConverter))]
        [JsonProperty(PropertyName = "v_align", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ElementVerticalAlignment VerticalAlignment { get; set; }

        [DefaultValue(ElementHorizontalAlignment.Left)]
        [JsonConverter(typeof(JsonEnumConverter))]
        [JsonProperty(PropertyName = "h_align", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ElementHorizontalAlignment HorizontalAlignment { get; set; }

        [JsonProperty(PropertyName = "font")]
        public CertificateElementFont Font { get; set; } = new CertificateElementFont();

        #endregion

        #region Methods

        public override void Render(CertificateBuilder builder)
        {
            if (string.IsNullOrEmpty(Value))
                throw new Exception("Certificate text element value is null");

            StringFormat format = null;
            Font font = null;
            Brush brush = null;

            try
            {
                format = new StringFormat
                {
                    Alignment = HorizontalAlignment == ElementHorizontalAlignment.Center
                        ? StringAlignment.Center
                        : HorizontalAlignment == ElementHorizontalAlignment.Right
                            ? StringAlignment.Far
                            : StringAlignment.Near,
                    LineAlignment = VerticalAlignment == ElementVerticalAlignment.Middle
                        ? StringAlignment.Center
                        : VerticalAlignment == ElementVerticalAlignment.Bottom
                            ? StringAlignment.Far
                            : StringAlignment.Near,
                };
                font = Font.CreateFont();
                brush = Font.CreateBrush();

                var rectangle = Layout.CreateRectangleF(builder.BackgroundBitmap.Size);
                var value = builder.Variables.ReplacePlaceholders(Value);

                builder.Graphics.DrawString(value, font, brush, rectangle, format);
            }
            finally
            {
                format?.Dispose();
                font?.Dispose();
                brush?.Dispose();
            }
        }

        #endregion
    }
}