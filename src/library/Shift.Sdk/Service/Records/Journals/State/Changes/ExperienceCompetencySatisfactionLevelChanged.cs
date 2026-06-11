using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class ExperienceCompetencySatisfactionLevelChanged : Change
    {
        public Guid Experience { get; }
        public Guid Competency { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ExperienceCompetencySatisfactionLevel SatisfactionLevel { get; }

        public ExperienceCompetencySatisfactionLevelChanged(Guid experience, Guid competency, ExperienceCompetencySatisfactionLevel satisfactionLevel)
        {
            Experience = experience;
            Competency = competency;
            SatisfactionLevel = satisfactionLevel;
        }
    }
}
