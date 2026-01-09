using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class AddExperienceCompetency : Command
    {
        public Guid Experience { get; }
        public Guid Competency { get; }
        public decimal? Hours { get; }

        public AddExperienceCompetency(Guid journal, Guid experience, Guid competency, decimal? hours)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Competency = competency;
            Hours = hours;
        }
    }
}
