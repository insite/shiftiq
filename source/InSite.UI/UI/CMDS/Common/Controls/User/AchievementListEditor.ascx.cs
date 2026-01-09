using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.UI.CMDS.Common.Controls.User
{
    public partial class AchievementListEditor : BaseUserControl
    {
        #region Delegates

        public List<AchievementListGridItem> GetMatchingAchievements(Guid enterpriseId, Guid organizationId, string scope, string keyword, Guid? departmentId = null)
        {
            return VCmdsAchievementSearch.SelectOrganizationAchievements(enterpriseId, organizationId, scope, keyword, departmentId);
        }

        public delegate List<AchievementListGridItem> GetAssignedAchievements(List<AchievementListGridItem> list);

        public delegate void DeleteAchievements(IEnumerable<Guid> achievements);

        public delegate int InsertAchievements(IEnumerable<Guid> achievements);

        #endregion

        #region AchievementGroup class


        #endregion

        #region Fields

        private Guid _selectedOrganizationId;
        private GetAssignedAchievements _getAssignedAchievements;
        private DeleteAchievements _deleteAchievements;
        private InsertAchievements _insertAchievements;

        #endregion

        #region Events

        public event IntValueHandler Refreshed;
        private void OnRefreshed(int count) =>
            Refreshed?.Invoke(this, new IntValueArgs(count));

        #endregion

        #region Properties

        private bool AllowSelect
        {
            get => (bool)(ViewState[nameof(AllowSelect)] ?? false);
            set => ViewState[nameof(AllowSelect)] = value;
        }

        private Guid? DepartmentId
        {
            get => ViewState[nameof(DepartmentId)] as Guid?;
            set => ViewState[nameof(DepartmentId)] = value;
        }

        private string EntityName
        {
            get => ViewState[nameof(EntityName)] as string;
            set => ViewState[nameof(EntityName)] = value;
        }

        private bool IsEditable
        {
            get => (bool)(ViewState[nameof(IsEditable)] ?? false);
            set => ViewState[nameof(IsEditable)] = value;
        }

        private GroupByEnum GroupBy
        {
            get => GroupByComboBox.Value.ToEnum<GroupByEnum>();
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupByComboBox.AutoPostBack = true;
            GroupByComboBox.ValueChanged += GroupByComboBox_ValueChanged;

            AchievementGroups.Delete += AchievementGroups_Delete;
            NewAchievementGroups.Add += NewAchievementGroups_Add;

            DeleteAchievementButton.Click += DeleteAchievementButton_Click;
            AddAchievementButton.Click += AddAchievementButton_Click;

            FilterButton.Click += FilterButton_Click;
            ClearButton.Click += ClearButton_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            string achievementId, newAchievementId;

            if (IsGroupingEnabled())
            {
                achievementId = AchievementGroups.ClientID;
                newAchievementId = NewAchievementGroups.ClientID;
            }
            else
            {
                achievementId = AchievementGrid.ClientID;
                newAchievementId = NewAchievementGrid.ClientID;
            }

            SelectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', true);", achievementId);
            UnselectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', false);", achievementId);

            DeleteAchievementButton.OnClientClick = "return confirm('Are you sure you want to delete selected achievements?');";

            SelectAllButton2.OnClientClick = string.Format("return setCheckboxes('{0}', true);", newAchievementId);
            UnselectAllButton2.OnClientClick = string.Format("return setCheckboxes('{0}', false);", newAchievementId);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void GroupByComboBox_ValueChanged(object sender, EventArgs e)
        {
            LoadAchievements(GroupBy, DepartmentId);
        }

        private void AchievementGroups_Delete(object sender, AchievementListGroup.CommandArgs args)
        {
            var achievements = AchievementGroups.GetSelectedAchievements(args.VisibilityName, args.GroupName);

            _deleteAchievements(achievements);

            var count = LoadAchievements(GroupBy, DepartmentId);
            OnRefreshed(count);
        }

        private void NewAchievementGroups_Add(object sender, AchievementListGroup.CommandArgs args)
        {
            var achievements = NewAchievementGroups.GetSelectedAchievements(args.VisibilityName, args.GroupName);

            _insertAchievements(achievements);

            var count = LoadAchievements(GroupBy, DepartmentId);
            OnRefreshed(count);
        }

        private void DeleteAchievementButton_Click(object sender, EventArgs e)
        {
            var achievements = IsGroupingEnabled()
                ? AchievementGroups.GetSelectedAchievements()
                : AchievementGrid.GetSelectedAchievements();

            _deleteAchievements(achievements);

            var count = LoadAchievements(GroupBy, DepartmentId);

            OnRefreshed(count);
        }

        private void AddAchievementButton_Click(object sender, EventArgs e)
        {
            var achievements = IsGroupingEnabled()
                ? NewAchievementGroups.GetSelectedAchievements()
                : NewAchievementGrid.GetSelectedAchievements();

            _insertAchievements(achievements);

            var count = LoadAchievements(GroupBy, DepartmentId);

            OnRefreshed(count);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            LoadAchievements(GroupBy, DepartmentId);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            SearchText.Text = null;

            LoadAchievements(GroupBy, DepartmentId);
        }

        #endregion

        #region Public methods

        public void InitDelegates(
            Guid selectedOrganizationId,
            GetAssignedAchievements getAssignedAchievements,
            DeleteAchievements deleteAchievements,
            InsertAchievements insertAchievements,
            string entityName)
        {
            _selectedOrganizationId = selectedOrganizationId;

            _getAssignedAchievements = getAssignedAchievements;
            _deleteAchievements = deleteAchievements;
            _insertAchievements = insertAchievements;

            EntityName = entityName;
        }

        public int LoadAchievements(GroupByEnum? groupBy = GroupByEnum.Type, Guid? departmentId = null)
        {
            GroupByComboBox.Value = groupBy.GetName();

            DepartmentId = departmentId;

            Visibility.Toggle(ButtonsPanel, AllowSelect);

            Visibility.Toggle(DeleteAchievementButton, IsEditable);

            AchievementTab.Visible = AllowSelect;

            NewAchievementTab.Visible = IsEditable;

            var scope = SelectedAccountScope();

            var enterpriseId = ServiceLocator.AppSettings.Application.Organizations.Global;

            var organizationId = Organization.Identifier;

            var keyword = SearchText.Text;

            var matchingAchievements = GetMatchingAchievements(enterpriseId, organizationId, scope, keyword, DepartmentId);

            var hasMatchingAchievements = matchingAchievements.Count > 0;

            var assignedAchievements = _getAssignedAchievements != null
                ? _getAssignedAchievements(matchingAchievements)
                : new List<AchievementListGridItem>();

            var hasAssignedAchievements = assignedAchievements.Count > 0;

            var unassignedAchievements = matchingAchievements
                .Where(x => !assignedAchievements
                    .Select(y => y.AchievementIdentifier)
                    .Contains(x.AchievementIdentifier))
                .ToList();

            var hasUnassignedAchievements = unassignedAchievements.Count > 0;

            var isGrouping = IsGroupingEnabled();

            if (isGrouping)
                AchievementGroups.LoadData(assignedAchievements, GroupBy, AllowSelect, IsEditable);
            else
                AchievementGrid.LoadData(assignedAchievements, AllowSelect);

            Visibility.Toggle(AchievementCount, hasMatchingAchievements);

            Visibility.Toggle(AchievementNoMatch, !hasAssignedAchievements);
            Visibility.Toggle(AchievementGroups, hasAssignedAchievements && isGrouping);
            Visibility.Toggle(AchievementGrid, hasAssignedAchievements && !isGrouping);
            Visibility.Toggle(ButtonsPanel, hasAssignedAchievements);

            Visibility.Toggle(NewAchievementNoMatch, !hasUnassignedAchievements);
            Visibility.Toggle(NewAchievementGroups, hasUnassignedAchievements && isGrouping);
            Visibility.Toggle(NewAchievementGrid, hasUnassignedAchievements && !isGrouping);
            Visibility.Toggle(NewButtonsPanel, hasUnassignedAchievements);

            if (isGrouping)
                NewAchievementGroups.LoadData(unassignedAchievements, GroupBy, AllowSelect, IsEditable);
            else
                NewAchievementGrid.LoadData(unassignedAchievements, AllowSelect);

            Visibility.Toggle(NewAchievementCount, hasUnassignedAchievements);

            AchievementCount.InnerHtml = assignedAchievements.Count > 0
                ? $"Found {assignedAchievements.Count:n0} achievements assigned"
                : "No achievements found";

            NewAchievementCount.InnerHtml = unassignedAchievements.Count > 0
                ? $"Found {unassignedAchievements.Count:n0} achievements available. <span class='fs-sm'>Select the achievements you want to assign to this {EntityName}. Check the box next to each one, and click the Add button.</span>"
                : "No achievements found";

            return assignedAchievements.Count;
        }

        private string SelectedAccountScope()
        {
            var scope = AccountScopes.Organization;

            if (Visibility.IsHidden(AccountScope) || AccountScope.Value == AccountScopes.Partition)
                scope = AccountScopes.Partition;

            else if (AccountScope.Value == AccountScopes.Enterprise)
                scope = AccountScopes.Enterprise;

            return scope;
        }

        public void SetEditable(bool isEditable, bool allowSelect)
        {
            AllowSelect = allowSelect;
            IsEditable = isEditable;
        }

        public void ShowVisibilityCriteria()
        {
            Visibility.Show(AccountScope);
        }

        public HashSet<Guid> GetSelectedAchievements()
        {
            return IsGroupingEnabled()
                ? AchievementGroups.GetSelectedAchievements()
                : AchievementGrid.GetSelectedAchievements();
        }

        #endregion

        #region Helper methods

        private bool IsGroupingEnabled() => GroupBy != GroupByEnum.None;

        #endregion
    }
}