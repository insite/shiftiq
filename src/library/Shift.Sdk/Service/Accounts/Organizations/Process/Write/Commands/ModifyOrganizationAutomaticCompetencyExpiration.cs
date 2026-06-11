using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationAutomaticCompetencyExpiration : Command, IHasRun
    {
        public AutomaticCompetencyExpiration Settings { get; set; }

        public ModifyOrganizationAutomaticCompetencyExpiration(Guid organizationId, AutomaticCompetencyExpiration settings)
        {
            AggregateIdentifier = organizationId;
            Settings = settings;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.PlatformCustomization.AutomaticCompetencyExpiration.IsEqual(Settings))
                return true;

            aggregate.Apply(new OrganizationAutomaticCompetencyExpirationModified(Settings));

            return true;
        }
    }
}
