using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Sdk.UI;

namespace InSite.UI.Portal.Records.Certificates
{
    [JsonConverter(typeof(JsonElementConverter))]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class CertificateBaseElement
    {
        #region Properties

        [JsonProperty(PropertyName = "layout", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CertificateElementLayout Layout { get; set; } = new CertificateElementLayout();

        #endregion

        #region Methods

        public abstract void Render(CertificateBuilder builder);

        #endregion
    }
}