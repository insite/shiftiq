using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class ChangeAchievementType : Command
    {
        public string Type { get; private set; }

        public ChangeAchievementType(Guid achievement, string type)
        {
            AggregateIdentifier = achievement;
            Type = type;
        }
    }
}
