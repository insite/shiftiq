using System;
using System.ComponentModel;

using InSite.Common.Web.UI.Certificates.Builder;

using Newtonsoft.Json;

using Shift.Constant;

using CertificateElementLayout = Shift.Sdk.UI.CertificateElementLayout;

namespace InSite.Common.Web.UI.Certificates
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class iecbcCertificate1 : BaseCertificate
    {
        private const string DefaultBackgroundImageUrl = "/library/tenants/iecbc/images/certificates/fast.png";

        public override bool IsValid() => true;

        private class ParentInfo
        {
            public Guid RootResourceIdentifier { get; set; }
            public Guid ResourceIdentifier { get; set; }
            public int AssetNumber { get; set; }
            public string ResourceType { get; set; }
        }

        [DefaultValue(DefaultBackgroundImageUrl)]
        [JsonProperty(PropertyName = "background", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BackgroundImageUrl { get; set; } = DefaultBackgroundImageUrl;

        protected override CertificateBaseElement[] GetElements()
        {
            return new CertificateBaseElement[]
            {
                new CertificateTextElement
                {
                    Value = Title,
                    HorizontalAlignment = ElementHorizontalAlignment.Center,
                    VerticalAlignment = ElementVerticalAlignment.Top,
                    Font = new CertificateElementFont
                    {
                        Size = 35,
                        Color = System.Drawing.Color.White
                    },
                    Layout = new CertificateElementLayout
                    {
                        X = 0.058M,
                        Y = 0.009M,
                        Width = 0.644M,
                        Height = 0.129M
                    }
                },
                new CertificateTextElement
                {
                    Value = "{User.Name}",
                    HorizontalAlignment = ElementHorizontalAlignment.Center,
                    VerticalAlignment = ElementVerticalAlignment.Middle,
                    Font = new CertificateElementFont
                    {
                        Style = System.Drawing.FontStyle.Bold,
                        Size = 43
                    },
                    Layout = new CertificateElementLayout
                    {
                        X = 0.07324M,
                        Y = 0.39240M,
                        Width = 0.60742M,
                        Height = 0.08227M
                    }
                }
            };
        }

        protected override string GetBackgroundImageUrl() => BackgroundImageUrl;
    }
}