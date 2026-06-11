using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceCompleted : Command
    {
        public Guid Experience { get; }
        public DateTime? Completed { get; }

        public ChangeExperienceCompleted(Guid journal, Guid experience, DateTime? completed)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Completed = completed;
        }
    }
}
