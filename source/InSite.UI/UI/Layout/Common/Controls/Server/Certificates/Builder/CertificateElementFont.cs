using System.ComponentModel;
using System.Drawing;

using InSite.Common.Web.UI.Certificates.Json;

using Newtonsoft.Json;

using JsonEnumConverter = Shift.Sdk.UI.JsonEnumConverter;

namespace InSite.Common.Web.UI.Certificates
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class CertificateElementFont
    {
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
    }
}