using System.ComponentModel;

using InSite.Common.Web.UI.Certificates.Builder;

using Newtonsoft.Json;

using Shift.Constant;

using CertificateElementLayout = Shift.Sdk.UI.CertificateElementLayout;

namespace InSite.Common.Web.UI.Certificates
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class iecbcCertificate2 : BaseCertificate
    {
        private const string DefaultBackgroundImageUrl = "/library/tenants/iecbc/images/iecbc-certificate-2.png";
        private const string DefaultDate = "{Assignment.CompletedOn:MMMM d, yyyy}";

        public override bool IsValid() => true;

        [DefaultValue(DefaultDate)]
        [JsonProperty(PropertyName = "date", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Date { get; set; } = DefaultDate;

        [DefaultValue(DefaultBackgroundImageUrl)]
        [JsonProperty(PropertyName = "background", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BackgroundImageUrl { get; set; } = DefaultBackgroundImageUrl;

        protected override CertificateBaseElement[] GetElements()
        {
            return new CertificateBaseElement[]
            {
                new CertificateTextElement
                {
                    Value = "{User.Name}",
                    HorizontalAlignment = ElementHorizontalAlignment.Left,
                    VerticalAlignment = ElementVerticalAlignment.Middle,
                    Font = new CertificateElementFont
                    {
                        Style = System.Drawing.FontStyle.Bold,
                        Size = 120
                    },
                    Layout = new CertificateElementLayout
                    {
                        X = 0.27M,
                        Y = 0.418M,
                        Width = 0.60742M,
                        Height = 0.08227M
                    }
                },
                new CertificateTextElement
                {
                    Value = "{Assignment.CompletedOn:MMMM d, yyyy}",
                    HorizontalAlignment = ElementHorizontalAlignment.Left,
                    VerticalAlignment = ElementVerticalAlignment.Middle,
                    Font = new CertificateElementFont
                    {
                        Size = 50
                    },
                    Layout = new CertificateElementLayout
                    {
                        X = 0.275M,
                        Y = 0.425M,
                        Width = 0.644M,
                        Height = 0.129M
                    }
                }
            };
        }

        protected override string GetBackgroundImageUrl() => BackgroundImageUrl;
    }
}