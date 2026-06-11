using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationStandardSettings : Command, IHasRun
    {
        public StandardSettings Standards { get; set; }

        public ModifyOrganizationStandardSettings(Guid organizationId, StandardSettings standards)
        {
            AggregateIdentifier = organizationId;
            Standards = standards;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Standards.IsEqual(Standards))
                return true;

            aggregate.Apply(new OrganizationStandardSettingsModified(Standards));

            return true;
        }
    }
}
