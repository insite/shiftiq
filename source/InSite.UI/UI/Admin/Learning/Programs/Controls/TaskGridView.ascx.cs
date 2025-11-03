using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class TaskGridView : TaskGridBase
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

            var achievementSettings = (Repeater)e.Item.FindControl("ItemRepeater");
            achievementSettings.DataSource = achievement.Items;
            achievementSettings.DataBind();
        }

        protected string GetBoolString(string name)
        {
            var value = (bool)DataBinder.Eval(Page.GetDataItem(), name);

            return value ? "Yes" : string.Empty;
        }
    }
}