using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupCapacityChanged : Change
    {
        public int? Capacity { get; }

        public GroupCapacityChanged(int? capacity)
        {
            Capacity = capacity;
        }
    }
}
