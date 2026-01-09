using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.UI.CMDS.Common.Controls.User
{
    public partial class AchievementListGroup : BaseUserControl
    {
        #region Classes

        public enum ControlMode
        {
            Add,
            Delete
        }

        private class AchievementVisibility
        {
            public string AccountScope { get; set; }
            public IList<AchievementGroup> Groups { get; set; }
        }

        private class AchievementGroup
        {
            public string AccountScope { get; set; }
            public string GroupName { get; set; }
            public IList<AchievementCategory> Categories { get; set; }

            public string VisibilityLabel
            {
                get
                {
                    if (StringHelper.Equals(AccountScope, "Organization"))
                        return "<span class='badge bg-info'>Organization</span>";

                    if (StringHelper.Equals(AccountScope, "Root"))
                        return "<span class='badge bg-custom-default'>Global</span>";

                    return string.Empty;
                }
            }
        }

        private class AchievementCategory
        {
            public string CategoryName { get; set; }
            public bool HasCategory { get; set; }
            public IList<AchievementListGridItem> Achievements { get; set; }

            public int RowCount
            {
                get
                {
                    var rowCount = Achievements.Count / 2;

                    if (Achievements.Count % 2 != 0)
                        rowCount++;

                    if (!string.IsNullOrEmpty(CategoryName))
                        rowCount++;

                    return rowCount;
                }
            }
        }

        #endregion

        #region Events

        public class CommandArgs : EventArgs
        {
            public string VisibilityName { get; }
            public string GroupName { get; }

            internal CommandArgs(string args)
            {
                var parts = args.Split(';');
                VisibilityName = parts[0];
                GroupName = parts[1];
            }
        }

        public delegate void CommandHandler(object sender, CommandArgs args);

        public event CommandHandler Add;
        private void OnAdd(string args) => Add?.Invoke(this, new CommandArgs(args));

        public event CommandHandler Delete;
        private void OnDelete(string args) => Delete?.Invoke(this, new CommandArgs(args));

        #endregion

        #region Properties

        public ControlMode Mode
        {
            get => (ControlMode)ViewState[nameof(Mode)];
            set => ViewState[nameof(Mode)] = value;
        }

        private bool AllowSelect
        {
            get => (bool)(ViewState[nameof(AllowSelect)] ?? false);
            set => ViewState[nameof(AllowSelect)] = value;
        }

        private bool IsEditable
        {
            get => (bool)(ViewState[nameof(IsEditable)] ?? false);
            set => ViewState[nameof(IsEditable)] = value;
        }

        #endregion

        #region Intialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonScript.ContentKey = typeof(AchievementListGroup).FullName;

            VisibilityRepeater.ItemCreated += VisibilityRepeater_ItemCreated;
            VisibilityRepeater.ItemDataBound += VisibilityRepeater_ItemDataBound;
        }

        #endregion

        #region Data binding

        public void LoadData(List<AchievementListGridItem> items, GroupByEnum groupBy, bool allowSelect, bool isEditable)
        {
            AllowSelect = allowSelect;
            IsEditable = isEditable;

            VisibilityRepeater.DataSource = CreateVisibilities(items, groupBy);
            VisibilityRepeater.DataBind();
        }

        private IList<AchievementVisibility> CreateVisibilities(List<AchievementListGridItem> items, GroupByEnum groupBy)
        {
            var isGroupByType = groupBy == GroupByEnum.Type;
            var isGropByTypeCategory = groupBy == GroupByEnum.TypeAndCategory;

            items = items.OrderBy(x => x.AchievementLabel).ToList();

            if (isGropByTypeCategory)
                items = items.OrderBy(x => x.CategoryName).ToList();

            items = items.OrderBy(x => x.AchievementTitle).ToList();

            var visibilities = CreateAchievementVisibilities(groupBy);
            var companySpecificGroupMap = new Dictionary<string, AchievementGroup>();
            var globalGroupMap = new Dictionary<string, AchievementGroup>();

            foreach (var item in items)
            {
                var achievementVisibility = item.Visibility;

                AchievementVisibility visibility;
                Dictionary<string, AchievementGroup> groupMap;

                if (achievementVisibility == AccountScopes.Organization || isGroupByType)
                {
                    visibility = visibilities[0];
                    groupMap = companySpecificGroupMap;
                }
                else
                {
                    visibility = visibilities[1];
                    groupMap = globalGroupMap;
                }

                string subType = item.AchievementLabel;

                if (subType == null)
                    continue;

                if (!groupMap.TryGetValue(subType.ToLower(), out var group))
                {
                    group = new AchievementGroup { GroupName = subType, AccountScope = visibility.AccountScope, Categories = new List<AchievementCategory>() };
                    visibility.Groups.Add(group);
                    groupMap.Add(subType.ToLower(), group);
                }

                var categories = group.Categories;

                var categoryName = isGropByTypeCategory ? item.CategoryName.IfNullOrEmpty("No Category") : string.Empty;

                if (categories.Count == 0 || !StringHelper.Equals(categories[categories.Count - 1].CategoryName, categoryName))
                {
                    var category = new AchievementCategory
                    {
                        CategoryName = categoryName,
                        HasCategory = isGropByTypeCategory,
                        Achievements = new List<AchievementListGridItem>()
                    };

                    categories.Add(category);
                }

                categories[categories.Count - 1].Achievements.Add(item);
            }

            for (var i = visibilities.Count - 1; i >= 0; i--)
                if (visibilities[i].Groups.Count == 0)
                    visibilities.RemoveAt(i);

            return visibilities;
        }

        private List<AchievementVisibility> CreateAchievementVisibilities(GroupByEnum groupBy)
        {
            var visibilities = new List<AchievementVisibility>();

            if (groupBy != GroupByEnum.Type)
            {
                visibilities.Add(new AchievementVisibility());
                visibilities[0].AccountScope = "ORGANIZATION-SPECIFIC";
                visibilities[0].Groups = new List<AchievementGroup>();

                visibilities.Add(new AchievementVisibility());
                visibilities[1].AccountScope = "GLOBAL";
                visibilities[1].Groups = new List<AchievementGroup>();
            }
            else
            {
                visibilities.Add(new AchievementVisibility());
                visibilities[0].AccountScope = null;
                visibilities[0].Groups = new List<AchievementGroup>();
            }

            return visibilities;
        }

        #endregion

        #region Event handlers

        private void VisibilityRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var achievementGroups = (Repeater)e.Item.FindControl("AchievementGroups");
            achievementGroups.ItemCreated += AchievementGroups_ItemCreated;
            achievementGroups.ItemCommand += AchievementGroups_ItemCommand;
            achievementGroups.ItemDataBound += AchievementGroups_ItemDataBound;
        }

        private void VisibilityRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var visibility = (AchievementVisibility)e.Item.DataItem;

            var achievementGroups = (Repeater)e.Item.FindControl("AchievementGroups");
            achievementGroups.DataSource = visibility.Groups;
            achievementGroups.DataBind();
        }

        private void AchievementGroups_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var achievementCategories = (Repeater)e.Item.FindControl("AchievementCategories");
            achievementCategories.ItemDataBound += AchievementCategories_ItemDataBound;
        }

        private void AchievementGroups_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "GroupAddAchievement")
                OnAdd((string)e.CommandArgument);
            else if (e.CommandName == "GroupDeleteAchievement")
                OnDelete((string)e.CommandArgument);
        }

        private void AchievementGroups_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var group = (AchievementGroup)e.Item.DataItem;

            var achievementCategories = (Repeater)e.Item.FindControl("AchievementCategories");
            achievementCategories.DataSource = group.Categories;
            achievementCategories.DataBind();

            var groupCheckBox = e.Item.FindControl("GroupCheckBox");
            Visibility.Toggle(groupCheckBox, AllowSelect);

            var showAddButton = Mode == ControlMode.Add && AllowSelect && IsEditable;

            var groupAddButton = (InSite.Common.Web.UI.Button)e.Item.FindControl("GroupAddButton");
            Visibility.Toggle(groupAddButton, showAddButton);
            groupAddButton.CommandArgument = $"{group.AccountScope};{group.GroupName}";

            var showDeleteButton = Mode == ControlMode.Delete && AllowSelect && IsEditable;

            var groupDeleteButton = (InSite.Common.Web.UI.Button)e.Item.FindControl("GroupDeleteButton");
            Visibility.Toggle(groupDeleteButton, showDeleteButton);
            groupDeleteButton.CommandArgument = $"{group.AccountScope};{group.GroupName}";

            var scriptManager = ScriptManager.GetCurrent(Page);
            if (showAddButton)
                scriptManager.RegisterPostBackControl(groupAddButton);
            if (showDeleteButton)
                scriptManager.RegisterPostBackControl(groupDeleteButton);
        }

        private void AchievementCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var category = (AchievementCategory)e.Item.DataItem;
            var achievements = (Repeater)e.Item.FindControl("Achievements");

            achievements.ItemDataBound += Achievements_ItemDataBound;
            achievements.DataSource = category.Achievements;
            achievements.DataBind();
        }

        private void Achievements_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (AchievementListGridItem)e.Item.DataItem;

            var achievementSelected = (InSite.Common.Web.UI.CheckBox)e.Item.FindControl("AchievementSelected");

            if (Mode == ControlMode.Add)
            {
                achievementSelected.RenderMode = CheckBoxRenderMode.Input;

                var achievementLabel = (ITextControl)e.Item.FindControl("AchievementLabel");
                achievementLabel.Text = $"<a href='/ui/cmds/admin/achievements/edit?id={item.AchievementIdentifier}' class=\"fs-sm\">{item.AchievementTitle}</a>";
            }
            else if (Mode == ControlMode.Delete)
            {
                achievementSelected.Text = item.AchievementTitle;
            }
        }

        #endregion

        #region Helper methods

        protected static string GetDisplay(string achievementType)
        {
            return AchievementTypes.Pluralize(achievementType, CurrentSessionState.Identity.Organization.Code);
        }

        public HashSet<Guid> GetSelectedAchievements(string visibilityName, string groupName)
        {
            var achievements = new HashSet<Guid>();

            foreach (RepeaterItem visibilityItem in VisibilityRepeater.Items)
            {
                if (!string.IsNullOrEmpty(visibilityName))
                {
                    var visibilityNameCtrl = (ITextControl)visibilityItem.FindControl("VisibilityName");
                    if (visibilityNameCtrl != null && !StringHelper.Equals(visibilityNameCtrl.Text, visibilityName))
                        continue;
                }

                var achievementeGroups = (Repeater)visibilityItem.FindControl("AchievementGroups");

                foreach (RepeaterItem groupItem in achievementeGroups.Items)
                {
                    var groupNameCtrl = (ITextControl)groupItem.FindControl("GroupName");
                    if (!string.Equals(groupNameCtrl.Text, GetDisplay(groupName)))
                        continue;

                    var achievementCategories = (Repeater)groupItem.FindControl("AchievementCategories");

                    foreach (RepeaterItem categoryItem in achievementCategories.Items)
                    {
                        var achievementsRepeater = (Repeater)categoryItem.FindControl("Achievements");

                        foreach (RepeaterItem item in achievementsRepeater.Items)
                        {
                            var achievement = (ICheckBoxControl)item.FindControl("AchievementSelected");
                            if (!achievement.Checked)
                                continue;

                            var achievementIDControl = (ITextControl)item.FindControl("AchievementIdentifier");
                            var achievementID = Guid.Parse(achievementIDControl.Text);

                            achievements.Add(achievementID);
                        }
                    }
                }
            }

            return achievements;
        }

        public HashSet<Guid> GetSelectedAchievements()
        {
            var achievements = new HashSet<Guid>();

            foreach (RepeaterItem visibilityItem in VisibilityRepeater.Items)
            {
                var achievementGroups = (Repeater)visibilityItem.FindControl("AchievementGroups");

                foreach (RepeaterItem groupItem in achievementGroups.Items)
                {
                    var achievementCategories = (Repeater)groupItem.FindControl("AchievementCategories");

                    foreach (RepeaterItem categoryItem in achievementCategories.Items)
                    {
                        var achievementsRepeater = (Repeater)categoryItem.FindControl("Achievements");

                        foreach (RepeaterItem item in achievementsRepeater.Items)
                        {
                            var achievement = (ICheckBoxControl)item.FindControl("AchievementSelected");
                            if (!achievement.Checked)
                                continue;

                            var achievementIDControl = (ITextControl)item.FindControl("AchievementIdentifier");
                            var achievementID = Guid.Parse(achievementIDControl.Text);

                            achievements.Add(achievementID);
                        }
                    }
                }
            }

            return achievements;
        }

        #endregion
    }
}