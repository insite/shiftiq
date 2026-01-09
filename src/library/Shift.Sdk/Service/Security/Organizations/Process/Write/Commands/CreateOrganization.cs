using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

namespace InSite.Application.Organizations.Write
{
    public class CreateOrganization : Command, IHasRun, IHasAggregate
    {
        private OrganizationAggregate Aggregate { get; set; }

        OrganizationAggregate IHasAggregate.Aggregate => Aggregate;

        public DateTimeOffset? Opened { get; set; }

        public CreateOrganization(Guid organizationId, DateTimeOffset? opened = null)
        {
            AggregateIdentifier = organizationId;
            Opened = opened;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            Aggregate = new OrganizationAggregate { AggregateIdentifier = AggregateIdentifier };
            Aggregate.Apply(new OrganizationCreated(Opened));

            return true;
        }
    }
}
