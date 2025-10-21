using System.ComponentModel;

using Newtonsoft.Json;

using Shift.Constant;

using JsonEnumConverter = Shift.Sdk.UI.JsonEnumConverter;

namespace InSite.Common.Web.UI.Certificates.Builder
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
    }
}