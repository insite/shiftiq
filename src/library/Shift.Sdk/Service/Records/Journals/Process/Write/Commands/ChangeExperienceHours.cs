using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceHours : Command
    {
        public Guid Experience { get; }
        public decimal? Hours { get; }

        public ChangeExperienceHours(Guid journal, Guid experience, decimal? hours)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Hours = hours;
        }
    }
}
