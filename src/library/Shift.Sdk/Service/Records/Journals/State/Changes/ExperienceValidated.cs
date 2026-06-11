using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceValidated : Change
    {
        public Guid Experience { get; }
        public Guid? Validator { get; }
        public DateTimeOffset? Validated { get; }
        public int? SkillRating { get; }

        public ExperienceValidated(Guid experience, Guid? validator, DateTimeOffset? validated, int? skillRating)
        {
            Experience = experience;
            Validator = validator;
            Validated = validated;
            SkillRating = skillRating;
        }
    }
}
