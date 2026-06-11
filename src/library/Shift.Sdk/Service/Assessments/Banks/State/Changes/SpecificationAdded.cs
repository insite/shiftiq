using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class SpecificationAdded : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SpecificationType Type { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ConsequenceType Consequence { get; set; }

        public Guid Specification { get; set; }
        public string Name { get; set; }
        public int Asset { get; set; }
        public int FormLimit { get; set; }
        public int QuestionLimit { get; set; }

        public ScoreCalculation Calculation { get; set; }

        public SpecificationAdded(SpecificationType type, ConsequenceType consequence, Guid spec, string name, int asset, int formLimit, int questionLimit, ScoreCalculation calculation)
        {
            Type = type;
            Consequence = consequence;
            Specification = spec;
            Name = name;
            Asset = asset;
            FormLimit = formLimit;
            QuestionLimit = questionLimit;
            Calculation = calculation;
        }
    }
}
