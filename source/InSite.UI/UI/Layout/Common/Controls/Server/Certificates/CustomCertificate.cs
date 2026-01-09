using System.Collections.Generic;

using InSite.Common.Web.UI.Certificates.Builder;

using Newtonsoft.Json;

namespace InSite.Common.Web.UI.Certificates
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CustomCertificate : BaseCertificate
    {
        public override bool IsValid() => true;

        [JsonProperty(PropertyName = "background")]
        public string BackgroundImageUrl { get; set; }

        [JsonProperty(PropertyName = "elements")]
        public List<CertificateBaseElement> Elements { get; private set; } = new List<CertificateBaseElement>();

        protected override CertificateBaseElement[] GetElements() => Elements.ToArray();

        protected override string GetBackgroundImageUrl() => BackgroundImageUrl;
    }
}