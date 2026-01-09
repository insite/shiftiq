using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using InSite.Domain.Foundations;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Layout.Common.Contents
{
    [JsonObject]
    public class SiteConfiguration
    {
        public string Tenant { get; set; }
        public string Domain { get; set; }
        public string IconUrl { get; set; }
        public string LogoUrl { get; set; }
        public string HeaderLogoUrl { get; set; }
        public string FooterLogoUrl { get; set; }
        
        [JsonProperty]
        public List<SiteLink> SocialLinks { get; private set; }

        public SiteConfiguration()
        {
            SocialLinks = new List<SiteLink>();
        }

        public bool ShouldSerializeIconUrl()
        {
            return !string.IsNullOrEmpty(IconUrl);
        }

        public bool ShouldSerializeLogoUrl()
        {
            return !string.IsNullOrEmpty(LogoUrl);
        }

        public bool ShouldSerializeHeaderLogoUrl()
        {
            return !string.IsNullOrEmpty(HeaderLogoUrl)
                && (string.IsNullOrEmpty(LogoUrl) || !string.Equals(HeaderLogoUrl, LogoUrl, StringComparison.OrdinalIgnoreCase));
        }

        public bool ShouldSerializeFooterLogoUrl()
        {
            return !string.IsNullOrEmpty(FooterLogoUrl)
                && (string.IsNullOrEmpty(LogoUrl) || !string.Equals(FooterLogoUrl, LogoUrl, StringComparison.OrdinalIgnoreCase));
        }

        public bool ShouldSerializeSocialLinks()
        {
            return SocialLinks.Count > 0;
        }

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            if (string.IsNullOrEmpty(Tenant))
                throw new ApplicationError("Organization is required property.");

            if (string.IsNullOrEmpty(Domain))
                throw new ApplicationError("Domain is required property.");
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (string.IsNullOrEmpty(HeaderLogoUrl))
                HeaderLogoUrl = LogoUrl;

            if (string.IsNullOrEmpty(FooterLogoUrl))
                FooterLogoUrl = LogoUrl;
        }
    }
}