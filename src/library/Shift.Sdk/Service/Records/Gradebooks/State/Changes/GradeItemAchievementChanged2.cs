using System;

using Shift.Common.Timeline.Changes;
using Shift.Constant;

namespace InSite.Domain.Records
{
    public class GradeItemAchievementChanged2 : Change
    {
        #region Obsolete GradeItemAchievementChanged

        private class ObsoleteGradeItemAchievementChanged : Change
        {
            public enum TriggerCauseGrade_Old { None, Pass, Fail }

            public class GradeItemAchievement_Old
            {
                public TriggerCauseChange WhenChange { get; set; }
                public TriggerCauseGrade_Old WhenGrade { get; set; }
                public TriggerEffectCommand ThenCommand { get; set; }
                public TriggerEffectCommand ElseCommand { get; set; }
                public Guid Achievement { get; set; }
                public DateTimeOffset? AchievementFixedDate { get; set; }
            }

            public Guid Item { get; set; }
            public GradeItemAchievement_Old Achievement { get; set; }

            public ObsoleteGradeItemAchievementChanged(Guid item, GradeItemAchievement_Old achievement)
            {
                Item = item;
                Achievement = achievement;
            }
        }

        #endregion

        public const string ObsoleteChangeType = "GradeItemAchievementChanged";

        public Guid Item { get; set; }
        public GradeItemAchievement Achievement { get; set; }

        public GradeItemAchievementChanged2(Guid item, GradeItemAchievement achievement)
        {
            Item = item;
            Achievement = achievement;
        }

        public static GradeItemAchievementChanged2 Upgrade(SerializedChange serializedChange)
        {
            var v1 = serializedChange.Deserialize<ObsoleteGradeItemAchievementChanged>();
            var a1 = v1.Achievement;

            var a2 = a1 != null
                ? new GradeItemAchievement
                {
                    WhenChange = a1.WhenChange,
                    WhenPassCommand = a1.WhenGrade == ObsoleteGradeItemAchievementChanged.TriggerCauseGrade_Old.Pass ? a1.ThenCommand : a1.ElseCommand,
                    WhenFailCommand = a1.WhenGrade == ObsoleteGradeItemAchievementChanged.TriggerCauseGrade_Old.Fail ? a1.ThenCommand : a1.ElseCommand,
                    ElseCommand = a1.ElseCommand,
                    Achievement = a1.Achievement,
                    AchievementFixedDate = a1.AchievementFixedDate
                }
                : null;

            var v2 = new GradeItemAchievementChanged2(v1.Item, a2);

            return v2;
        }
    }
}
