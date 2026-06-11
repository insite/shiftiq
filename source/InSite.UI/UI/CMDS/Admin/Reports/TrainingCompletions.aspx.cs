using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class TrainingCompletions : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const string CloseUrl = "/ui/admin/reporting";

        #endregion

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

            Criteria.MessageRaised += (type, message) => ScreenStatus.AddMessage(type, message);

            DownloadXlsx.Click += DownloadXlsx_Click;
            ReportButton.Click += ReportButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Server.ScriptTimeout = 60 * 5;

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CloseButton1.NavigateUrl = CloseUrl;
            CloseButton2.NavigateUrl = CloseUrl;
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                ReportTab.Visible = false;
                return;
            }

            LoadReport();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            if (CurrentParameters == null)
                return;

            var dataSource = SelectCompletions()
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
                    Assigned = x.DateAssigned.HasValue ? x.DateAssigned.Value.UtcDateTime : (DateTime?)null,
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
            helper.Map("Assigned", "Assigned", "MMM d, yyyy", 20, HorizontalAlignment.Center);
            helper.Map("Expired", "Expired", "MMM d, yyyy", 20, HorizontalAlignment.Center);
            helper.Map("Score", "Score", "0%", 20, HorizontalAlignment.Center);

            var bytes = helper.GetXlsxBytes(dataSource, Route.Title);

            ReportXlsxHelper.ExportToXlsx(Route.Title, bytes);
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            ReportTab.Visible = false;

            if (!Criteria.ValidateNarrowSelection(out var error))
            {
                ScreenStatus.AddMessage(AlertType.Error, error);
                return;
            }

            var departments = Criteria.EffectiveDepartmentValues;
            if (departments.Length == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            CurrentParameters = BuildSearchParameters(departments);

            var dataSource = SelectCompletions().ToList();

            if (dataSource.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            BindReportHeader();
            BindReportData(dataSource);
        }

        private IEnumerable<CmdsReportHelper.TrainingCompletionDate> SelectCompletions()
        {
            return CmdsReportHelper.SelectTrainingCompletionDates(
                CurrentParameters.Departments,
                CurrentParameters.Achievements,
                CurrentParameters.Learners,
                CurrentParameters.IsRequired,
                CurrentParameters.CredentialGranted,
                CurrentParameters.CredentialStatus,
                CurrentParameters.MembershipFunction,
                CurrentParameters.ExcludeSelfDeclaredCredentials,
                achievementType: null);
        }

        private SearchParameters BuildSearchParameters(Guid[] departments)
        {
            return new SearchParameters
            {
                OrganizationIdentifier = Organization.Identifier,
                Departments = departments,
                Achievements = Criteria.SelectedAchievements,
                Learners = Criteria.LearnerValues,
                IsRequired = Criteria.IsRequiredFilter,
                CredentialGranted = new DateTimeRange(Criteria.CompletedSinceFilter, Criteria.CompletedBeforeFilter),
                CredentialStatus = Criteria.CredentialStatusFilter,
                MembershipFunction = string.Join(",", Criteria.MembershipFunctions),
                ExcludeSelfDeclaredCredentials = Criteria.ExcludeSelfDeclared
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
    }
}
