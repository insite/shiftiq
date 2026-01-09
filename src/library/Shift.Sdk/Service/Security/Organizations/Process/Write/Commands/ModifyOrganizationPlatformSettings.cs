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

        public ModifyOrganizationPlatformSettings(
            Guid organizationId,
            string inlineInstructionsUrl,
            string inlineLabelsUrl,
            string safeExamBrowserUserAgentSuffix)
        {
            AggregateIdentifier = organizationId;
            InlineInstructionsUrl = inlineInstructionsUrl;
            InlineLabelsUrl = inlineLabelsUrl;
            SafeExamBrowserUserAgentSuffix = safeExamBrowserUserAgentSuffix;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            var inlineInstructionsUrl = state.PlatformCustomization.InlineInstructionsUrl;
            var inlineLabelsUrl = state.PlatformCustomization.InlineLabelsUrl;
            var safeExamBrowserUserAgentSuffix = state.PlatformCustomization.SafeExamBrowserUserAgentSuffix;

            var isSame = safeExamBrowserUserAgentSuffix.NullIfEmpty() == SafeExamBrowserUserAgentSuffix.NullIfEmpty() &&
                inlineInstructionsUrl.NullIfEmpty() == InlineInstructionsUrl.NullIfEmpty() &&
                inlineLabelsUrl.NullIfEmpty() == InlineLabelsUrl.NullIfEmpty()
                ;

            if (isSame)
                return true;

            aggregate.Apply(new OrganizationPlatformSettingsModified(InlineInstructionsUrl, InlineLabelsUrl, SafeExamBrowserUserAgentSuffix));

            return true;
        }
    }
}
