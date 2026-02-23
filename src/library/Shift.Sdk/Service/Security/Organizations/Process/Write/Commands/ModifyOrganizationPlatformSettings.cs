using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationPlatformSettings : Command, IHasRun
    {
        public string InlineInstructionsUrl { get; set; }
        public string InlineLabelsUrl { get; set; }
        public string SafeExamBrowserUserAgentSuffix { get; set; }
        public bool RequireEmailVerification { get; set; }

        public ModifyOrganizationPlatformSettings(
            Guid organizationId,
            string inlineInstructionsUrl,
            string inlineLabelsUrl,
            string safeExamBrowserUserAgentSuffix,
            bool requireEmailVerification)
        {
            AggregateIdentifier = organizationId;
            InlineInstructionsUrl = inlineInstructionsUrl;
            InlineLabelsUrl = inlineLabelsUrl;
            SafeExamBrowserUserAgentSuffix = safeExamBrowserUserAgentSuffix;
            RequireEmailVerification = requireEmailVerification;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            var customization = state.PlatformCustomization;

            
            
            var requireEmailVerification = state.PlatformCustomization.RequireEmailVerification;

            var isSame = customization.InlineInstructionsUrl.NullIfEmpty() == InlineInstructionsUrl.NullIfEmpty()
                      && customization.InlineLabelsUrl.NullIfEmpty() == InlineLabelsUrl.NullIfEmpty()
                      && customization.SafeExamBrowserUserAgentSuffix.NullIfEmpty() == SafeExamBrowserUserAgentSuffix.NullIfEmpty()
                      && customization.RequireEmailVerification == RequireEmailVerification;

            if (isSame)
                return true;

            aggregate.Apply(new OrganizationPlatformSettingsModified(InlineInstructionsUrl, InlineLabelsUrl, SafeExamBrowserUserAgentSuffix, RequireEmailVerification));

            return true;
        }
    }
}
