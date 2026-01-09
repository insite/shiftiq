using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceCompetencySkillRating : Command
    {
        public Guid Experience { get; }
        public Guid Competency { get; }
        public int? SkillRating { get; }

        public ChangeExperienceCompetencySkillRating(Guid journal, Guid experience, Guid competency, int? skillRating)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Competency = competency;
            SkillRating = skillRating;
        }
    }
}
