using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class EnableAchievementReporting : Command
    {
        public EnableAchievementReporting(Guid achievement)
        {
            AggregateIdentifier = achievement;
        }
    }
}