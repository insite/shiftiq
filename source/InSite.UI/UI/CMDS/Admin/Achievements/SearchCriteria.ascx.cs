using System;

using InSite.Common.Web.UI;
using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Cmds.Controls.Training.Achievements
{
    public partial class AchievementSearchCriteria : SearchCriteriaController<VCmdsAchievementFilter>
    {
        private Guid PartitionId => ServiceLocator.Partition.Identifier;

        public override VCmdsAchievementFilter Filter
        {
            get
            {
                var filter = new VCmdsAchievementFilter
                {
                    AchievementVisibility = AccountScopes.Organization,
                    OrganizationIdentifier = Organization.Identifier,

                    AchievementType = AchievementType.Value,
                    Title = Title.Text,
                    Description = Description.Text,

                    IsTimeSensitive = IsTimeSensitive.ValueAsBoolean,
                    AllowSelfDeclared = AllowSelfDeclaration.ValueAsBoolean,
                    CategoryIdentifier = Category.ValueAsGuid,

                    AchievementScope = AchievementScope.Value
                };

                ApplyScope(filter);

                return filter;
            }
            set
            {
                AchievementType.Value = value.AchievementType;
                Title.Text = value.Title;
                Description.Text = value.Description;

                IsTimeSensitive.ValueAsBoolean = value.IsTimeSensitive;
                AllowSelfDeclaration.ValueAsBoolean = value.AllowSelfDeclared;
                Category.ValueAsGuid = value.CategoryIdentifier;

                AchievementScope.Value = string.IsNullOrEmpty(value.AchievementScope)
                    ? OrganizationScopeSelector.ScopeOrganization
                    : value.AchievementScope;
            }
        }

        private void ApplyScope(VCmdsAchievementFilter filter)
        {
            var ownerIds = AchievementScope.ResolveOwnerOrganizationIdentifiers(Organization.Identifier, PartitionId);

            if (ownerIds == null)
                return; // current organization (default): leave filter unchanged

            filter.AchievementVisibility = null;
            filter.OrganizationIdentifier = null;
            filter.OwnerOrganizationIdentifiers = ownerIds;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            AchievementScope.Visible = Identity.IsOperator ||
                Identity.IsInRole(CmdsRole.SystemAdministrators);

            LoadCategories();
        }

        public override void Clear()
        {
            AchievementType.ClearSelection();
            Title.Text = null;
            Description.Text = null;

            IsTimeSensitive.ClearSelection();
            AllowSelfDeclaration.ClearSelection();
            AchievementScope.Value = OrganizationScopeSelector.ScopeOrganization;
            LoadCategories();
        }

        private void LoadCategories()
        {
            Category.ListFilter.OrganizationIdentifier = Organization.Identifier;
            Category.RefreshData();
        }
    }
}
