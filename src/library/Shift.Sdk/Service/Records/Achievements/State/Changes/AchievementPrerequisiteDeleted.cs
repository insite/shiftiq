using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementPrerequisiteDeleted : Change
    {
        public AchievementPrerequisiteDeleted(Guid prerequisite)
        {
            Prerequisite = prerequisite;
        }

        public Guid Prerequisite { get; set; }
    }
}
