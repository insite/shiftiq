using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class LockAchievement : Command
    {
        public bool Cascade { get; set; }

        public LockAchievement(Guid achievement, bool cascade = false)
        {
            AggregateIdentifier = achievement;
            Cascade = cascade;
        }
    }
}
