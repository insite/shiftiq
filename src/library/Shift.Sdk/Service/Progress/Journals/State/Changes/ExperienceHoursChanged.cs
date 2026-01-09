using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceHoursChanged : Change
    {
        public Guid Experience { get; }
        public decimal? Hours { get; }

        public ExperienceHoursChanged(Guid experience, decimal? hours)
        {
            Experience = experience;
            Hours = hours;
        }
    }
}
