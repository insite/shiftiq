using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Achievements.Write
{
    public class ChangeAchievementExpiry : Command
    {
        public Expiration Expiration { get; set; }
        public bool Cascade { get; set; }

        public ChangeAchievementExpiry(Guid achievement, Expiration expiration, bool cascade = false)
        {
            AggregateIdentifier = achievement;
            Expiration = expiration;
            Cascade = cascade;
        }
    }
}