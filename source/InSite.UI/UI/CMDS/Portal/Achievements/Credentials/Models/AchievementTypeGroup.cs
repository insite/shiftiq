using System.Collections.Generic;
using System.Linq;

namespace InSite.UI.CMDS.Portal.Achievements.Credentials
{
    public class AchievementTypeGroup
    {
        public int AchievementItemsCount => AchievementCategoryGroups.Sum(x => x.AchievementItems.Count);

        public string AchievementType { get; set; }

        public List<AchievementCategoryGroup> AchievementCategoryGroups { get; }

        public AchievementTypeGroup()
        {
            AchievementCategoryGroups = new List<AchievementCategoryGroup>();
        }
    }
}