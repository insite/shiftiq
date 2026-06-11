using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceTimeChanged : Change
    {
        public Guid Experience { get; }
        public DateTime? Started { get; }
        public DateTime? Stopped { get; }

        public ExperienceTimeChanged(Guid experience, DateTime? started, DateTime? stopped)
        {
            Experience = experience;
            Started = started;
            Stopped = stopped;
        }
    }
}
