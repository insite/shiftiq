using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignRegistrationHoursWorked : Command
    {
        public int? HoursWorked { get; set; }

        public AssignRegistrationHoursWorked(Guid aggregate, int? hoursWorked)
        {
            AggregateIdentifier = aggregate;
            HoursWorked = hoursWorked;
        }
    }
}
