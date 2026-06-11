using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationSalesSettings : Command, IHasRun
    {
        public SalesSettings Settings { get; set; }

        public ModifyOrganizationSalesSettings(Guid organizationId, SalesSettings settings)
        {
            AggregateIdentifier = organizationId;
            Settings = settings;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Sales.IsEqual(Settings))
                return true;

            aggregate.Apply(new OrganizationSalesSettingsModified(Settings));

            return true;
        }
    }
}
