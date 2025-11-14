using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.Records.Programs
{
    public partial class TaskGrid : UserControl
    {
        public int AchievementCount { get; private set; }

        public void BindModelToControls(Guid programIdentifier)
        {
            var achievementTypeMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector.CreateAchievementLabelMapping(CurrentSessionState.Identity.Organization.Code);

            var achievements = TaskSearch
                .SelectByProgram(programIdentifier)
                .GroupBy(x => x.AchievementLabel)
                .Select(x => new
                {
                    AchievementLabel = achievementTypeMapping.GetOrDefault(x.Key, x.Key),
                    Items = x.Select(y => new
                    {
                        y.DepartmentIdentifier,
                        y.AchievementIdentifier,
                        y.AchievementTitle,
                        IsTimeSensitive = y.LifetimeMonths.HasValue,
                        y.LifetimeMonths,
                        y.IsPlanned,
                        y.IsRequired
                    })
                    .OrderBy(y => y.AchievementTitle)
                    .ToList()
                })
                .OrderBy(x => x.AchievementLabel)
                .ToList();

            AchievementTypes.DataSource = achievements;
            AchievementTypes.DataBind();

            AchievementCount = achievements.Sum(x => x.Items.Count);
        }

        public void BindModelToControls(IEnumerable<Guid> achievementIdentifiers)
        {
            var achievementTypeMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector.CreateAchievementLabelMapping(CurrentSessionState.Identity.Organization.Code);
            var achievements = VCmdsAchievementSearch.Select(x => achievementIdentifiers.Contains(x.AchievementIdentifier))
                .Select(x => new
                {
                    x.AchievementIdentifier,
                    x.AchievementLabel,
                    x.AchievementTitle
                })
                .ToList()
                .GroupBy(x => x.AchievementLabel)
                .Select(x => new
                {
                    AchievementLabel = achievementTypeMapping.GetOrDefault(x.Key, x.Key),
                    Items = x.Select(y => new
                    {
                        y.AchievementIdentifier,
                        y.AchievementTitle,
                        IsTimeSensitive = false,
                        LifetimeMonths = (int?)null,
                        IsPlanned = false,
                        IsRequired = false
                    })
                    .OrderBy(y => y.AchievementTitle)
                    .ToList()
                })
                .OrderBy(x => x.AchievementLabel)
                .ToList();

            AchievementTypes.DataSource = achievements;
            AchievementTypes.DataBind();
        }

        public List<AchievementItem> GetAchievements()
        {
            var achievements = new List<AchievementItem>();

            foreach (RepeaterItem achievementTypeItem in AchievementTypes.Items)
            {
                var achievementSettings = (Repeater)achievementTypeItem.FindControl("AchievementSettings");

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

            AchievementTypes.ItemDataBound += AchievementTypes_ItemDataBound;
        }

        private void AchievementTypes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            dynamic achievement = e.Item.DataItem;

            var achievementSettings = (Repeater)e.Item.FindControl("AchievementSettings");
            achievementSettings.DataSource = achievement.Items;
            achievementSettings.DataBind();
        }
    }
}