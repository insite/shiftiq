using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class AddAchievementPrerequisite : Command
    {
        public AddAchievementPrerequisite(Guid achievement, Guid prerequisite, Guid[] conditions)
        {
            AggregateIdentifier = achievement;
            Prerequisite = prerequisite;
            Conditions = conditions;
        }

        public Guid Prerequisite { get; set; }
        public Guid[] Conditions { get; set; }
    }
}
