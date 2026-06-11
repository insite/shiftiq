using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class EmployerAssigned : Change
    {
        public Guid? Employer { get; set; }

        public EmployerAssigned(Guid? employer)
        {
            Employer = employer;
        }
    }
}
