using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceInstructor : Command
    {
        public Guid Experience { get; }
        public string Instructor { get; }

        public ChangeExperienceInstructor(Guid journal, Guid experience, string instructor)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Instructor = instructor;
        }
    }
}
