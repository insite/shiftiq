
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class AttendanceTaken : Change
    {
        public string Status { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }

        public AttendanceTaken(string status, decimal? quantity, string unit)
        {
            Status = status;
            Quantity = quantity;
            Unit = unit;
        }
    }
}
