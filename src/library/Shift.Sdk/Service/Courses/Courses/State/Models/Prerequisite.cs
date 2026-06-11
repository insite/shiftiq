using System;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class Prerequisite
    {
        public Guid Identifier { get; set; }
        public Guid TriggerIdentifier { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public TriggerType TriggerType { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public TriggerChange TriggerChange { get; set; }

        public int? TriggerConditionScoreFrom { get; set; }
        public int? TriggerConditionScoreThru { get; set; }

        public Prerequisite Clone()
        {
            return (Prerequisite)MemberwiseClone();
        }

        public bool Equal(Prerequisite p)
        {
            if (p == null)
                return false;

            return TriggerIdentifier == p.TriggerIdentifier
                && TriggerType == p.TriggerType
                && TriggerChange == p.TriggerChange
                && TriggerConditionScoreFrom == p.TriggerConditionScoreFrom
                && TriggerConditionScoreThru == p.TriggerConditionScoreThru
                ;
        }
    }
}
