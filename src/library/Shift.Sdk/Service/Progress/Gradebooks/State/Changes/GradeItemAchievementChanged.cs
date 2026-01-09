using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradeItemAchievementChanged : Change
    {
        public GradeItemAchievementChanged(Guid item, GradeItemAchievement achievement)
        {
            Item = item;
            Achievement = achievement;
        }

        public Guid Item { get; set; }
        public GradeItemAchievement Achievement { get; set; }
    }
}
