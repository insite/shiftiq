using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class SchoolAssigned : Change
    {
        public Guid School { get; set; }

        public SchoolAssigned(Guid school)
        {
            School = school;
        }
    }
}
