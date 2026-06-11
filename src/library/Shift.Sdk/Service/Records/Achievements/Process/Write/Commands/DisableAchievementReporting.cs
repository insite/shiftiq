using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class DisableAchievementReporting : Command
    {
        public DisableAchievementReporting(Guid achievement)
        {
            AggregateIdentifier = achievement;
        }
    }
}