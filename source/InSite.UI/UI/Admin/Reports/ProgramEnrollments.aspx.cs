using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.UI.Admin.Reports
{
    public partial class ProgramEnrollments : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const string CloseUrl = "/ui/admin/reporting";

        #endregion

        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid OrganizationIdentifier { get; set; }
            public string GroupType { get; set; }
            public Guid[] Groups { get; set; }
            public Guid[] Programs { get; set; }
            public Guid[] Achievements { get; set; }
            public Guid[] Learners { get; set; }
            public string CredentialStatus { get; set; }
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

            Criteria.Alert += (s, a) => ScreenStatus.AddMessage(a);

            DataRepeater.ItemDataBound += DataRepeater_ItemDataBound;

            DownloadXlsx.Click += DownloadXlsx_Click;
            ReportButton.Click += ReportButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Server.ScriptTimeout = 60 * 5;

            if (IsPostBack)
                return;

            var title = GetReportTitle();

            ActionModel.ActionName = title;

            PageHelper.AutoBindHeader(this);

            ReportTitle.Text = title;
            CloseButton1.NavigateUrl = CloseUrl;
            CloseButton2.NavigateUrl = CloseUrl;
        }

        #endregion

        #region Event handlers

        private void DataRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var achievementRepeater = (Repeater)e.Item.FindControl("AchievementRepeater");
            achievementRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Achievements");
            achievementRepeater.DataBind();
        }

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

            var title = GetReportTitle();
            var dataSource = SelectData()
                .Select(x => new
                {
                    Person = x.FullName,
                    GroupType = x.GroupType,
                    GroupName = x.GroupName,
                    Achievement = x.AchievementTitle,
                    Status = x.Status,
                    ProgramName = x.ProgramName,
                    ProgramStartDate = x.ProgramStartDate,
                    ProgramCompletedDate = x.ProgramCompletedDate,
                    TimeTaken = x.TimeTakenDays,
                })
                .ToList();

            var helper = new XlsxExportHelper();
            helper.Map("Person", "Person", 25, HorizontalAlignment.Left);
            helper.Map("GroupType", "Group Type", 20, HorizontalAlignment.Left);
            helper.Map("GroupName", "Group", 25, HorizontalAlignment.Left);
            helper.Map("Achievement", "Achievement", 40, HorizontalAlignment.Left);
            helper.Map("Status", "Status", 15, HorizontalAlignment.Left);
            helper.Map("ProgramName", "Program Name", 35, HorizontalAlignment.Left);
            helper.Map("ProgramStartDate", "Program Start Date", "MMM d, yyyy", 20, HorizontalAlignment.Center);
            helper.Map("ProgramCompletedDate", "Program Completed Date", "MMM d, yyyy", 20, HorizontalAlignment.Center);
            helper.Map("TimeTaken", "Time Taken (Days)", 18, HorizontalAlignment.Center);

            var bytes = helper.GetXlsxBytes(dataSource, title);

            ReportXlsxHelper.ExportToXlsx(title, bytes);
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            ReportTab.Visible = false;

            CurrentParameters = new SearchParameters
            {
                OrganizationIdentifier = Organization.Identifier,
                GroupType = Criteria.GroupTypeValue,
                Groups = Criteria.GroupValues,
                Programs = Criteria.ProgramValues,
                Achievements = Criteria.SelectedAchievements,
                Learners = Criteria.LearnerValues,
                CredentialStatus = Criteria.CredentialStatusValue,
            };

            var dataSource = SelectData()
                .GroupBy(x => (x.UserIdentifier, x.ProgramIdentifier))
                .Select(x =>
                {
                    var enrollment = x.First();

                    return new
                    {
                        UserIdentifier = enrollment.UserIdentifier,
                        FullName = enrollment.FullName,
                        GroupType = enrollment.GroupType,
                        GroupName = enrollment.GroupName,
                        ProgramIdentifier = enrollment.ProgramIdentifier,
                        ProgramName = enrollment.ProgramName,
                        ProgramStartDate = enrollment.ProgramStartDate,
                        ProgramCompletedDate = enrollment.ProgramCompletedDate,
                        TimeTakenDays = enrollment.TimeTakenDays,
                        Achievements = x
                            .GroupBy(y => y.AchievementIdentifier)
                            .Select(y =>
                            {
                                var achievement = y.First();

                                return new
                                {
                                    AchievementIdentifier = achievement.AchievementIdentifier,
                                    AchievementTitle = achievement.AchievementTitle,
                                    Status = achievement.Status,
                                };
                            })
                            .OrderBy(y => y.AchievementTitle)
                            .ToArray(),
                    };
                })
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.UserIdentifier)
                .ThenBy(x => x.ProgramName)
                .ThenBy(x => x.ProgramIdentifier)
                .ToArray();

            if (dataSource.Length == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            ReportRowCount.Text = dataSource.Select(x => x.UserIdentifier).Distinct().Count().ToString("n0");
            DataRepeater.DataSource = dataSource;
            DataRepeater.DataBind();
        }

        private IEnumerable<CommonReportHelper.ProgramEnrollment> SelectData()
        {
            return CommonReportHelper.SelectProgramEnrollments(
                CurrentParameters.OrganizationIdentifier,
                CurrentParameters.GroupType,
                CurrentParameters.Groups,
                CurrentParameters.Programs,
                CurrentParameters.Achievements,
                CurrentParameters.Learners,
                CurrentParameters.CredentialStatus);
        }

        private string GetReportTitle() => LabelSearch.GetTranslation(Route.Title, Language.Default, Organization.Identifier);

        #endregion
    }
}
