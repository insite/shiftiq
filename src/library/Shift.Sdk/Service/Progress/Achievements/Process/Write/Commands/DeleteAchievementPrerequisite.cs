using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class DeleteAchievementPrerequisite : Command
    {
        public DeleteAchievementPrerequisite(Guid achievement, Guid prerequisite)
        {
            AggregateIdentifier = achievement;
            Prerequisite = prerequisite;
        }

        public Guid Prerequisite { get; set; }
    }
}
