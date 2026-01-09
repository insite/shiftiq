using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

using Label = System.Web.UI.WebControls.Label;

namespace InSite.Cmds.Admin.Workflows.Profiles.Forms
{
    public partial class AssignDepartment : AdminBasePage, ICmdsUserControl
    {
        #region Properties

        private ProfileFilter ProfileFilter => (ProfileFilter)(ViewState[nameof(ProfileFilter)]
            ?? (ViewState[nameof(ProfileFilter)] = new ProfileFilter
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier
            }));

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProfilesValidator.ServerValidate += ProfilesValidator_ServerValidate;
            DepartmentsValidator.ServerValidate += DepartmentsValidator_ServerValidate;

            FilterButton.Click += FilterButton_Click;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            LoadDepartments();
            LoadProfiles();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            SelectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', true);", Profiles.ClientID);
            UnselectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', false);", Profiles.ClientID);
        }

        #endregion

        #region Event handlers

        private void ProfilesValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetSelectedProfiles().Length > 0;
        }

        private void DepartmentsValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetSelectedDepartments().Length > 0;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            ProfileFilter.ProfileNumber = ProfileNumber.Text;
            ProfileFilter.ProfileTitle = ProfileTitle.Text;

            LoadProfiles();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            SaveData();

            ScreenStatus.AddMessage(AlertType.Success, "Your changes have been saved.");
        }

        #endregion

        #region Save data methods

        private void SaveData()
        {
            var profiles = GetSelectedProfiles();
            var departments = GetSelectedDepartments();
            var newWorkflows = new List<TDepartmentStandard>();

            foreach (var profileStandardIdentifier in profiles)
            {
                foreach (var department in departments)
                {
                    if (!TDepartmentStandardSearch.Exists(x => x.DepartmentIdentifier == department && x.StandardIdentifier == profileStandardIdentifier))
                        newWorkflows.Add(new TDepartmentStandard { DepartmentIdentifier = department, StandardIdentifier = profileStandardIdentifier });
                }
            }

            TDepartmentStandardStore.Insert(newWorkflows);

            foreach (var workflow in newWorkflows)
                DepartmentProfileCompetencyRepository2.InsertProfileCompetencies(workflow.DepartmentIdentifier, workflow.StandardIdentifier);
        }

        private Guid[] GetSelectedProfiles()
        {
            List<Guid> selectedProfiles = new List<Guid>();

            foreach (RepeaterItem achievementItem in Profiles.Items)
            {
                var selected = (ICheckBoxControl)achievementItem.FindControl("Selected");
                if (!selected.Checked)
                    continue;

                var achievementID = Guid.Parse(((Label)achievementItem.FindControl("ProfileStandardIdentifier")).Text);
                selectedProfiles.Add(achievementID);
            }

            return selectedProfiles.ToArray();
        }

        private Guid[] GetSelectedDepartments()
        {
            var selectedDepartments = new List<Guid>();

            foreach (RepeaterItem item in Departments.Items)
            {
                var selected = (ICheckBoxControl)item.FindControl("Selected");
                if (!selected.Checked)
                    continue;

                var key = Guid.Parse(((Label)item.FindControl("DepartmentIdentifier")).Text);
                selectedDepartments.Add(key);
            }

            return selectedDepartments.ToArray();
        }

        #endregion

        #region Load data methods

        private void LoadDepartments()
        {
            Departments.DataSource = ContactRepository3.SelectDepartments(new DepartmentFilter
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier
            });
            Departments.DataBind();
        }

        private void LoadProfiles()
        {
            Profiles.DataSource = ProfileRepository.SelectSearchResults(ProfileFilter);
            Profiles.DataBind();
        }

        #endregion
    }
}