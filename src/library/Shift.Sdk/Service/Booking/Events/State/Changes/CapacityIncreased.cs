using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class CapacityIncreased : Change
    {
        public int Increment { get; set; }

        public CapacityIncreased(int increment)
        {
            Increment = increment;
        }
    }
}