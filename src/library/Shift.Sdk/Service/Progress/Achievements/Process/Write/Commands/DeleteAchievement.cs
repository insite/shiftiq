using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class DeleteAchievement : Command
    {
        public bool Cascade { get; set; }

        public DeleteAchievement(Guid achievement, bool cascade)
        {
            AggregateIdentifier = achievement;
            Cascade = cascade;
        }
    }
}