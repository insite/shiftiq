using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.SessionState;

using InSite.Common.Web.UI.Certificates.Builder;
using InSite.Common.Web.UI.Certificates.Json;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI.Certificates
{
    [JsonConverter(typeof(JsonCertificateConverter))]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class BaseCertificate
    {
        private const string DefaultTitle = "Certificate of Achievement";

        [DefaultValue(DefaultTitle)]
        [JsonProperty(PropertyName = "title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Title { get; set; } = DefaultTitle;

        public CertificateVariables Variables { get; } = new CertificateVariables();

        protected abstract string GetBackgroundImageUrl();

        protected abstract CertificateBaseElement[] GetElements();

        public abstract bool IsValid();

        public byte[] CreatePng(HttpResponse http, string filename, int? maxWidth = null, int? maxHeight = null)
        {
            return CertificateBuilder.CreatePng(http, filename, GetBackgroundImageUrl(), GetElements(), Variables, maxWidth, maxHeight);
        }

        public void DownloadPng(HttpSessionState session, string filename, int? maxWidth = null, int? maxHeight = null)
        {
            CertificateBuilder.DownloadPng(session, GetBackgroundImageUrl(), GetElements(), filename, Variables, maxWidth, maxHeight);
        }

        public void DownloadPdf(HttpSessionState session, string filename)
        {
            CertificateBuilder.DownloadPdf(session, GetBackgroundImageUrl(), GetElements().ToArray(), Variables);
        }
    }
}