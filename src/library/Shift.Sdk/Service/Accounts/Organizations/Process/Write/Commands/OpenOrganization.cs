using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class OpenOrganization : Command, IHasRun
    {
        public OpenOrganization(Guid organizationId)
        {
            AggregateIdentifier = organizationId;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Closed)
                return false;

            aggregate.Apply(new OrganizationOpened());

            return true;
        }
    }
}
