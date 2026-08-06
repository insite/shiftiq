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
using Shift.Sdk.UI;

namespace InSite.Cmds.Controls.Reporting.Report
{
    /// <summary>
    /// The achievements the user asked for, resolved from the per-type selection modes. An empty
    /// <see cref="Achievements"/> array combined with <see cref="IsEveryTypeAll"/> means "every achievement", which
    /// the report queries express by applying no achievement filter at all.
    /// </summary>
    public sealed class AchievementSelection
    {
        public AchievementSelection(Guid[] achievements, bool isEveryTypeAll)
        {
            Achievements = achievements ?? new Guid[0];
            IsEveryTypeAll = isEveryTypeAll;
        }

        public Guid[] Achievements { get; }

        public bool IsEveryTypeAll { get; }

        public bool HasSelection => IsEveryTypeAll || Achievements.Length > 0;
    }

    public partial class AchievementCriteriaSelector : UserControl
    {
        private const string ModeNone = "None";
        private const string ModeAll = "All";
        private const string ModeSpecific = "Specific";

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

        /// <summary>
        /// When enabled, each achievement type gets a None/All/Specific mode selector and the picker is shown only
        /// for the types set to Specific. Off by default so the hosts that predate the modes keep the Select All and
        /// Clear All buttons.
        /// </summary>
        public bool EnableSelectionModes
        {
            get => (bool)(ViewState[nameof(EnableSelectionModes)] ?? false);
            set => ViewState[nameof(EnableSelectionModes)] = value;
        }

        public bool EnableSingleSelection
        {
            get => (bool)(ViewState[nameof(EnableSingleSelection)] ?? false);
            set => ViewState[nameof(EnableSingleSelection)] = value;
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

            var selector = GetSelector(e.Item);
            selector.NeedDataCount += AchievementSelector_NeedDataCount;
            selector.NeedDataSource += AchievementSelector_NeedDataSource;
            selector.NeedSelectedItems += AchievementSelector_NeedSelectedItems;

            if (EnableSingleSelection)
            {
                selector.MaxSelectionCount = 1;
                selector.AutoPostBack = true;
                selector.ValueChanged += AchievementSelector_ValueChanged;
            }

            var mode = (ComboBox)e.Item.FindControl("ModeSelector");
            mode.Visible = EnableSelectionModes && !EnableSingleSelection;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (EnableSingleSelection || !EnableSelectionModes)
                return;

            // Applied here rather than in the combo's ValueChanged handler so the picker is in the right state on the
            // first render too, not only after a mode is changed.
            foreach (RepeaterItem item in Repeater.Items)
            {
                var mode = GetMode(item);
                var selector = GetSelector(item);
                var isSpecific = mode == ModeSpecific;

                selector.Visible = isSpecific;

                if (!isSpecific)
                    selector.Values = null;
            }
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

        private void AchievementSelector_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var changed = (FindEntity)sender;

            foreach (RepeaterItem item in Repeater.Items)
            {
                var selector = GetSelector(item);
                if (selector != changed)
                    selector.Values = null;
            }
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in Repeater.Items)
            {
                var selector = GetSelector(item);
                selector.Values = Achievements[item.ItemIndex].Items.Select(x => x.Value).ToArray();
            }
        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in Repeater.Items)
            {
                var selector = GetSelector(item);
                selector.Values = null;
            }
        }

        public bool LoadData(Guid[] departments, bool? isRequired)
        {
            var data = departments.IsNotEmpty()
                ? VCmdsCredentialSearch.SelectAchievementsByDepartment(departments, null, isRequired)
                : null;

            return BindAchievements(data);
        }

        public bool LoadDataByOrganization(Guid organizationId, string[] categories, bool? isRequired)
        {
            var data = VCmdsCredentialSearch.SelectAchievementsByOrganization(organizationId, categories, isRequired);

            return BindAchievements(data);
        }

        private bool BindAchievements(List<VCmdsAchievement> data)
        {
            var hasData = data.IsNotEmpty();

            Achievements = hasData ? CreateGroups(data) : null;

            var showButtons = hasData && !EnableSelectionModes && !EnableSingleSelection;

            SelectAllButton.Visible = showButtons;
            DeselectAllButton.Visible = showButtons;

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
                .Select(x => GetSelector(x))
                .SelectMany(x => x.Values)
                .ToArray();
        }

        /// <summary>
        /// Resolves the per-type modes into the achievement identifiers to filter on. When every type is set to All
        /// the result is an empty array, which every report query reads as "no achievement filter" - the cheapest
        /// path through the query, since it skips the identifier list entirely.
        /// </summary>
        public AchievementSelection ResolveSelection()
        {
            if (EnableSingleSelection)
                return new AchievementSelection(GetSelectedAchievements().Take(1).ToArray(), false);

            if (!EnableSelectionModes)
                return new AchievementSelection(GetSelectedAchievements(), false);

            var items = Repeater.Items.Cast<RepeaterItem>().ToArray();
            if (items.Length == 0)
                return new AchievementSelection(new Guid[0], false);

            var isEveryTypeAll = items.All(x => GetMode(x) == ModeAll);
            if (isEveryTypeAll)
                return new AchievementSelection(new Guid[0], true);

            var identifiers = new List<Guid>();

            foreach (var item in items)
            {
                var mode = GetMode(item);

                if (mode == ModeNone)
                {
                    continue;
                }
                else if (mode == ModeAll)
                {
                    var group = Achievements[item.ItemIndex];
                    identifiers.AddRange(group.Items.Select(x => x.Value));
                }
                else if (mode == ModeSpecific)
                {
                    var selector = GetSelector(item);
                    identifiers.AddRange(selector.Values);
                }
            }

            return new AchievementSelection(identifiers.Distinct().ToArray(), false);
        }

        private static FindEntity GetSelector(RepeaterItem item) =>
            (FindEntity)item.FindControl("AchievementSelector");

        private static string GetMode(RepeaterItem item) =>
            ((ComboBox)item.FindControl("ModeSelector")).Value;

        public bool HasValue()
        {
            return Repeater.Items.Cast<RepeaterItem>()
                .Select(x => GetSelector(x))
                .Any(x => x.HasValue);
        }
    }
}
