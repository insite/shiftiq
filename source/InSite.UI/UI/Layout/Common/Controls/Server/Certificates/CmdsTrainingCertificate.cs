using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using InSite.Common.Web.UI.Certificates.Builder;

using Newtonsoft.Json;

using Shift.Constant;

using CertificateElementLayout = Shift.Sdk.UI.CertificateElementLayout;

namespace InSite.Common.Web.UI.Certificates
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CmdsTrainingCertificate : BaseCertificate
    {
        private const string DefaultIntroduction = "This certifies that";
        private const string DefaultRecipient = "{User.Name}";
        private const string DefaultAction = "successfully completed";
        private const string DefaultReason = "{Asset.Title}";
        private const string DefaultDate = "Granted {Assignment.CompletedOn:MMMM d, yyyy}";
        private const string DefaultExpires = "(Expires {Assignment.ExpiresOn:MMMM d, yyyy})";

        private const string DefaultFrameUrl = "/Library/Images/Certificates/Frames/Elegant.png";
        private const string DefaultLogoUrl = "/Library/Images/Certificates/Logos/Cmds.jpg";
        private const string DefaultBadgeUrl = "/Library/Images/Certificates/Badges/Badge-01.png";

        public override bool IsValid() => true;

        [DefaultValue(DefaultIntroduction)]
        [JsonProperty(PropertyName = "introduction", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Introduction { get; set; } = DefaultIntroduction;

        [DefaultValue(DefaultRecipient)]
        [JsonProperty(PropertyName = "recipient", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Recipient { get; set; } = DefaultRecipient;

        [DefaultValue(DefaultAction)]
        [JsonProperty(PropertyName = "action", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Action { get; set; } = DefaultAction;

        [DefaultValue(DefaultReason)]
        [JsonProperty(PropertyName = "reason", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Reason { get; set; } = DefaultReason;

        [DefaultValue(DefaultDate)]
        [JsonProperty(PropertyName = "date", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Date { get; set; } = DefaultDate;

        [DefaultValue(DefaultExpires)]
        [JsonProperty(PropertyName = "expires", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Expires { get; set; } = DefaultExpires;

        [DefaultValue(DefaultFrameUrl)]
        [JsonProperty(PropertyName = "frame", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FrameUrl { get; set; } = DefaultFrameUrl;

        [DefaultValue(DefaultLogoUrl)]
        [JsonProperty(PropertyName = "logo", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LogoUrl { get; set; } = DefaultLogoUrl;

        [DefaultValue(DefaultBadgeUrl)]
        [JsonProperty(PropertyName = "badge", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BadgeUrl { get; set; } = DefaultBadgeUrl;

        protected override CertificateBaseElement[] GetElements()
        {
            var list = new List<CertificateBaseElement>()
            {
                new CertificateTextElement
                {
                    Value = Title,
                    HorizontalAlignment = ElementHorizontalAlignment.Center,
                    VerticalAlignment = ElementVerticalAlignment.Top,
                    Font = new CertificateElementFont
                    {
                        FamilyName = "Old English Text MT",
                        Size = 220,
                        Style = FontStyle.Bold,
                        // Color = System.Drawing.Color.SteelBlue
                    },
                    Layout = new CertificateElementLayout
                    {
                        X = 0.06M,
                        Y = 0.13M,
                        Width = 0.88M,
                        Height = 0.1M
                    }
                },
                new CertificateTextElement
                {
                    Value = Introduction,
                    HorizontalAlignment = ElementHorizontalAlignment.Center,
                    VerticalAlignment = ElementVerticalAlignment.Top,
                    Font = new CertificateElementFont
                    {
                        FamilyName = "Calibri",
                        Size = 115,
                        Style = FontStyle.Italic,
                        Color = System.Drawing.Color.DimGray
                    },
                    Layout = new CertificateElementLayout
                    {
                        X = 0.06M,
                        Y = 0.26M,
                        Width = 0.88M,
                        Height = 0.07M
                    }
                },
                new CertificateTextElement
                {
                    Value = Recipient,
                    HorizontalAlignment = ElementHorizontalAlignment.Center,
                    VerticalAlignment = ElementVerticalAlignment.Top,
                    Font = new CertificateElementFont
                    {
                        FamilyName = "Calibri",
                        Size = 135,
                        Style = FontStyle.Bold
                    },
                    Layout = new CertificateElementLayout
                    {
                        X = 0.06M,
                        Y = 0.346M,
                        Width = 0.88M,
                        Height = 0.07M
                    }
                },
                new CertificateTextElement
                {
                    Value = Action,
                    HorizontalAlignment = ElementHorizontalAlignment.Center,
                    VerticalAlignment = ElementVerticalAlignment.Top,
                    Font = new CertificateElementFont
                    {
                        FamilyName = "Calibri",
                        Size = 115,
                        Style = FontStyle.Italic,
                        Color = System.Drawing.Color.DimGray
                    },
                    Layout = new CertificateElementLayout
                    {
                        X = 0.06M,
                        Y = 0.432M,
                        Width = 0.88M,
                        Height = 0.07M
                    }
                },
                new CertificateTextElement
                {
                    Value = Reason,
                    HorizontalAlignment = ElementHorizontalAlignment.Center,
                    VerticalAlignment = ElementVerticalAlignment.Top,
                    Font = new CertificateElementFont
                    {
                        FamilyName = "Calibri",
                        Size = 135,
                        Style = FontStyle.Bold
                    },
                    Layout = new CertificateElementLayout
                    {
                        X = 0.06M,
                        Y = 0.518M,
                        Width = 0.88M,
                        Height = 0.07M
                    }
                },
                new CertificateTextElement
                {
                    Value = Date,
                    HorizontalAlignment = ElementHorizontalAlignment.Center,
                    VerticalAlignment = ElementVerticalAlignment.Top,
                    Font = new CertificateElementFont
                    {
                        FamilyName = "Calibri",
                        Size = 85
                    },
                    Layout = new CertificateElementLayout
                    {
                        X = 0.06M,
                        Y = 0.61M,
                        Width = 0.88M,
                        Height = 0.04M
                    }
                },
                new CertificateImageElement
                {
                    Path = LogoUrl,
                    HorizontalAlignment = ElementHorizontalAlignment.Left,
                    VerticalAlignment = ElementVerticalAlignment.Bottom,
                    Layout = new CertificateElementLayout
                    {
                        X = 0.11M,
                        Y = 0.66M,
                        Width = 0.2M,
                        Height = 0.2M
                    }
                },
                new CertificateImageElement
                {
                    Path = BadgeUrl,
                    HorizontalAlignment = ElementHorizontalAlignment.Right,
                    VerticalAlignment = ElementVerticalAlignment.Bottom,
                    Layout = new CertificateElementLayout
                    {
                        X = 0.71M,
                        Y = 0.62M,
                        Width = 0.18M,
                        Height = 0.24M
                    }
                }
            };

            if (!string.IsNullOrEmpty(Expires) && (!Expires.Contains("Assignment.ExpiresOn") || Variables["Assignment.ExpiresOn"] != null))
            {
                list.Add(
                    new CertificateTextElement
                    {
                        Value = Expires,
                        HorizontalAlignment = ElementHorizontalAlignment.Center,
                        VerticalAlignment = ElementVerticalAlignment.Top,
                        Font = new CertificateElementFont
                        {
                            FamilyName = "Calibri",
                            Size = 65,
                            Color = System.Drawing.Color.DimGray
                        },
                        Layout = new CertificateElementLayout
                        {
                            X = 0.06M,
                            Y = 0.656M,
                            Width = 0.88M,
                            Height = 0.04M
                        }
                    }
                );
            }

            return list.ToArray();
        }

        protected override string GetBackgroundImageUrl() => FrameUrl;
    }
}
