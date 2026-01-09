using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new CourseState();

        public CourseState Data => (CourseState)State;
    }
}
