using InSite.Common.Web.UI.Certificates.Json;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI.Certificates.Builder
{
    [JsonConverter(typeof(JsonElementConverter))]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class CertificateBaseElement
    {
        [JsonProperty(PropertyName = "layout", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CertificateElementLayout Layout { get; set; } = new CertificateElementLayout();
    }
}