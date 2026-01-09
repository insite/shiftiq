using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Sdk.UI;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class TaskGridEdit : TaskGridBase
    {
        public int AchievementCount { get; private set; }

        public void BindModelToControls(Guid programId)
        {
            var data = GetDataSource(programId);

            FolderRepeater.DataSource = data;
            FolderRepeater.DataBind();

            AchievementCount = data.Sum(x => x.Items.Length);
        }

        public void BindModelToControls(IEnumerable<Guid> achievementIds)
        {
            FolderRepeater.DataSource = GetDataSource(achievementIds);
            FolderRepeater.DataBind();
        }

        public List<AchievementItem> GetAchievements()
        {
            var achievements = new List<AchievementItem>();

            foreach (RepeaterItem achievementTypeItem in FolderRepeater.Items)
            {
                var achievementSettings = (Repeater)achievementTypeItem.FindControl("ItemRepeater");

                foreach (RepeaterItem settingItem in achievementSettings.Items)
                {
                    var achievement = new AchievementItem
                    {
                        AchievementIdentifier = Guid.Parse(((ITextControl)settingItem.FindControl("AchievementIdentifier")).Text),
                        LifetimeMonths = int.TryParse(((ITextBox)settingItem.FindControl("LifetimeMonths")).Text, out int temp) ? temp : (int?)null,
                        IsPlanned = ((ICheckBoxControl)settingItem.FindControl("IsPlanned")).Checked,
                        IsRequired = ((ICheckBoxControl)settingItem.FindControl("IsRequired")).Checked
                    };

                    achievements.Add(achievement);
                }
            }

            return achievements;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FolderRepeater.ItemDataBound += FolderRepeater_ItemDataBound;
        }

        private void FolderRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var achievement = (DataFolder)e.Item.DataItem;

            var repeater = (Repeater)e.Item.FindControl("ItemRepeater");
            repeater.DataSource = achievement.Items;
            repeater.DataBind();
        }
    }
}