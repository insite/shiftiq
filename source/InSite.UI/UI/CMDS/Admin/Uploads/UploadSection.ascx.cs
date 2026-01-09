using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InSite.Cmds.Admin.Uploads.Controls
{
    public partial class UploadSection : UserControl
    {
        #region Classes

        public class CompetencyItem
        {
            public Guid ID { get; set; }
            public string Number { get; set; }
        }

        public class DownloadItem
        {
            public Guid ID { get; set; }
            public string Title { get; set; }
            public int? SizeInKilobytes { get; set; }
            public string Url { get; set; }
            public List<CompetencyItem> Competencies { get; set; }
        }

        public class AchievementItem
        {
            public Guid ID { get; set; }
            public string Number { get; set; }
            public string Title { get; set; }
            public List<DownloadItem> Downloads { get; set; }
            public AchievementItem() { Downloads = new List<DownloadItem>(); }
        }

        public class AchievementTypeItem
        {
            public string TitleValue { get; set; }
            public string TitleDisplay { get; set; }
            public List<AchievementItem> Achievements { get; set; }
            public AchievementTypeItem() { Achievements = new List<AchievementItem>(); }
        }

        public class AchievementCategoryItem
        {
            public string Title { get; set; }
            public List<List<AchievementTypeItem>> ListOfTypes { get; set; }
            public AchievementCategoryItem() { ListOfTypes = new List<List<AchievementTypeItem>>(); }
            public int AchievementCount => ListOfTypes.Sum(x => x.Sum(y => y.Achievements.Count));
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        public void LoadData(AchievementCategoryItem category)
        {
            ListOfAchievementTypes.ItemDataBound += ListOfAchievementTypes_ItemDataBound;

            ListOfAchievementTypes.DataSource = category.ListOfTypes;
            ListOfAchievementTypes.DataBind();
        }

        private void ListOfAchievementTypes_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            var achievementTypes = (IList<AchievementTypeItem>)((ListViewDataItem)e.Item).DataItem;
            var achievementTypesRepeater = (Repeater)e.Item.FindControl("AchievementTypes");

            achievementTypesRepeater.ItemDataBound += AchievementTypes_ItemDataBound;

            achievementTypesRepeater.DataSource = achievementTypes;
            achievementTypesRepeater.DataBind();
        }

        private void AchievementTypes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var achievementType = (AchievementTypeItem)e.Item.DataItem;
            var achievementsRepeater = (Repeater)e.Item.FindControl("Achievements");

            achievementsRepeater.ItemDataBound += Achievements_ItemDataBound;

            achievementsRepeater.DataSource = achievementType.Achievements;
            achievementsRepeater.DataBind();
        }

        private void Achievements_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var achievement = (AchievementItem)e.Item.DataItem;
            var downloadsRepeater = (Repeater)e.Item.FindControl("Downloads");

            downloadsRepeater.ItemDataBound += Downloads_ItemDataBound;

            downloadsRepeater.DataSource = achievement.Downloads;
            downloadsRepeater.DataBind();
        }

        private void Downloads_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var download = (DownloadItem)e.Item.DataItem;
            var competenciesRepeater = (Repeater)e.Item.FindControl("Competencies");

            competenciesRepeater.DataSource = download.Competencies;
            competenciesRepeater.DataBind();
        }
    }
}