using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class ChangeAchievementOrganization : Command
    {
        public Guid Organization { get; set; }

        public ChangeAchievementOrganization(Guid achievement, Guid organization)
        {
            AggregateIdentifier = achievement;
            Organization = organization;
        }
    }
}
