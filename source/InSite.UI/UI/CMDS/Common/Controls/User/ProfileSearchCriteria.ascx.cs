using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant.CMDS;

namespace InSite.Cmds.Controls.Profiles.Profiles
{
    public partial class ProfileSearchCriteria : SearchCriteriaController<ProfileFilter>
    {
        public override ProfileFilter Filter
        {
            get
            {
                var filter = new ProfileFilter
                {
                    ProfileNumber = Number.Text,
                    ProfileTitle = Title.Text,
                    ProfileDescription = Description.Text,
                    ProfileVisibility = AccountScope.Value,
                    ProfileOrganizationIdentifier = CompanyPanel.Visible ? Company.Value : null,
                    ParentProfileStandardIdentifier = ParentProfilePanel.Visible ? ParentProfile.Value : null,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Number.Text = value.ProfileNumber;
                Title.Text = value.ProfileTitle;
                Description.Text = value.ProfileDescription;
                AccountScope.Value = value.ProfileVisibility;
                Company.Value = value.ProfileOrganizationIdentifier;
                ParentProfile.Value = value.ParentProfileStandardIdentifier;

                InitVisibility();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AccountScope.AutoPostBack = true;
            AccountScope.ValueChanged += (s, a) => InitVisibility();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            ParentProfile.Filter.IsHierarchySelect = true;

            InitVisibility();
        }

        public override void Clear()
        {
            Number.Text = null;
            Title.Text = null;
            Description.Text = null;
            AccountScope.ClearSelection();
            Company.Value = null;
            ParentProfile.Value = null;

            InitVisibility();
        }

        private void InitVisibility()
        {
            switch (AccountScope.Value)
            {
                case AccountScopes.Organization:
                    CompanyPanel.Visible = true;
                    ParentProfilePanel.Visible = true;
                    break;
                default:
                    CompanyPanel.Visible = false;
                    ParentProfilePanel.Visible = false;
                    break;
            }

            LoadParentProfiles();
        }

        private void LoadParentProfiles()
        {
            if (AccountScope.Value == AccountScopes.Organization)
                return;

            ParentProfile.Filter.ProfileOrganizationIdentifier = null;
            ParentProfile.Value = null;
        }
    }
}