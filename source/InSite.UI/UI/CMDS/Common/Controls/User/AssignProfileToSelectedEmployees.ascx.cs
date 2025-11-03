using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

using UserProfileRepository = InSite.Persistence.Plugin.CMDS.UserProfileRepository;

namespace InSite.Cmds.Controls.BulkTool.Assign
{
    public partial class AssignProfileToSelectedEmployees : UserControl
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
            DepartmentIdentifier.ValueChanged += (s, a) => LoadProfiles();

            ProfileIdentifier.AutoPostBack = true;
            ProfileIdentifier.ValueChanged += (s, a) => LoadEmployees();

            ProfileType.AutoPostBack = true;
            ProfileType.ValueChanged += (s, a) => LoadEmployees();

            DepartmentEmployees.ItemDataBound += DepartmentEmployees_ItemDataBound;

            SaveButton.Click += SaveButton_Click;
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

            LoadProfiles();
        }

        #endregion

        #region Event handlers

        private void DepartmentEmployees_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (DataRowView)e.Item.DataItem;
            var row = item.Row;
            var contactID = (Guid)row["UserIdentifier"];

            if (ProfileType.Value != "Primary")
                return;

            var profile = UserProfileRepository.SelectPrimaryProfile(contactID, CurrentIdentityFactory.ActiveOrganizationIdentifier);
            if (profile == null || profile.ProfileStandardIdentifier == ProfileIdentifier.Value)
                return;

            var cb = (System.Web.UI.WebControls.CheckBox)e.Item.FindControl("IsSelected");
            cb.Checked = false;
            cb.Enabled = false;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            SaveData();
            LoadEmployees();
        }

        #endregion

        #region Helper methods

        private void LoadProfiles()
        {
            var departmentId = DepartmentIdentifier.Value;

            RowProfileType.Visible = departmentId.HasValue;
            ComplianceRequiredField.Visible = departmentId.HasValue;
            RowProfile.Visible = departmentId.HasValue;
            EmployeesColumn.Visible = false;

            if (!departmentId.HasValue)
                return;

            ProfileIdentifier.Filter.DepartmentIdentifier = departmentId.Value;
            ProfileIdentifier.Value = null;
        }

        private void LoadEmployees()
        {
            var hasProfile = ProfileIdentifier.HasValue;

            EmployeesColumn.Visible = hasProfile;

            if (!hasProfile)
                return;

            var isPrimaryProfile = ProfileType.Value == "Primary";

            ComplianceRequired.Checked = isPrimaryProfile;
            ComplianceRequired.Enabled = !isPrimaryProfile;

            DepartmentEmployees.DataSource = Persistence.Plugin.CMDS.ContactRepository3.SelectEmployeesByDepartmentProfileId(
                DepartmentIdentifier.Value.Value, ProfileIdentifier.Value.Value, isPrimaryProfile);
            DepartmentEmployees.DataBind();
        }

        private void SaveData()
        {
            if (!DepartmentIdentifier.HasValue)
                throw ApplicationError.Create("Missing Required Field: Department");

            var departmentIdentifier = DepartmentIdentifier.Value.Value;

            if (!ProfileIdentifier.HasValue)
                throw ApplicationError.Create("Missing Required Field: CurrentProfile");

            var profileStandardIdentifier = ProfileIdentifier.Value.Value;

            var isPrimaryProfile = ProfileType.Value == "Primary";
            var isComplianceRequired = isPrimaryProfile || ComplianceRequired.Checked;

            var updateList = new List<DepartmentProfileUser>();
            var deleteList = new List<DepartmentProfileUser>();
            var errors = new StringCollection();

            foreach (RepeaterItem item in DepartmentEmployees.Items)
            {
                var checkbox = (ICheckBoxControl)item.FindControl("IsSelected");
                var userIdentifier = Guid.Parse(((ITextControl)item.FindControl("UserIdentifier")).Text);
                var isSelected = checkbox.Checked;

                var employeeProfileInfo = DepartmentProfileUserSearch.SelectFirst(x => x.UserIdentifier == userIdentifier && x.DepartmentIdentifier == departmentIdentifier && x.ProfileStandardIdentifier == profileStandardIdentifier);
                var primaryUserProfile = DepartmentProfileUserSearch.SelectFirst(x => x.UserIdentifier == userIdentifier && x.Department.OrganizationIdentifier == CurrentIdentityFactory.ActiveOrganizationIdentifier && x.IsPrimary);

                if (isSelected && employeeProfileInfo == null)
                {
                    try
                    {
                        UserProfileRepository.RegisterNewProfile(isPrimaryProfile, departmentIdentifier, userIdentifier, profileStandardIdentifier, false, false, isComplianceRequired);
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Cannot insert duplicate key row in object"))
                            throw;

                        var person = UserSearch.Select(userIdentifier);
                        var error = $"{person.FullName} is already assigned another primary profile";
                        errors.Add(error);
                        continue;
                    }

                    employeeProfileInfo = DepartmentProfileUserSearch.SelectFirst(x => x.UserIdentifier == userIdentifier && x.DepartmentIdentifier == departmentIdentifier && x.ProfileStandardIdentifier == profileStandardIdentifier);
                    primaryUserProfile = DepartmentProfileUserSearch.SelectFirst(x => x.UserIdentifier == userIdentifier && x.Department.OrganizationIdentifier == CurrentIdentityFactory.ActiveOrganizationIdentifier && x.IsPrimary);
                }

                if (isPrimaryProfile)
                {
                    if (isSelected)
                    {
                        SetEmployeeProfileAsPrimary(userIdentifier);
                    }
                    else if (employeeProfileInfo != null && employeeProfileInfo.IsPrimary)
                    {
                        employeeProfileInfo.IsPrimary = false;
                        updateList.Add(employeeProfileInfo);
                    }
                }
                else
                {
                    if (employeeProfileInfo == null)
                        continue;

                    if (isSelected)
                    {
                        if (employeeProfileInfo.IsRequired == isComplianceRequired && !employeeProfileInfo.IsPrimary)
                            continue;

                        employeeProfileInfo.IsRequired = isComplianceRequired;
                        updateList.Add(employeeProfileInfo);
                    }
                    else if (primaryUserProfile == null || primaryUserProfile.DepartmentIdentifier != departmentIdentifier || primaryUserProfile.ProfileStandardIdentifier != profileStandardIdentifier)
                    {
                        deleteList.Add(employeeProfileInfo);
                    }
                }
            }

            DepartmentProfileUserStore.InsertUpdateDelete(null, updateList, deleteList);

            if (errors.Count > 0)
            {
                var html = new StringBuilder();
                html.Append("<ul>");
                foreach (var error in errors)
                    html.Append("<li>" + error + "</li>");
                html.Append("</ul>");
                OnAlert(AlertType.Warning, html.ToString());
            }
            else
            {
                OnAlert(AlertType.Success, "Your changes have been saved.");
            }
        }

        private void SetEmployeeProfileAsPrimary(Guid employeeID)
        {
            if (ProfileIdentifier.HasValue && DepartmentIdentifier.HasValue)
                UserProfileRepository.ChangePrimaryUserProfile(
                    employeeID, ProfileIdentifier.Value.Value, DepartmentIdentifier.Value.Value);
        }

        #endregion
    }
}