using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class PlatformCustomization
    {
        public PlatformUrl PlatformUrl { get; set; }
        public Location TenantLocation { get; set; }
        public OrganizationUrl TenantUrl { get; set; }

        public string InlineInstructionsUrl { get; set; }
        public string InlineLabelsUrl { get; set; }
        public string SafeExamBrowserUserAgentSuffix { get; set; }

        public AutomaticCompetencyExpiration AutomaticCompetencyExpiration { get; set; }

        public UserRegistration UserRegistration { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public UploadSettings UploadSettings { get; set; }

        public PlatformCustomizationSignIn SignIn { get; set; }

        public PlatformCustomization()
        {
            AutomaticCompetencyExpiration = new AutomaticCompetencyExpiration();
            PlatformUrl = new PlatformUrl();
            TenantLocation = new Location();
            TenantUrl = new OrganizationUrl();
            UserRegistration = new UserRegistration();
            UploadSettings = new UploadSettings();
            SignIn = new PlatformCustomizationSignIn();
        }

        public string Serialize() => JsonConvert.SerializeObject(this);

        public bool ShouldSerializeTenantLocation() => TenantLocation != null && !TenantLocation.IsEmpty;
        public bool ShouldSerializeTenantUrl() => TenantUrl != null && !TenantUrl.IsEmpty;
        public bool ShouldSerializeUserRegistration() => !UserRegistration.IsEmpty;

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (UserRegistration == null)
                UserRegistration = new UserRegistration();

            if (SignIn == null)
                SignIn = new PlatformCustomizationSignIn();
        }
    }
}