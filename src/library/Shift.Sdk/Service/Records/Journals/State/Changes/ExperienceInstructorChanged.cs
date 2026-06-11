using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceInstructorChanged : Change
    {
        public Guid Experience { get; }
        public string Instructor { get; }

        public ExperienceInstructorChanged(Guid experience, string instructor)
        {
            Experience = experience;
            Instructor = instructor;
        }
    }
}
