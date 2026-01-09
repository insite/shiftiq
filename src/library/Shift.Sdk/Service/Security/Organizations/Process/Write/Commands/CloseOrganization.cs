using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class CloseOrganization : Command, IHasRun
    {
        public DateTimeOffset? Closed { get; set; }

        public CloseOrganization(Guid organizationId, DateTimeOffset? closed = null)
        {
            AggregateIdentifier = organizationId;
            Closed = closed;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            aggregate.Apply(new OrganizationClosed(Closed));

            return true;
        }
    }
}
