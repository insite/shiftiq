using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationNCSHASettings : Command, IHasRun
    {
        public NCSHASettings Settings { get; set; }

        public ModifyOrganizationNCSHASettings(Guid organizationId, NCSHASettings settings)
        {
            AggregateIdentifier = organizationId;
            Settings = settings;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.NCSHA.IsEqual(Settings))
                return true;

            aggregate.Apply(new OrganizationNCSHASettingsModified(Settings));

            return true;
        }
    }
}
