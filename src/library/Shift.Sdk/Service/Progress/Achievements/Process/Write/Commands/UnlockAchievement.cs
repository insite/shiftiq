using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class UnlockAchievement : Command
    {
        public bool Cascade { get; set; }

        public UnlockAchievement(Guid achievement, bool cascade = false)
        {
            AggregateIdentifier = achievement;
            Cascade = cascade;
        }
    }
}