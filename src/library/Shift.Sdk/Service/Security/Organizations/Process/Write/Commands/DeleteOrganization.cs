using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class DeleteOrganization : Command, IHasRun
    {

        public DeleteOrganization(Guid organizationId)
        {
            AggregateIdentifier = organizationId;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus == AccountStatus.Destroyed)
                return false;

            aggregate.Apply(new OrganizationDeleted());

            return true;
        }
    }
}
