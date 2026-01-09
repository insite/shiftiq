using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ValidateExperience : Command
    {
        public Guid Experience { get; }
        public Guid? Validator { get; }
        public DateTimeOffset? Validated { get; }
        public int? SkillRating { get; }

        public ValidateExperience(Guid journal, Guid experience, Guid? validator, DateTimeOffset? validated, int? skillRating)
        { 
            AggregateIdentifier = journal;
            Experience = experience;
            Validator = validator;
            Validated = validated;
            SkillRating = skillRating;
        }
    }
}
