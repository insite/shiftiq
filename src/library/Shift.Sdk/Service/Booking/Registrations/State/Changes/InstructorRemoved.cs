using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class InstructorRemoved : Change
    {
        public Guid Instructor { get; set; }

        public InstructorRemoved(Guid instructor)
        {
            Instructor = instructor;
        }
    }
}
