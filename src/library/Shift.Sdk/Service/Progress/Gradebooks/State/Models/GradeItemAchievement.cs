using System;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Records
{
    [Serializable]
    public class GradeItemAchievement
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public TriggerCauseChange WhenChange { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public TriggerCauseGrade WhenGrade { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public TriggerEffectCommand ThenCommand { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public TriggerEffectCommand ElseCommand { get; set; }

        public Guid Achievement { get; set; }
        public DateTimeOffset? AchievementFixedDate { get; set; }

        public GradeItemAchievement Clone()
        {
            return (GradeItemAchievement)MemberwiseClone();
        }

        public static bool Equals(GradeItemAchievement a, GradeItemAchievement b)
        {
            return (a == null && b == null)
                || (a != null && b != null
                    && a.WhenChange == b.WhenChange
                    && a.WhenGrade == b.WhenGrade
                    && a.ThenCommand == b.ThenCommand
                    && a.ElseCommand == b.ElseCommand
                    && a.Achievement == b.Achievement
                    && a.AchievementFixedDate == b.AchievementFixedDate
                );
        }
    }
}