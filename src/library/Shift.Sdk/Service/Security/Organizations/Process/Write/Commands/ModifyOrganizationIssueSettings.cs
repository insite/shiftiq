using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationIssueSettings : Command, IHasRun
    {
        public IssueSettings Issues { get; set; }

        public ModifyOrganizationIssueSettings(Guid organizationId, IssueSettings issues)
        {
            AggregateIdentifier = organizationId;
            Issues = issues;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Issues.IsEqual(Issues))
                return true;

            aggregate.Apply(new OrganizationIssueSettingsModified(Issues));

            return true;
        }
    }
}
