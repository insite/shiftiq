using System;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Organizations
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class AutomaticCompetencyExpiration
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public OrganizationExpirationType Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Month { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Day { get; set; }

        public bool IsEqual(AutomaticCompetencyExpiration other)
        {
            return Type == other.Type
                && Month == other.Month
                && Day == other.Day;
        }
    }
}