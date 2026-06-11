using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceCompetencyDeleted : Change
    {
        public Guid Experience { get; }
        public Guid Competency { get; }

        public ExperienceCompetencyDeleted(Guid experience, Guid competency)
        {
            Experience = experience;
            Competency = competency;
        }
    }
}
