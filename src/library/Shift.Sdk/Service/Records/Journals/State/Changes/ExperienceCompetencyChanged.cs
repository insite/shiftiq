using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceCompetencyChanged : Change
    {
        public Guid Experience { get; }
        public Guid Competency { get; }
        public decimal? Hours { get; }

        public ExperienceCompetencyChanged(Guid experience, Guid competency, decimal? hours)
        {
            Experience = experience;
            Competency = competency;
            Hours = hours;
        }
    }
}
