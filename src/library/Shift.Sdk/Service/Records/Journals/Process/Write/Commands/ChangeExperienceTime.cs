using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceTime : Command
    {
        public Guid Experience { get; }
        public DateTime? Started { get; }
        public DateTime? Stopped { get; }

        public ChangeExperienceTime(Guid journal, Guid experience, DateTime? started, DateTime? stopped)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Started = started;
            Stopped = stopped;
        }
    }
}
