using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupLifetimeChanged : Change
    {
        public int? Quantity { get; }
        public string Unit { get; }

        public GroupLifetimeChanged(int? quantity, string unit)
        {
            Quantity = quantity;
            Unit = unit;
        }
    }
}
