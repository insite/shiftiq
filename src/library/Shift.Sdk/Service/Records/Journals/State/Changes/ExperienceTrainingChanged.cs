using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceTrainingChanged : Change
    {
        public Guid Experience { get; }
        public string Level { get; }
        public string Location { get; }
        public string Provider { get; }
        public string CourseTitle { get; }
        public string Comment { get; }
        public string Type { get; }

        public ExperienceTrainingChanged(Guid experience, string level, string location, string provider, string courseTitle, string comment, string type)
        {
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
