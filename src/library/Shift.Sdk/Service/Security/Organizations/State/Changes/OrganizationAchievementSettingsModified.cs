using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationAchievementSettingsModified : Change
    {
        public AchievementSettings Achievements { get; set; }

        public OrganizationAchievementSettingsModified(AchievementSettings achievements)
        {
            Achievements = achievements;
        }
    }
}
