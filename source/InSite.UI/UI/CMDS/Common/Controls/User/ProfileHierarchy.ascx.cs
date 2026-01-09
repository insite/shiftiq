using System;
using System.Web.UI;

using InSite.Application.Standards.Read;
using InSite.Persistence;

using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Cmds.Controls.Profiles.Profiles
{
    public partial class ProfileHierarchy : UserControl
    {
        #region Properties

        public bool AllowToAddProfilesFromCompany { get; set; }

        public bool Enabled
        {
            get { return ViewState[nameof(Enabled)] == null ? false : (bool)ViewState[nameof(Enabled)]; }
            set { ViewState[nameof(Enabled)] = value; }
        }

        public bool IsMoveStarted
        {
            get { return ViewState[nameof(IsMoveStarted)] == null ? false : (bool)ViewState[nameof(IsMoveStarted)]; }
            set { ViewState[nameof(IsMoveStarted)] = value; }
        }

        public Guid OrganizationId
        {
            get
            {
                return ViewState[nameof(OrganizationId)] != null
                    ? (Guid)ViewState[nameof(OrganizationId)]
                    : CurrentSessionState.Identity.Organization.Identifier;
            }
            set { ViewState[nameof(OrganizationId)] = value; }
        }

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Visibility.AutoPostBack = true;
            Visibility.ValueChanged += Visibility_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            ParentProfile.Filter.IsHierarchySelect = true;

            InitVisibility();
        }

        #endregion

        #region Settings and getting input values

        public void SetInputValues(Standard info)
        {
            OrganizationId = info.OrganizationIdentifier;

            Visibility.Value = OrganizationId != OrganizationIdentifiers.CMDS
                ? AccountScopes.Organization
                : AccountScopes.Organization;

            InitVisibility();

            if (Visibility.Value == AccountScopes.Organization)
                Company.Value = info.OrganizationIdentifier;

            if (info.ParentStandardIdentifier.HasValue)
                ParentProfile.Value = info.ParentStandardIdentifier;

            ParentProfile.Filter.ExcludeProfileStandardIdentifier = info.StandardIdentifier;

            Visibility.Enabled = Enabled;
            Company.Enabled = Enabled;
            ParentProfile.Enabled = Enabled;
        }

        public void GetInputValues(QStandard info)
        {
            var isOrganizationScope = Visibility.Value == AccountScopes.Organization;

            info.OrganizationIdentifier = isOrganizationScope
                ? Company.Value.Value
                : OrganizationIdentifiers.CMDS;

            info.ParentStandardIdentifier = isOrganizationScope
                ? null
                : ParentProfile.Value;
        }

        #endregion

        #region Event handlers

        private void Visibility_ValueChanged(object sender, EventArgs e)
        {
            InitVisibility();
            ResetValidators();
        }

        #endregion

        #region Public methods

        public void SwitchToViewMode()
        {
            Visibility.Enabled = false;
            Company.Enabled = false;
            ParentProfile.Enabled = false;
        }

        public void SwitchToMoveMode(string visibilityLabel)
        {
            IsMoveStarted = true;
            Enabled = true;
            VisibilityLabel.InnerHtml = visibilityLabel;
            Visibility.Enabled = true;
            Company.Enabled = true;
            ParentField.Visible = false;
        }

        #endregion

        #region Helper methods

        private void ResetValidators()
        {
            CompanyRequired.IsValid = true;
        }

        private void InitVisibility()
        {
            switch (Visibility.Value)
            {
                case AccountScopes.Organization:
                    Company.Value = OrganizationId;
                    Company.Enabled = IsMoveStarted;
                    CompanyField.Visible = true;
                    ParentField.Visible = !IsMoveStarted;
                    break;

                default:
                    Company.Value = OrganizationId;
                    Company.Enabled = true;
                    CompanyField.Visible = false;
                    ParentField.Visible = false;
                    break;
            }

            LoadParentProfiles();
        }

        private void LoadParentProfiles()
        {
            if (AllowToAddProfilesFromCompany)
                ParentProfile.Filter.AddProfilesFromOrganizationIdentifier = Company.Value ?? Guid.Empty;

            if (Visibility.Value == AccountScopes.Organization)
                return;

            ParentProfile.Filter.ProfileOrganizationIdentifier = null;

            ParentProfile.Value = null;
        }

        public Guid GetSelectedOrganizationId()
        {
            return Company.Value ?? OrganizationId;
        }

        #endregion
    }
}