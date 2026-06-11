using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookAchievementChanged : Change
    {
        public GradebookAchievementChanged(Guid? achievement)
        {
            Achievement = achievement;
        }

        public Guid? Achievement { get; set; }
    }
}
