using System;

using Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationPlatformSettings : Command, IHasRun
    {
        public string EmbeddedHelpContentUrl { get; set; }
        public string SafeExamBrowserUserAgentSuffix { get; set; }

        public ModifyOrganizationPlatformSettings(
            Guid organizationId,
            string embeddedHelpContentUrl,
            string safeExamBrowserUserAgentSuffix)
        {
            AggregateIdentifier = organizationId;
            EmbeddedHelpContentUrl = embeddedHelpContentUrl;
            SafeExamBrowserUserAgentSuffix = safeExamBrowserUserAgentSuffix;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            var url = state.PlatformCustomization.EmbeddedHelpContentUrl;

            var suffix = state.PlatformCustomization.SafeExamBrowserUserAgentSuffix;

            var isSame = suffix.NullIfEmpty() == SafeExamBrowserUserAgentSuffix.NullIfEmpty() &&
                url.NullIfEmpty() == EmbeddedHelpContentUrl.NullIfEmpty();

            if (isSame)
                return true;

            aggregate.Apply(new OrganizationPlatformSettingsModified(EmbeddedHelpContentUrl, SafeExamBrowserUserAgentSuffix));

            return true;
        }
    }
}
