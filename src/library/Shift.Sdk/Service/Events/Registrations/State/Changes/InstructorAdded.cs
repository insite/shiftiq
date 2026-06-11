using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class InstructorAdded : Change
    {
        public Guid Instructor { get; set; }

        public InstructorAdded(Guid instructor)
        {
            Instructor = instructor;
        }
    }
}
