using System;

using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.CMDS
{
    public partial class Tools : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            if (ServiceLocator.Partition.IsE03())
                BindModelToControlsForE03();
            else
                BindModelToControls();
        }

        protected void BindModelToControlsForE03()
        {
            var isGrantedAdministrators = Identity.IsGranted(PermissionNames.Custom_CMDS_Administrators);
            var isGrantedDevelopers = Identity.IsGranted(PermissionNames.Custom_CMDS_Developers);
            var isGrantedFields = Identity.IsGranted(PermissionNames.Custom_CMDS_Fields);
            var isGrantedTesters = Identity.IsGranted(PermissionNames.Custom_CMDS_Testers);
            var isGrantedUsers = Identity.IsGranted(PermissionNames.Custom_CMDS_Users);
            var isGrantedValidators = Identity.IsGranted(PermissionNames.Custom_CMDS_Validators);

            SearchPeopleLink.Visible = isGrantedUsers;
            SearchOrganizationsLink.Visible = isGrantedAdministrators;
            OverrideCompetenciesLink.Visible = isGrantedAdministrators;

            SearchAchievementsLink.Visible = isGrantedAdministrators;
            SearchCompetenciesLink.Visible = isGrantedAdministrators;
            SearchProfilesLink.Visible = isGrantedAdministrators;
            SearchProgramsLink.Visible = isGrantedFields;
            SearchCategoriesLink.Visible = isGrantedAdministrators;

            ScoopLibraryLink.Visible = false;

            var scoopBaseUrl = ServiceLocator.AppSettings.Engine?.Api?.Scoop?.BaseUrl;

            if (scoopBaseUrl != null)
            {
                var baseUri = new Uri(scoopBaseUrl);

                var libraryUrl = new Uri(baseUri, Organization.Code);

                ScoopLibraryLink.Text = $"<i class='fas fa-building-columns me-1'></i>{Organization.Name} SCO Library";

                ScoopLibraryLink.NavigateUrl = libraryUrl.AbsoluteUri.ToString();

                ScoopLibraryLink.Visible = true;

                ScoopLibraryLink.Target = "_blank";
            }

            CompareProfilesLink.Visible = isGrantedAdministrators;

            FieldSearchAchievementsLink.Visible = isGrantedFields;
            FieldSearchProfilesLink.Visible = isGrantedFields;

            AssignDepartmentsLink1.Visible = isGrantedAdministrators;
            AssignDepartmentsLink2.Visible = isGrantedAdministrators;
            AssignDepartmentsLink3.Visible = isGrantedAdministrators;
            AssignProgramsLink.Visible = isGrantedFields;
            ValidateCompetenciesLink.Visible = isGrantedValidators;

            BulkAssignCertificatesLink.Visible = isGrantedFields;
            BulkSetupforPolicySignOffs.Visible = isGrantedFields;
            BulkAssignEmployeestoManagers.Visible = isGrantedFields;
            AssignProfilestoPeople.Visible = isGrantedFields;
            BulkCompetencyExpiry.Visible = isGrantedFields;
            BulkEmployeeAchievementExpiry.Visible = isGrantedFields;

            DeveloperTools.Visible = isGrantedDevelopers;
            TroubleshootingTools.Visible = isGrantedFields;
            AnalyticsPanel.Visible = isGrantedTesters;
        }

        protected void BindModelToControls() { }
    }
}