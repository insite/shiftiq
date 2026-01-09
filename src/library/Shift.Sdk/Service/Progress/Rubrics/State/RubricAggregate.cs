using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new RubricState();

        public RubricState Data => (RubricState)State;
    }
}
