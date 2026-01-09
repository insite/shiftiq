using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceCompetencySatisfactionLevel : Command
    {
        public Guid Experience { get; }
        public Guid Competency { get; }
        public ExperienceCompetencySatisfactionLevel SatisfactionLevel { get; }

        public ChangeExperienceCompetencySatisfactionLevel(Guid journal, Guid experience, Guid competency, ExperienceCompetencySatisfactionLevel satisfactionLevel)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Competency = competency;
            SatisfactionLevel = satisfactionLevel;
        }
    }
}
