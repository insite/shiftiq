
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationStatusAssigned : Change
    {
        public string Status { get; set; }

        public RegistrationStatusAssigned(string status)
        {
            Status = status;
        }
    }
}
