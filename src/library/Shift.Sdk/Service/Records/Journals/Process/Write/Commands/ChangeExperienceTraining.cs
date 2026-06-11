using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceTraining : Command
    {
        public Guid Experience { get; }
        public string Level { get; }
        public string Location { get; }
        public string Provider { get; }
        public string CourseTitle { get; }
        public string Comment { get; }
        public string Type { get; }

        public ChangeExperienceTraining(Guid journal, Guid experience, string level, string location, string provider, string courseTitle, string comment, string type)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Level = level;
            Location = location;
            Provider = provider;
            CourseTitle = courseTitle;
            Comment = comment;
            Type = type;
        }
    }
}
