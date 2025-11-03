using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common.Events;
using Shift.Constant;

namespace InSite.Cmds.Controls.BulkTool.Assign
{
    public partial class AssignSelectedProfilesToEmployees : UserControl
    {
        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) => LoadDepartmentData();

            SaveButton.Click += SaveButton_Click;
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            SaveDepartmentData();
            LoadDepartmentData();
        }

        #endregion

        #region Public methods

        public void LoadData(PersonFinderSecurityInfoWrapper finderSecurityInfo)
        {
            DepartmentIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            DepartmentIdentifier.Filter.UserIdentifier = finderSecurityInfo.CanSeeAllDepartments || CurrentSessionState.Identity.HasAccessToAllCompanies
                ? (Guid?)null
                : CurrentSessionState.Identity.User.UserIdentifier;
            DepartmentIdentifier.Value = null;
        }

        #endregion

        #region Helper methods

        private void LoadDepartmentData()
        {
            var hasDepartment = DepartmentIdentifier.HasValue;

            DepartmentRow.Visible = hasDepartment;

            if (!hasDepartment)
                return;

            DepartmentProfiles.DataSource = ProfileRepository.SelectCompanyProfilesForDepartment(CurrentIdentityFactory.ActiveOrganizationIdentifier, DepartmentIdentifier.Value.Value);
            DepartmentProfiles.DataBind();

            DepartmentEmployees.DataSource = ContactRepository3.SelectEmployeesByDepartmentId(DepartmentIdentifier.Value.Value);
            DepartmentEmployees.DataBind();
        }

        private void SaveDepartmentData()
        {
            SaveProfilesForDepartment();
            SaveProfilesForEmployees();

            OnAlert(AlertType.Success, "Your changes have been saved.");
        }

        private void SaveProfilesForDepartment()
        {
            var newProfiles = new List<Guid>();
            var deleteProfiles = new List<Guid>();

            foreach (RepeaterItem item in DepartmentProfiles.Items)
            {
                var profileStandardIdentifierCtrl = (ITextControl)item.FindControl("ProfileStandardIdentifier");
                var IsSelectedCtrl = (ICheckBoxControl)item.FindControl("IsSelected");

                var profileStandardIdentifier = Guid.Parse(profileStandardIdentifierCtrl.Text);
                var isSelected = IsSelectedCtrl.Checked;

                var entity = TDepartmentStandardSearch.SelectFirst(x => x.DepartmentIdentifier == DepartmentIdentifier.Value.Value && x.StandardIdentifier == profileStandardIdentifier);

                if (isSelected)
                {
                    if (entity == null)
                        newProfiles.Add(profileStandardIdentifier);
                }
                else if (entity != null)
                    deleteProfiles.Add(profileStandardIdentifier);
            }

            if (newProfiles.Count > 0)
                TDepartmentStandardStore.InsertPermissions(DepartmentIdentifier.Value.Value, newProfiles);

            if (deleteProfiles.Count > 0)
                TDepartmentStandardStore.DeleteByDepartment(DepartmentIdentifier.Value.Value, deleteProfiles);
        }

        private void SaveProfilesForEmployees()
        {
            var persons = ContactRepository3.SelectEmployeesByDepartmentId(DepartmentIdentifier.Value.Value);

            var departmentKey = DepartmentIdentifier.Value.Value;

            foreach (RepeaterItem item in DepartmentProfiles.Items)
            {
                var ProfileIDCtrl = (ITextControl)item.FindControl("ProfileStandardIdentifier");
                var IsSelectedCtrl = (ICheckBoxControl)item.FindControl("IsSelected");

                var profileID = Guid.Parse(ProfileIDCtrl.Text);
                var isSelected = IsSelectedCtrl.Checked;

                foreach (DataRow person in persons.Rows)
                {
                    var personID = (Guid)person["PersonID"];

                    var info = DepartmentProfileUserSearch.SelectFirst(x => x.UserIdentifier == personID && x.DepartmentIdentifier == departmentKey && x.ProfileStandardIdentifier == profileID);

                    if (isSelected)
                    {
                        if (info == null)
                            UserProfileRepository.RegisterNewProfile(false, departmentKey, personID, profileID, false, false, false);
                    }
                    else if (info != null)
                    {
                        DepartmentProfileUserStore.Delete(new DepartmentProfileUser[] { info }, CurrentSessionState.Identity.User.UserIdentifier, CurrentSessionState.Identity.Organization.OrganizationIdentifier);
                    }
                }
            }
        }

        #endregion
    }
}