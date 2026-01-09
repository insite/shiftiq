using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Cmds.Controls.Reporting.Report
{
    public partial class AchievementCriteriaSelector : UserControl
    {
        [Serializable]
        private class AchievementGroup
        {
            public string Label { get; set; }
            public FindEntity.DataItem[] Items { get; set; }
        }

        private AchievementGroup[] Achievements
        {
            get => (AchievementGroup[])ViewState[nameof(Achievements)];
            set => ViewState[nameof(Achievements)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemCreated += Repeater_ItemCreated;

            SelectAllButton.Click += SelectAllButton_Click;
            DeselectAllButton.Click += DeselectAllButton_Click;
        }

        private void Repeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var selector = (FindEntity)e.Item.FindControl("AchievementSelector");
            selector.NeedDataCount += AchievementSelector_NeedDataCount;
            selector.NeedDataSource += AchievementSelector_NeedDataSource;
            selector.NeedSelectedItems += AchievementSelector_NeedSelectedItems;
        }

        private void AchievementSelector_NeedDataCount(object sender, FindEntity.CountArgs args)
        {
            var index = ((RepeaterItem)((Control)sender).NamingContainer).ItemIndex;
            var items = Achievements[index].Items;

            args.Count = items.Length;
        }

        private void AchievementSelector_NeedDataSource(object sender, FindEntity.DataArgs args)
        {
            var index = ((RepeaterItem)((Control)sender).NamingContainer).ItemIndex;
            var items = Achievements[index].Items.AsQueryable();

            if (args.Keyword.IsNotEmpty())
                items = items.Where(x => x.Text.IndexOf(args.Keyword, StringComparison.OrdinalIgnoreCase) >= 0);

            items = items.ApplyPaging(args.Paging);

            args.Items = items.ToArray();
        }

        private void AchievementSelector_NeedSelectedItems(object sender, FindEntity.ItemsArgs args)
        {
            var index = ((RepeaterItem)((Control)sender).NamingContainer).ItemIndex;
            var items = Achievements[index].Items;

            args.Items = items.Where(x => args.Identifiers.Contains(x.Value)).ToArray();
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in Repeater.Items)
            {
                var selector = (FindEntity)item.FindControl("AchievementSelector");
                selector.Values = Achievements[item.ItemIndex].Items.Select(x => x.Value).ToArray();
            }
        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in Repeater.Items)
            {
                var selector = (FindEntity)item.FindControl("AchievementSelector");
                selector.Values = null;
            }
        }

        public bool LoadData(Guid[] departments, bool? isRequired)
        {
            var data = departments.IsNotEmpty()
                ? VCmdsCredentialSearch.SelectAchievementsByDepartment(departments, null, isRequired)
                : null;
            var hasData = data.IsNotEmpty();

            Achievements = hasData ? CreateGroups(data) : null;

            SelectAllButton.Visible = hasData;
            DeselectAllButton.Visible = hasData;

            Repeater.DataSource = Achievements;
            Repeater.DataBind();

            return hasData;
        }

        private static AchievementGroup[] CreateGroups(List<VCmdsAchievement> table)
        {
            var organization = CurrentSessionState.Identity.Organization;
            var labels = ServiceLocator.AchievementSearch.GetAchievementLabels(organization.Identifier);
            var achievementLabels = VCmdsAchievementSearch
                .SelectAchievementLabels(organization.Code, labels, null).Items
                .ToDictionary(x => x.Value, x => x.Text, StringComparer.OrdinalIgnoreCase);

            return table
                .OrderBy(x => x.AchievementLabel)
                .GroupBy(x => x.AchievementLabel)
                .Select(g => new AchievementGroup
                {
                    Label = achievementLabels.ContainsKey(g.Key) ? achievementLabels[g.Key] : g.Key,
                    Items = g
                        .OrderBy(a => a.AchievementTitle)
                        .Select(a => new FindEntity.DataItem
                        {
                            Value = a.AchievementIdentifier,
                            Text = a.AchievementTitle
                        })
                        .ToArray()
                })
                .ToArray();
        }

        public Guid[] GetSelectedAchievements()
        {
            return Repeater.Items.Cast<RepeaterItem>()
                .Select(x => (FindEntity)x.FindControl("AchievementSelector"))
                .SelectMany(x => x.Values)
                .ToArray();
        }

        public bool HasValue()
        {
            return Repeater.Items.Cast<RepeaterItem>()
                .Select(x => (FindEntity)x.FindControl("AchievementSelector"))
                .Any(x => x.HasValue);
        }
    }
}