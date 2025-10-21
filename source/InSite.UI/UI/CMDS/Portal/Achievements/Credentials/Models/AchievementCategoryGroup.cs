using System.Collections.Generic;

namespace InSite.UI.CMDS.Portal.Achievements.Credentials
{
    public class AchievementCategoryGroup
    {
        public string AchievementCategory { get; set; }

        public List<AchievementItem> AchievementItems { get; }

        public AchievementCategoryGroup()
        {
            AchievementItems = new List<AchievementItem>();
        }
    }
}