using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationLocation : Command, IHasRun
    {
        public Location Location { get; set; }

        public ModifyOrganizationLocation(Guid organizationId, Location location)
        {
            AggregateIdentifier = organizationId;
            Location = location;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.PlatformCustomization.TenantLocation.IsEqual(Location))
                return true;

            aggregate.Apply(new OrganizationLocationModified(Location));

            return true;
        }
    }
}
