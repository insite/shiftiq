using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceCompetencySkillRatingChanged : Change
    {
        public Guid Experience { get; }
        public Guid Competency { get; }
        public int? SkillRating { get; }

        public ExperienceCompetencySkillRatingChanged(Guid experience, Guid competency, int? skillRating)
        {
            Experience = experience;
            Competency = competency;
            SkillRating = skillRating;
        }
    }
}
