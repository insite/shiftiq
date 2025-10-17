using Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationPlatformSettingsModified : Change
    {
        public string EmbeddedHelpContentUrl { get; set; }

        public string SafeExamBrowserUserAgentSuffix { get; set; }

        public OrganizationPlatformSettingsModified(string embeddedHelpContentUrl, string safeExamBrowserUserAgentSuffix)
        {
            EmbeddedHelpContentUrl = embeddedHelpContentUrl;
            SafeExamBrowserUserAgentSuffix = safeExamBrowserUserAgentSuffix;
        }
    }
}
