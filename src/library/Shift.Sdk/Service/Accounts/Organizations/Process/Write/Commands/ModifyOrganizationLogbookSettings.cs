using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationLogbookSettings : Command, IHasRun
    {
        public LogbookSettings Logbooks { get; set; }

        public ModifyOrganizationLogbookSettings(Guid organizationId, LogbookSettings logbooks)
        {
            AggregateIdentifier = organizationId;
            Logbooks = logbooks;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Logbooks.IsEqual(Logbooks))
                return true;

            aggregate.Apply(new OrganizationLogbookSettingsModified(Logbooks));

            return true;
        }
    }
}
