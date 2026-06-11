using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationAchievementSettings : Command, IHasRun
    {
        public AchievementSettings Achievements { get; set; }

        public ModifyOrganizationAchievementSettings(Guid organizationId, AchievementSettings achievements)
        {
            AggregateIdentifier = organizationId;
            Achievements = achievements;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Achievements.IsShallowEqual(Achievements))
                return true;

            aggregate.Apply(new OrganizationAchievementSettingsModified(Achievements));

            return true;
        }
    }
}
