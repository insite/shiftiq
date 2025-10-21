using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using SystemListItem = System.Web.UI.WebControls.ListItem;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class TrainingCompletions : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid[] Departments { get; set; }
            public Guid[] Achievements { get; set; }
            public bool? IsRequired { get; set; }
            public DateTimeRange CredentialGranted { get; set; }
            public string CredentialStatus { get; set; }
            public string MembershipFunction { get; set; }
            public bool? IncludeSelfDeclaredCredentials { get; set; }
        }

        #endregion

        #region Properties

        private SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentIdentifierValidator.ServerValidate += (s, a) => a.IsValid = DepartmentIdentifier.Enabled;
            AchievementSelectorValidator.ServerValidate += (s, a) => a.IsValid = AchievementSelector.HasValue();

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) => LoadAchievements();

            IsRequired.AutoPostBack = true;
            IsRequired.SelectedIndexChanged += (s, a) => LoadAchievements();

            DownloadXlsx.Click += DownloadXlsx_Click;

            ReportButton.Click += ReportButton_Click;

            BindMembershipFunctions(false, false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Server.ScriptTimeout = 60 * 5; // 5 minutes

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            DepartmentIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!Identity.HasAccessToAllCompanies)
                DepartmentIdentifier.Filter.UserIdentifier = User.UserIdentifier;

            var hasDepartments = DepartmentIdentifier.GetCount() > 0;

            DepartmentIdentifier.Enabled = hasDepartments;
            DepartmentIdentifier.EmptyMessage = hasDepartments ? "All Departments" : "None";

            LoadAchievements();
        }

        private void BindMembershipFunctions(bool showAdmin, bool isOrganizationChecked)
        {
            var org = new SystemListItem("Organization");

            var dept = new SystemListItem("Department");

            org.Selected = isOrganizationChecked;

            dept.Selected = true;

            if (showAdmin)
            {
                var admin = new SystemListItem("Administration");

                MembershipFunction.Items.Add(admin);
            }

            MembershipFunction.Items.Add(org);

            MembershipFunction.Items.Add(dept);
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            if (CurrentParameters == null)
                return;

            var dataSource = CmdsReportHelper
                .SelectTrainingCompletionDates(
                    CurrentParameters.Departments,
                    CurrentParameters.Achievements,
                    CurrentParameters.IsRequired,
                    CurrentParameters.CredentialGranted,
                    CurrentParameters.CredentialStatus,
                    CurrentParameters.MembershipFunction,
                    CurrentParameters.IncludeSelfDeclaredCredentials)
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.CompanyName)
                .ThenBy(x => x.DepartmentName)
                .ThenBy(x => x.AchievementTitle)
                .ThenBy(x => x.DateCompleted)
                .ThenBy(x => x.CredentialStatus)
                .Select(x => new
                {
                    Person = x.FullName,
                    Organization = x.CompanyName,
                    Department = x.DepartmentName,
                    Achievement = x.AchievementTitle,
                    AchievementType = x.AchievementLabel,
                    Completed = x.DateCompleted.HasValue ? x.DateCompleted.Value.UtcDateTime : (DateTime?)null,
                    Status = x.CredentialStatus,
                    Expired = x.ExpirationDate.HasValue ? x.ExpirationDate.Value.UtcDateTime : (DateTime?)null,
                    Score = x.GradePercent,
                })
                .ToList();

            var helper = new XlsxExportHelper();
            helper.Map("Person", "Person");
            helper.Map("Organization", "Organization");
            helper.Map("Department", "Department");
            helper.Map("Achievement", "Achievement", 60, HorizontalAlignment.Left);
            helper.Map("AchievementType", "Achievement Type", 35, HorizontalAlignment.Left);
            helper.Map("Completed", "Completed", "MMM d, yyyy", 20, HorizontalAlignment.Center);
            helper.Map("Status", "Status", 20, HorizontalAlignment.Left);
            helper.Map("Expired", "Expired", "MMM d, yyyy", 20, HorizontalAlignment.Center);
            helper.Map("Score", "Score", "0%", 20, HorizontalAlignment.Center);

            var bytes = helper.GetXlsxBytes(dataSource, Route.Title);

            ReportXlsxHelper.ExportToXlsx(Route.Title, bytes);
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                Departments = DepartmentIdentifier.Values,
                Achievements = AchievementSelector.GetSelectedAchievements(),
                IsRequired = GetIsRequired(),
                CredentialGranted = new DateTimeRange(CredentialGrantedStartDate.Value, CredentialGrantedEndDate.Value),
                CredentialStatus = CredentialStatus.Value,
                MembershipFunction = GetMembershipFunction(),
                IncludeSelfDeclaredCredentials = IncludeSelfDeclaredCredentials.Checked
            };

            if (CurrentParameters.Departments.Length == 0)
                CurrentParameters.Departments = DepartmentIdentifier.GetDataItems().Select(x => x.Value).ToArray();

            if (CurrentParameters.Departments.Length == 0)
                return;

            var dataSource = CmdsReportHelper.SelectTrainingCompletionDates(
                CurrentParameters.Departments,
                CurrentParameters.Achievements,
                CurrentParameters.IsRequired,
                CurrentParameters.CredentialGranted,
                CurrentParameters.CredentialStatus,
                CurrentParameters.MembershipFunction,
                CurrentParameters.IncludeSelfDeclaredCredentials);

            if (!dataSource.Any())
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            var departmentsNames = DepartmentSearch.Bind(x => x.DepartmentName, x => CurrentParameters.Departments.Contains(x.DepartmentIdentifier), null, "DepartmentName");
            DepartmentsList.Text = string.Join(", ", departmentsNames);

            CompanyName.Text = OrganizationSearch
                .Select(CurrentIdentityFactory.ActiveOrganizationIdentifier).CompanyName;

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DataRepeater.DataSource = dataSource
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.CompanyName)
                .ThenBy(x => x.DepartmentName)
                .ThenBy(x => x.AchievementTitle)
                .ThenBy(x => x.DateCompleted)
                .ThenBy(x => x.CredentialStatus);
            DataRepeater.DataBind();
        }

        private void LoadAchievements()
        {
            Guid[] departments = null;

            if (DepartmentIdentifier.Enabled)
            {
                departments = DepartmentIdentifier.Values;

                if (departments.Length == 0)
                    departments = DepartmentIdentifier.GetDataItems().Select(x => x.Value).ToArray();
            }

            var hasAchievements = AchievementSelector.LoadData(departments, GetIsRequired());

            AchievementSelector.Visible = hasAchievements;

            if (!hasAchievements)
                ScreenStatus.AddMessage(AlertType.Error, "The departments you have selected do not have any training achievements.");
        }

        #endregion

        #region Helper methods

        private bool? GetIsRequired()
        {
            return IsRequired.SelectedIndex > 0 ? bool.Parse(IsRequired.SelectedValue) : (bool?)null;
        }

        private string GetMembershipFunction()
        {
            var list = new StringBuilder();

            foreach (SystemListItem item in MembershipFunction.Items)
            {
                if (!item.Selected)
                    continue;

                if (list.Length > 0)
                    list.Append(",");

                list.Append(item.Value);
            }

            if (list.Length == 0)
                foreach (SystemListItem item in MembershipFunction.Items)
                {
                    if (list.Length > 0)
                        list.Append(",");

                    list.Append(item.Value);
                }

            return list.ToString();
        }

        #endregion
    }
}