using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceCompletedChanged : Change
    {
        public Guid Experience { get; }
        public DateTime? Completed { get; }

        public ExperienceCompletedChanged(Guid experience, DateTime? completed)
        {
            Experience = experience;
            Completed = completed;
        }
    }
}
