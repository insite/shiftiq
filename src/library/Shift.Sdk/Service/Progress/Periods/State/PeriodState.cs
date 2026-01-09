using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    [Serializable]
    public class PeriodState : AggregateState
    {
        public Guid Identifier { get; set; }
        public Guid Tenant { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public void When(PeriodCreated e)
        {
            Identifier = e.AggregateIdentifier;
            Tenant = e.Tenant;
            Name = e.Name;
            Start = e.Start;
            End = e.End;
        }

        public void When(PeriodDeleted _)
        {
        }

        public void When(PeriodRenamed e)
        {
            Name = e.Name;
        }

        public void When(PeriodRescheduled e)
        {
            Start = e.Start;
            End = e.End;
        }
    }
}
