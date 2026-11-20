using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.CMDS.Common.Controls.User
{
    public partial class AchievementListEditor : BaseUserControl
    {
        #region Delegates

        public List<AchievementListGridItem> GetMatchingAchievements(Guid[] ownerOrganizationIdentifiers, string keyword, Guid? departmentId = null)
        {
            return VCmdsAchievementSearch.SelectOrganizationAchievements(ownerOrganizationIdentifiers, keyword, departmentId);
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

        /// <summary>
        /// The organization whose achievements the search offers, alongside the partition (Global)
        /// organization. Defaults to the signed-in organization; set it when the entity being
        /// edited belongs to a different organization.
        /// </summary>
        public Guid? OwnerOrganizationIdentifier
        {
            get => ViewState[nameof(OwnerOrganizationIdentifier)] as Guid?;
            set => ViewState[nameof(OwnerOrganizationIdentifier)] = value;
        }

        public string Keyword => SearchText.Text;

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

            var owners = SelectedOwnerOrganizationIdentifiers();

            var keyword = SearchText.Text;

            var matchingAchievements = GetMatchingAchievements(owners, keyword, DepartmentId);

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

            if (!hasAssignedAchievements)
                AchievementNoMatch.InnerText = GetAssignedEmptyMessage(keyword);

            if (!hasUnassignedAchievements)
                NewAchievementNoMatch.InnerText = GetUnassignedEmptyMessage(owners, keyword, hasMatchingAchievements);

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

        private string GetAssignedEmptyMessage(string keyword)
        {
            return string.IsNullOrEmpty(keyword)
                ? $"There are no achievements assigned to this {EntityName}."
                : "There are no assigned achievements matching your search criteria.";
        }

        /// <summary>
        /// Explains why nothing is available to add. An empty list is usually not the keyword's
        /// fault: everything may already be assigned, or the department behind the list may have
        /// nothing to offer in this scope.
        /// </summary>
        private string GetUnassignedEmptyMessage(Guid[] owners, string keyword, bool hasMatchingAchievements)
        {
            var hasKeyword = !string.IsNullOrEmpty(keyword);

            if (hasMatchingAchievements)
            {
                return hasKeyword
                    ? $"Every achievement matching your search criteria is already assigned to this {EntityName}."
                    : $"Every available achievement is already assigned to this {EntityName}.";
            }

            var isBroadestScope = SelectedScope() == OrganizationScopeSelector.ScopeBoth;
            var scopePhrase = isBroadestScope ? string.Empty : " in the selected scope";

            if (DepartmentId.HasValue)
            {
                // With no keyword the department itself is empty. With one, check whether the
                // department has anything at all before blaming the keyword.
                var isDepartmentEmpty = !hasKeyword;
                if (hasKeyword)
                {
                    var departmentAchievements = GetMatchingAchievements(owners, null, DepartmentId);
                    isDepartmentEmpty = departmentAchievements.Count == 0;
                }

                if (isDepartmentEmpty)
                {
                    var department = ServiceLocator.GroupSearch.GetGroup(DepartmentId.Value);
                    var subject = department != null
                        ? $"The department \"{department.GroupName}\""
                        : "The department";

                    return $"{subject} has no achievements{scopePhrase}. "
                        + $"Add achievements to the department first, then add them to this {EntityName}.";
                }
            }

            return hasKeyword
                ? "There are no achievements matching your search criteria."
                : $"There are no achievements{scopePhrase}.";
        }

        private string SelectedScope()
        {
            // When the scope selector is hidden, default to the broadest scope.
            return Visibility.IsHidden(AccountScope)
                ? OrganizationScopeSelector.ScopeBoth
                : AccountScope.Value;
        }

        private Guid[] SelectedOwnerOrganizationIdentifiers()
        {
            var organizationId = OwnerOrganizationIdentifier ?? Organization.Identifier;
            var partitionId = ServiceLocator.Partition.Identifier;

            var scope = SelectedScope();

            switch (scope)
            {
                case OrganizationScopeSelector.ScopePartition:
                    return new[] { partitionId };

                case OrganizationScopeSelector.ScopeBoth:
                    return new[] { organizationId, partitionId };

                default: // ScopeOrganization
                    return new[] { organizationId };
            }
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
