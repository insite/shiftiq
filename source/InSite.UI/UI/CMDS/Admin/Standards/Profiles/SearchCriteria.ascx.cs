using System;

using InSite.Common.Web.UI;
using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Cmds.Controls.Profiles.Profiles
{
    public partial class ProfileSearchCriteria : SearchCriteriaController<ProfileFilter>
    {
        private Guid PartitionId => ServiceLocator.Partition.Identifier;

        public override ProfileFilter Filter
        {
            get
            {
                var filter = new ProfileFilter
                {
                    ProfileVisibility = AccountScopes.Organization,
                    ProfileOrganizationIdentifier = Organization.Identifier,

                    ProfileNumber = Number.Text,
                    ProfileTitle = Title.Text,
                    ProfileDescription = Description.Text,

                    ParentProfileStandardIdentifier = ParentProfile.Value,

                    ProfileScope = ProfileScope.Value
                };

                ApplyScope(filter);

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Number.Text = value.ProfileNumber;
                Title.Text = value.ProfileTitle;
                Description.Text = value.ProfileDescription;

                ParentProfile.Value = value.ParentProfileStandardIdentifier;

                ProfileScope.Value = string.IsNullOrEmpty(value.ProfileScope)
                    ? OrganizationScopeSelector.ScopeOrganization
                    : value.ProfileScope;
            }
        }

        private void ApplyScope(ProfileFilter filter)
        {
            var ownerIds = ProfileScope.ResolveOwnerOrganizationIdentifiers(Organization.Identifier, PartitionId);

            if (ownerIds == null)
                return; // current organization (default): leave filter unchanged

            filter.ProfileVisibility = null;
            filter.ProfileOrganizationIdentifier = null;
            filter.OwnerOrganizationIdentifiers = ownerIds;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            ProfileScope.Visible = Identity.IsOperator ||
                Identity.IsInRole(CmdsRole.SystemAdministrators);

            ParentProfile.Filter.IsHierarchySelect = true;

            LoadParentProfiles();
        }

        public override void Clear()
        {
            Number.Text = null;
            Title.Text = null;
            Description.Text = null;

            ParentProfile.Value = null;
            ProfileScope.Value = OrganizationScopeSelector.ScopeOrganization;

            LoadParentProfiles();
        }

        private void LoadParentProfiles()
        {
            ParentProfile.Filter.ProfileOrganizationIdentifier = Organization.Identifier;
        }
    }
}
