using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class CapacityDecreased : Change
    {
        public int Decrement { get; set; }

        public CapacityDecreased(int decrement)
        {
            Decrement = decrement;
        }
    }
}