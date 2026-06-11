using System;

using Newtonsoft.Json;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class GradebookSettings
    {
        public int? DefaultPassPercent { get; set; }
        public bool HideIgnoreScoreCheckbox { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int OperatorOnlyUnlockAfterMonths { get; set; } = 2;

        public bool IsEqual(GradebookSettings other)
        {
            return DefaultPassPercent == other.DefaultPassPercent
                && HideIgnoreScoreCheckbox == other.HideIgnoreScoreCheckbox
                && OperatorOnlyUnlockAfterMonths == other.OperatorOnlyUnlockAfterMonths;
        }
    }
}
