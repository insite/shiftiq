using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationEventSettings : Command, IHasRun
    {
        public EventSettings Events { get; set; }

        public ModifyOrganizationEventSettings(Guid organizationId, EventSettings events)
        {
            AggregateIdentifier = organizationId;
            Events = events;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Events.IsEqual(Events))
                return true;

            aggregate.Apply(new OrganizationEventSettingsModified(Events));

            return true;
        }
    }
}
