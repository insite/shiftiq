using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new OrganizationState();

        public OrganizationState Data => (OrganizationState)State;
    }
}
