using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class TrainingHistoryByWorker : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid[] Departments { get; set; }
            public Guid[] Achievements { get; set; }
            public Guid[] Learners { get; set; }
            public bool? IsRequired { get; set; }
            public string AchievementType { get; set; }
        }

        private class UserDataItem
        {
            public string PersonFullName { get; set; }

            public IEnumerable<AchievementDataItem> Achievements { get; set; }
        }

        private class AchievementDataItem
        {
            public string AchievementTitle { get; set; }
            public DateTimeOffset? DateCompleted { get; set; }
            public DateTimeOffset? ExpirationDate { get; set; }
            public string AccreditorName { get; set; }
            public bool IsCompetent { get; set; }
            public decimal? Score { get; set; }
        }

        #endregion

        #region Properties

        private SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        #endregion

        #region Initialization & Loading

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

            UserRepeater.ItemDataBound += UserRepeater_ItemDataBound;

            DownloadXlsx.Click += DownloadXlsx_Click;

            ReportButton.Click += ReportButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

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

        #endregion

        #region Event handlers

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
            FindAchievement.Filter.DepartmentIdentifiers = FindDepartment.Values;
            FindAchievement.Filter.ProgramIdentifiers = FindProgram.Values;
            FindAchievement.Filter.HasMandatoryCredential = GetIsRequired();
            FindAchievement.Filter.AchievementLabels.Clear();
            FindAchievement.Filter.AchievementLabels.Add(AchievementType.Value);
            FindAchievement.Value = null;
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void UserRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var userDataItem = (UserDataItem)e.Item.DataItem;

            var achievementRepeater = (Repeater)e.Item.FindControl("AchievementRepeater");
            achievementRepeater.DataSource = userDataItem.Achievements;
            achievementRepeater.DataBind();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            if (CurrentParameters == null)
                return;

            var dataSource = GetReportDataSource();
            if (!dataSource.Any())
                return;

            using (var excel = new ExcelPackage())
            {
                var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                defaultStyle.Font.Name = "Arial";
                defaultStyle.Font.Size = 10;
                defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;

                var reportTitleStyle = excel.Workbook.Styles.CreateNamedStyle("Report Title");
                reportTitleStyle.Style.Font.Bold = true;
                reportTitleStyle.Style.Font.Size = 14;
                reportTitleStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                reportTitleStyle.Style.Border.Bottom.Color.SetColor(Color.Black);

                var tableHeaderStyle = excel.Workbook.Styles.CreateNamedStyle("Table Header");
                tableHeaderStyle.Style.Font.Bold = true;
                tableHeaderStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                tableHeaderStyle.Style.Border.Bottom.Color.SetColor(Color.Black);

                var tableCellStyle = excel.Workbook.Styles.CreateNamedStyle("Table Cell");
                tableCellStyle.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                tableCellStyle.Style.Border.Top.Color.SetColor(Color.Silver);
                tableCellStyle.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                tableCellStyle.Style.Border.Right.Color.SetColor(Color.Silver);
                tableCellStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                tableCellStyle.Style.Border.Bottom.Color.SetColor(Color.Silver);
                tableCellStyle.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                tableCellStyle.Style.Border.Left.Color.SetColor(Color.Silver);

                var sheet = excel.Workbook.Worksheets.Add(Route.Title);

                sheet.Column(1).Width = 40;
                sheet.Column(2).Width = 30;
                sheet.Column(3).Width = 12;
                sheet.Column(4).Width = 12;
                sheet.Column(5).Width = 12;

                var rowNumber = 1;

                var headerCell = sheet.Cells[rowNumber, 1, rowNumber, 5];
                headerCell.Merge = true;
                headerCell.Value = "Training History By Worker";
                headerCell.StyleName = reportTitleStyle.Name;

                sheet.Row(rowNumber++).Height = 22.5;
                sheet.Row(rowNumber++).Height = 10;

                foreach (var user in dataSource)
                {
                    var userNameCell = sheet.Cells[rowNumber, 1, rowNumber, 5];
                    userNameCell.Merge = true;
                    userNameCell.Value = user.PersonFullName;

                    rowNumber++;
                    sheet.Row(rowNumber++).Height = 10;

                    var achievementNameHeaderCell = sheet.Cells[rowNumber, 1];
                    achievementNameHeaderCell.Value = "Achievement Name";
                    achievementNameHeaderCell.StyleName = tableHeaderStyle.Name;

                    var providerHeaderCell = sheet.Cells[rowNumber, 2];
                    providerHeaderCell.Value = "Provider";
                    providerHeaderCell.StyleName = tableHeaderStyle.Name;

                    var completionDateHeaderCell = sheet.Cells[rowNumber, 3];
                    completionDateHeaderCell.Value = "Completion";
                    completionDateHeaderCell.StyleName = tableHeaderStyle.Name;
                    completionDateHeaderCell.Style.WrapText = true;
                    completionDateHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    var renewalDateHeaderCell = sheet.Cells[rowNumber, 4];
                    renewalDateHeaderCell.Value = "Renewal";
                    renewalDateHeaderCell.StyleName = tableHeaderStyle.Name;
                    renewalDateHeaderCell.Style.WrapText = true;
                    renewalDateHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    var completeHeaderCell = sheet.Cells[rowNumber, 5];
                    completeHeaderCell.Value = "Valid";
                    completeHeaderCell.StyleName = tableHeaderStyle.Name;
                    completeHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    rowNumber++;

                    foreach (var achievement in user.Achievements)
                    {
                        var achievementNameCell = sheet.Cells[rowNumber, 1];
                        achievementNameCell.Value = achievement.AchievementTitle;
                        achievementNameCell.StyleName = tableCellStyle.Name;

                        var providerCell = sheet.Cells[rowNumber, 2];
                        providerCell.Value = achievement.AccreditorName;
                        providerCell.StyleName = tableCellStyle.Name;

                        var completionDateCell = sheet.Cells[rowNumber, 3];
                        completionDateCell.Value = achievement.DateCompleted == null ? "" : achievement.DateCompleted.Value.ToString("MM/dd/yyyy");
                        completionDateCell.StyleName = tableCellStyle.Name;
                        completionDateCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        var renewalDateCell = sheet.Cells[rowNumber, 4];
                        renewalDateCell.Value = achievement.ExpirationDate == null ? "" : achievement.ExpirationDate.Value.ToString("MM/dd/yyyy");
                        renewalDateCell.StyleName = tableCellStyle.Name;
                        renewalDateCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        var completeCell = sheet.Cells[rowNumber, 5];
                        completeCell.Value = achievement.IsCompetent ? "yes" : "no";
                        completeCell.StyleName = tableCellStyle.Name;
                        completeCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Row(rowNumber++).Style.WrapText = true;
                    }

                    sheet.Row(rowNumber++).Height = 20;
                }

                excel.Workbook.Properties.Title = Route.Title;

                excel.Workbook.Properties.Company = Organization.Name;
                excel.Workbook.Properties.Author = User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                if (rowNumber > 1)
                    sheet.PrinterSettings.PrintArea = sheet.Cells[1, 1, rowNumber - 1, 5];

                sheet.PrinterSettings.Orientation = eOrientation.Portrait;
                sheet.PrinterSettings.PaperSize = ePaperSize.A4;
                sheet.PrinterSettings.FitToPage = true;
                sheet.PrinterSettings.FitToWidth = 1;
                sheet.PrinterSettings.FitToHeight = 0;

                ReportXlsxHelper.Export(Route.Title, excel);
            }
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                Departments = FindDepartment.Values,
                Achievements = FindAchievement.Values,
                Learners = FindLearner.Values,
                IsRequired = GetIsRequired()
            };

            IEnumerable<UserDataItem> dataSource;

            try
            {
                dataSource = GetReportDataSource();
            }
            catch (EntityCommandExecutionException ecex)
            {
                if (ecex.InnerException is SqlException sqex && sqex.Number == -2)
                {
                    ScreenStatus.AddMessage(
                        AlertType.Error,
                        "The report generation took too long to complete. Please try selecting fewer items.");
                    return;
                }

                throw ecex;
            }

            if (!dataSource.Any())
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            UserRepeater.DataSource = dataSource;
            UserRepeater.DataBind();
        }

        private IEnumerable<UserDataItem> GetReportDataSource()
        {
            return CmdsReportHelper.SelectTrainingHistoryPerUser(CurrentParameters.Departments, CurrentParameters.Achievements, CurrentParameters.Learners, CurrentParameters.IsRequired, CurrentParameters.AchievementType)
                .GroupBy(x => x.PersonFullName)
                .Select(userGroup =>
                {
                    var firstUser = userGroup.First();

                    return new UserDataItem
                    {
                        PersonFullName = firstUser.PersonFullName,
                        Achievements = userGroup.Select(x => new AchievementDataItem
                        {
                            AchievementTitle = x.ResourceTitle,
                            DateCompleted = x.DateCompleted,
                            ExpirationDate = x.ExpirationDate,
                            AccreditorName = x.AccreditorName,
                            IsCompetent = x.IsCompetent,
                            Score = x.Score
                        })
                    };
                })
                .ToArray();
        }

        #endregion

        #region Helper methods

        private bool? GetIsRequired()
        {
            return IsRequired.SelectedIndex > 0 ? bool.Parse(IsRequired.SelectedValue) : (bool?)null;
        }

        #endregion
    }
}