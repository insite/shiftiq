using System;

using InSite.Application.Standards.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Cmds.Controls.Profiles.Profiles
{
    public partial class ProfileOwner : BaseUserControl
    {
        #region Properties

        public bool AllowToAddProfilesFromCompany { get; set; }

        /// <summary>
        /// Whether the "This Organization and Global" scope option is offered. Defaults to false.
        /// </summary>
        public bool AllowThisOrganizationAndGlobal { get; set; }

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

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // The combo binds its items during its own Init (before this one), so apply the
            // visibility of the combined item and rebuild the list.
            OrganizationScope.IsCombinedScopeVisible = AllowThisOrganizationAndGlobal;
            OrganizationScope.RefreshData();

            OrganizationScope.AutoPostBack = true;
            OrganizationScope.ValueChanged += OrganizationScope_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            ParentProfile.Filter.IsHierarchySelect = true;

            InitOrganizationScope();
        }

        #endregion

        #region Settings and getting input values

        public void SetInputValues(Standard info)
        {
            OrganizationScope.Value = AccountScopes.Organization;

            InitOrganizationScope();

            if (info.ParentStandardIdentifier.HasValue)
                ParentProfile.Value = info.ParentStandardIdentifier;

            ParentProfile.Filter.ExcludeProfileStandardIdentifier = info.StandardIdentifier;

            OrganizationScope.Enabled = Enabled;
            ParentProfile.Enabled = Enabled;
        }

        public void GetInputValues(QStandard info)
        {
            var isOrganizationScope = OrganizationScope.Value == AccountScopes.Organization;

            info.OrganizationIdentifier = isOrganizationScope
                ? Organization.Identifier
                : OrganizationIdentifiers.CMDS;

            info.ParentStandardIdentifier = ParentProfile.Value;
        }

        #endregion

        #region Event handlers

        private void OrganizationScope_ValueChanged(object sender, EventArgs e)
        {
            InitOrganizationScope();
        }

        #endregion

        #region Public methods

        public void SwitchToViewMode()
        {
            OrganizationScope.Enabled = false;
            ParentProfile.Enabled = false;
        }

        public void SwitchToMoveMode()
        {
            IsMoveStarted = true;
            Enabled = true;
            OrganizationScope.Enabled = true;
            ParentField.Visible = false;
        }

        #endregion

        #region Helper methods

        private void InitOrganizationScope()
        {
            switch (OrganizationScope.Value)
            {
                case AccountScopes.Organization:
                    ParentField.Visible = !IsMoveStarted;
                    break;

                default:
                    ParentField.Visible = false;
                    break;
            }

            LoadParentProfiles();
        }

        private void LoadParentProfiles()
        {
            if (AllowToAddProfilesFromCompany)
                ParentProfile.Filter.AddProfilesFromOrganizationIdentifier = Organization.Identifier;

            ParentProfile.Filter.ProfileOrganizationIdentifier = OrganizationScope.Value == AccountScopes.Organization
                ? Organization.Identifier
                : OrganizationIdentifiers.CMDS;

            if (OrganizationScope.Value != AccountScopes.Organization)
                ParentProfile.Value = null;
        }

        public Guid GetSelectedOrganizationId()
        {
            return Organization.Identifier;
        }

        #endregion
    }
}
