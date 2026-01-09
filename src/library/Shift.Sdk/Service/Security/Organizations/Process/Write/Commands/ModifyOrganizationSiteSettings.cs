using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationSiteSettings : Command, IHasRun
    {
        public SiteSettings Sites { get; set; }

        public ModifyOrganizationSiteSettings(Guid organizationId, SiteSettings sites)
        {
            AggregateIdentifier = organizationId;
            Sites = sites;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Sites.IsEqual(Sites))
                return true;

            aggregate.Apply(new OrganizationSiteSettingsModified(Sites));

            return true;
        }
    }
}
