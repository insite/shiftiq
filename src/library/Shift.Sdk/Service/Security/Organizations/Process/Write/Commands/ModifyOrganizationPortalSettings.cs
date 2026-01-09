using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationPortalSettings : Command, IHasRun
    {
        public PortalSettings Portal { get; set; }

        public ModifyOrganizationPortalSettings(Guid organizationId, PortalSettings portal)
        {
            AggregateIdentifier = organizationId;
            Portal = portal;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Portal.IsEqual(Portal))
                return true;

            aggregate.Apply(new OrganizationPortalSettingsModified(Portal));

            return true;
        }
    }
}
