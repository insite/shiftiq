using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.CMDS.Common.Controls.User
{
    public partial class AchievementListGrid : BaseUserControl
    {
        private bool AllowSelect
        {
            get => (bool)(ViewState[nameof(AllowSelect)] ?? false);
            set => ViewState[nameof(AllowSelect)] = value;
        }

        internal void LoadData(List<AchievementListGridItem> items, bool allowSelect)
        {
            AllowSelect = allowSelect;

            Repeater.DataSource = items
                .Select(x => new
                {
                    x.AchievementIdentifier,
                    CategoryName = x.CategoryName.IfNullOrEmpty("No Category"),
                    Item = x
                })
                .GroupBy(x => new { x.AchievementIdentifier, x.CategoryName })
                .Select(g =>
                {
                    var row = g.First().Item;

                    return new
                    {
                        row.OrganizationName,
                        row.AchievementIdentifier,
                        row.AchievementLabel,
                        row.AchievementTitle,
                        CategoryNames = string.Join(", ",
                            g.Where(x => x.CategoryName.IsNotEmpty())
                             .Select(x => x.CategoryName)
                             .OrderBy(x => x))
                    };
                })
                .OrderBy(x => x.AchievementLabel)
                .ThenBy(x => x.AchievementTitle);

            Repeater.DataBind();
        }

        internal HashSet<Guid> GetSelectedAchievements()
        {
            var achievements = new HashSet<Guid>();

            foreach (RepeaterItem item in Repeater.Items)
            {
                var achievement = (ICheckBoxControl)item.FindControl("AchievementSelected");

                if (!achievement.Checked)
                    continue;

                var achievementIDControl = (HiddenField)item.FindControl("AchievementIdentifier");

                var achievementID = Guid.Parse(achievementIDControl.Value);

                achievements.Add(achievementID);
            }

            return achievements;
        }
    }
}