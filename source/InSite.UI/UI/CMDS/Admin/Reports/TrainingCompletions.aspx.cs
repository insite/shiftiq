using System;
using System.Collections.Generic;
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
            public Guid[] Learners { get; set; }
            public bool? IsRequired { get; set; }
            public DateTimeRange CredentialGranted { get; set; }
            public string CredentialStatus { get; set; }
            public string MembershipFunction { get; set; }
            public bool ExcludeSelfDeclaredCredentials { get; set; }
            public string AchievementType { get; set; }
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

            FindDepartment.AutoPostBack = true;
            FindDepartment.ValueChanged += (s, a) => OnDepartmentChanged();

            FindProgram.AutoPostBack = true;
            FindProgram.ValueChanged += (s, a) => SetupFindAchievement();

            AchievementType.AutoPostBack = true;
            AchievementType.ValueChanged += (s, a) => SetupFindAchievement();

            IsRequired.AutoPostBack = true;
            IsRequired.SelectedIndexChanged += (s, a) => SetupFindAchievement();

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

            FindDepartment.Filter.OrganizationIdentifier = Organization.Identifier;

            if (!Identity.HasAccessToAllCompanies)
                FindDepartment.Filter.UserIdentifier = User.UserIdentifier;

            OnDepartmentChanged();

            var closeUrl = "/ui/admin/reporting";
            CloseButton1.NavigateUrl = closeUrl;
            CloseButton2.NavigateUrl = closeUrl;
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
                    CurrentParameters.Learners,
                    CurrentParameters.IsRequired,
                    CurrentParameters.CredentialGranted,
                    CurrentParameters.CredentialStatus,
                    CurrentParameters.MembershipFunction,
                    CurrentParameters.@ExcludeSelfDeclaredCredentials,
                    CurrentParameters.AchievementType)
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

        private void OnDepartmentChanged()
        {
            FindLearner.Enabled = FindDepartment.HasValue;
            FindLearner.Filter.OrganizationIdentifier = Organization.Identifier;
            FindLearner.Filter.GroupDepartmentIdentifiers = FindDepartment.Values;
            if (ServiceLocator.Partition.IsE03())
                FindLearner.Filter.GroupDepartmentFunctions = new[] { "Department" };
            FindLearner.Value = null;

            SetupFindAchievement();
        }

        private void SetupFindAchievement()
        {
            FindAchievement.Enabled = FindDepartment.HasValue;
            FindAchievement.Filter.DepartmentIdentifiers = FindDepartment.Values;
            FindAchievement.Filter.ProgramIdentifiers = FindProgram.Values;
            FindAchievement.Filter.HasMandatoryCredential = GetIsRequired();
            FindAchievement.Filter.AchievementLabels.Clear();
            FindAchievement.Filter.AchievementLabels.Add(AchievementType.Value);
            FindAchievement.Value = null;
        }

        private void LoadReport()
        {
            ReportTab.Visible = false;

            if (!Page.IsValid)
                return;

            var departments = GetEffectiveDepartments();
            if (departments.Length == 0)
                return;

            CurrentParameters = BuildSearchParameters(departments);

            var dataSource = CmdsReportHelper
                .SelectTrainingCompletionDates(
                    CurrentParameters.Departments,
                    CurrentParameters.Achievements,
                    CurrentParameters.Learners,
                    CurrentParameters.IsRequired,
                    CurrentParameters.CredentialGranted,
                    CurrentParameters.CredentialStatus,
                    CurrentParameters.MembershipFunction,
                    CurrentParameters.@ExcludeSelfDeclaredCredentials,
                    CurrentParameters.AchievementType
                    )
                .ToList();

            if (dataSource.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            BindReportHeader();
            BindReportData(dataSource);
        }

        private Guid[] GetEffectiveDepartments()
        {
            var selected = FindDepartment.Values;
            return selected.Length > 0
                ? selected
                : FindDepartment.GetDataItems().Select(x => x.Value).ToArray();
        }

        private SearchParameters BuildSearchParameters(Guid[] departments)
        {
            return new SearchParameters
            {
                OrganizationIdentifier = Organization.Identifier,
                Departments = departments,
                Achievements = FindAchievement.Values,
                Learners = FindLearner.Values,
                IsRequired = GetIsRequired(),
                CredentialGranted = new DateTimeRange(CredentialGrantedSince.Value, CredentialGrantedBefore.Value),
                CredentialStatus = CredentialStatus.Value,
                MembershipFunction = GetMembershipFunction(),
                ExcludeSelfDeclaredCredentials = ExcludeSelfDeclaredCredentials.Checked,
                AchievementType = AchievementType.Value
            };
        }

        private void BindReportHeader()
        {
            var departmentNames = DepartmentSearch.Bind(
                x => x.DepartmentName,
                x => CurrentParameters.Departments.Contains(x.DepartmentIdentifier),
                null,
                "DepartmentName");

            DepartmentsList.Text = string.Join(", ", departmentNames);
            CompanyName.Text = OrganizationSearch
                .Select(Organization.Identifier).CompanyName;
        }

        private void BindReportData(List<CmdsReportHelper.TrainingCompletionDate> dataSource)
        {
            ReportTab.Visible = true;
            ReportTab.IsSelected = true;
            DataRepeater.DataSource = dataSource;
            DataRepeater.DataBind();
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