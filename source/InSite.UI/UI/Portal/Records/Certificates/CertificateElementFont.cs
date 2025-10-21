using System.ComponentModel;
using System.Drawing;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Sdk.UI;

namespace InSite.UI.Portal.Records.Certificates
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class CertificateElementFont
    {
        #region Properties

        [JsonProperty(PropertyName = "name")]
        public string FamilyName { get; set; } = "Calibri";

        [DefaultValue(FontStyle.Regular)]
        [JsonConverter(typeof(JsonEnumConverter))]
        [JsonProperty(PropertyName = "style", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public FontStyle Style { get; set; } = FontStyle.Regular;

        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; } = 16;

        [JsonConverter(typeof(JsonColorConverter))]
        [JsonProperty(PropertyName = "color")]
        public System.Drawing.Color Color { get; set; } = System.Drawing.Color.Black;

        #endregion

        #region Methods

        public Font CreateFont() => new Font(FamilyName, Size, Style, GraphicsUnit.Pixel);

        public Brush CreateBrush() => new SolidBrush(Color);

        #endregion
    }
}