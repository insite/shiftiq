using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class PeriodAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new PeriodState();
        public PeriodState Data => (PeriodState)State;

        public void Create(Guid organization, string name, DateTimeOffset start, DateTimeOffset end)
        {
            Apply(new PeriodCreated(organization, name, start, end));
        }

        public void Delete()
        {
            Apply(new PeriodDeleted());
        }

        public void Rename(string name)
        {
            Apply(new PeriodRenamed(name));
        }

        public void Reschedule(DateTimeOffset start, DateTimeOffset end)
        {
            Apply(new PeriodRescheduled(start, end));
        }
    }
}
