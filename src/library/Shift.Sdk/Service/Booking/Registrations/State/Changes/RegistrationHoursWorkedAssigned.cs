
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationHoursWorkedAssigned : Change
    {
        public int? HoursWorked { get; set; }

        public RegistrationHoursWorkedAssigned(int? hoursWorked)
        {
            HoursWorked = hoursWorked;
        }
    }
}
