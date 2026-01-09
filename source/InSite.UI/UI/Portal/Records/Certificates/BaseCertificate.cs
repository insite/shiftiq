using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Shift.Sdk.UI;

namespace InSite.UI.Portal.Records.Certificates
{
    [JsonConverter(typeof(JsonCertificateConverter))]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class BaseCertificate
    {
        private const string DefaultTitle = "Certificate of Achievement";

        #region Properties

        [DefaultValue(DefaultTitle)]
        [JsonProperty(PropertyName = "title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Title { get; set; } = DefaultTitle;

        public CertificateVariables Variables { get; } = new CertificateVariables();

        #endregion

        #region Methods

        protected abstract string GetBackgroundImageUrl();

        protected abstract IEnumerable<CertificateBaseElement> GetElements();

        public abstract bool IsValid();

        protected virtual void OnPreRender() { }

        public byte[] CreateImage(int? maxWidth = null, int? maxHeight = null)
        {
            OnPreRender();

            return CertificateBuilder.CreateImage(GetBackgroundImageUrl(), GetElements(), Variables, maxWidth, maxHeight);
        }

        public void SaveImage(string outputPath, int? maxWidth = null, int? maxHeight = null)
        {
            OnPreRender();

            CertificateBuilder.SaveImage(outputPath, GetBackgroundImageUrl(), GetElements(), Variables, maxWidth, maxHeight);
        }

        public byte[] CreatePdf()
        {
            OnPreRender();

            return CertificateBuilder.CreatePdf(GetBackgroundImageUrl(), GetElements(), Variables);
        }

        #endregion
    }

    public class JsonCertificateConverter : JsonConverter
    {
        #region Classes

        private class InternalContractResolver : DefaultContractResolver
        {
            protected override JsonConverter ResolveContractConverter(Type type)
            {
                return typeof(BaseCertificate).IsAssignableFrom(type) && !type.IsAbstract
                    ? null
                    : base.ResolveContractConverter(type);
            }
        }

        private class InternalJsonSerializer : JsonSerializer
        {
            public InternalJsonSerializer()
                : base()
            {
                ContractResolver = new InternalContractResolver();
            }
        }

        #endregion

        public override bool CanConvert(Type type) => type.IsSubclassOf(typeof(BaseCertificate));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);
            var type = jObj["type"].Value<string>();
            var inSerializer = new InternalJsonSerializer();

            if (type == "cmds")
                return jObj.ToObject<CmdsTrainingCertificate>(inSerializer);

            if (type == "pfwic")
                return jObj.ToObject<PfwicCertificate>(inSerializer);

            if (type == "iecbc-2")
                return jObj.ToObject<iecbcCertificate2>(inSerializer);

            if (type == "custom")
                return jObj.ToObject<CustomCertificate>(inSerializer);

            throw new Exception("Unexpected certificate element type: " + type);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string type;

            if (value is CmdsTrainingCertificate)
                type = "cmds";
            else if (value is PfwicCertificate)
                type = "pfwic";
            else if (value is iecbcCertificate2)
                type = "iecbc-2";
            else if (value is CustomCertificate)
                type = "custom";
            else
                throw new Exception("Unexpected certificate type: " + value.GetType());

            var inSerializer = new InternalJsonSerializer();
            var jObj = JObject.FromObject(value, inSerializer);
            jObj.AddFirst(new JProperty("type", type));
            jObj.WriteTo(writer);
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CmdsTrainingCertificate : BaseCertificate
    {
        #region Constants

        private const string DefaultIntroduction = "This certifies that";
        private const string DefaultRecipient = "{User.Name}";
        private const string DefaultAction = "successfully completed";
        private const string DefaultReason = "{Asset.Title}";
        private const string DefaultDate = "Granted {Assignment.CompletedOn:MMMM d, yyyy}";
        private const string DefaultExpires = "(Expires {Assignment.ExpiresOn:MMMM d, yyyy})";

        private const string DefaultFrameUrl = "/Library/Images/Certificates/Frames/Elegant.png";
        private const string DefaultLogoUrl = "/Library/Images/Certificates/Logos/Cmds.jpg";
        private const string DefaultBadgeUrl = "/Library/Images/Certificates/Badges/Badge-01.png";

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        public override bool IsValid()
        {
            var pass = EqualsAny(Variables["Assignment.IsPassing"], new[] { "1", "t", "true", "y", "yes" });
            return Variables["Assignment.CompletedOn"] != null && pass;
        }

        public static bool EqualsAny(string text, string[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (string value in values)
            {
                if (Equals(text, value))
                    return true;
            }

            return false;
        }

        protected override IEnumerable<CertificateBaseElement> GetElements()
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

        #endregion
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PfwicCertificate : BaseCertificate
    {
        #region Constants

        // private const string DefaultTitle = "Prepare for Work in Canada";
        private const string DefaultBackgroundImageUrl = "/files/assets/22296/pfwic-certificate.png";

        #endregion

        #region Classes

        private class ParentInfo
        {
            public Guid RootResourceIdentifier { get; set; }
            public Guid ResourceIdentifier { get; set; }
            public int AssetNumber { get; set; }
            public string ResourceType { get; set; }
        }

        #endregion

        #region Properties

        [DefaultValue(DefaultBackgroundImageUrl)]
        [JsonProperty(PropertyName = "background", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BackgroundImageUrl { get; set; } = DefaultBackgroundImageUrl;

        #endregion

        #region Methods

        public override bool IsValid()
        {
            return true;
        }

        protected override IEnumerable<CertificateBaseElement> GetElements()
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

        #endregion
    }

    

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CustomCertificate : BaseCertificate
    {
        #region Properties

        [JsonProperty(PropertyName = "background")]
        public string BackgroundImageUrl { get; set; }

        [JsonProperty(PropertyName = "elements")]
        public List<CertificateBaseElement> Elements { get; private set; } = new List<CertificateBaseElement>();

        #endregion

        #region Methods

        protected override IEnumerable<CertificateBaseElement> GetElements() => Elements;

        protected override string GetBackgroundImageUrl() => BackgroundImageUrl;

        public override bool IsValid() => true;

        #endregion
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class iecbcCertificate2 : BaseCertificate
    {
        #region Constants

        private const string DefaultBackgroundImageUrl = "/library/tenants/iecbc/images/iecbc-certificate-2.png";
        private const string DefaultDate = "{Assignment.CompletedOn:MMMM d, yyyy}";

        #endregion

        #region Classes

        private class ParentInfo
        {
            public Guid RootResourceIdentifier { get; set; }
            public Guid ResourceIdentifier { get; set; }
            public int AssetNumber { get; set; }
            public string ResourceType { get; set; }
        }

        #endregion

        #region Properties

        [DefaultValue(DefaultDate)]
        [JsonProperty(PropertyName = "date", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Date { get; set; } = DefaultDate;

        [DefaultValue(DefaultBackgroundImageUrl)]
        [JsonProperty(PropertyName = "background", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BackgroundImageUrl { get; set; } = DefaultBackgroundImageUrl;

        #endregion

        #region Methods

        public override bool IsValid()
        {
            return Variables["Assignment.CompletedOn"] != null;
        }

        protected override IEnumerable<CertificateBaseElement> GetElements()
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
                    Value = Date,
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

        #endregion
    }
}