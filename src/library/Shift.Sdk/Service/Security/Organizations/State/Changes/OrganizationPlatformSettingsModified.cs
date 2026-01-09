using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationPlatformSettingsModified : Change
    {
        public string InlineInstructionsUrl { get; set; }
        public string InlineLabelsUrl { get; set; }
        public string SafeExamBrowserUserAgentSuffix { get; set; }

        public OrganizationPlatformSettingsModified(string inlineInstructionsUrl, string inlineLabelsUrl, string safeExamBrowserUserAgentSuffix)
        {
            InlineInstructionsUrl = inlineInstructionsUrl;
            InlineLabelsUrl = inlineLabelsUrl;
            SafeExamBrowserUserAgentSuffix = safeExamBrowserUserAgentSuffix;
        }
    }
}
