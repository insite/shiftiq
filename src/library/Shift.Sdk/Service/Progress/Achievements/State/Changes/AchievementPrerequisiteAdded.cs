using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementPrerequisiteAdded : Change
    {
        public AchievementPrerequisiteAdded(Guid prerequisite, Guid[] conditions)
        {
            Prerequisite = prerequisite;
            Conditions = conditions;
        }

        public Guid Prerequisite { get; set; }
        public Guid[] Conditions { get; set; }
    }
}
