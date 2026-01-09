using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using DocumentFormat.OpenXml.Spreadsheet;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Common.Web;
using InSite.Common.Web.UI.Chart;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;
using Shift.Sdk.UI;
using Shift.Sdk.UI.Help;

using AlertType = Shift.Constant.AlertType;
using Color = System.Drawing.Color;

namespace InSite.Cmds.Admin.Reports.Forms
{
    public partial class ComplianceSummaryReport : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const int ChartOperationWeight = 12;
        private const string OperationWritingData = "Writing data to Excel file";
        private const string OperationAddingCharts = "Adding charts to Excel file";

        #endregion

        #region Classes

        [Flags]
        public enum MembershipEnum
        {
            None = 0,

            Department = 1,
            Organization = 2,
            Administration = 4
        }

        [Serializable]
        protected class SearchParameters
        {
            public string ReportTitle => GetReportTitle(IsShowStatus);
            public string ReportType { get; set; }
            public Guid OrganizationIdentifier { get; set; }
            public Guid[] Departments { get; set; }
            public Guid[] Employees { get; set; }
            public MembershipEnum MembershipType { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int[] Sequences { get; set; }
            public int Option { get; set; }
            public bool ExcludeUsersWithoutProfile { get; set; }
            public bool IsShowWorkers { get; set; }
            public bool IsShowStatus { get; set; }
            public bool IsShowChart { get; set; }
            public bool IsIncludeUsersWithoutProfile { get; set; }
        }

        #endregion

        #region Properties

        protected SearchParameters ReportParameters
        {
            get => (SearchParameters)ViewState[nameof(ReportParameters)];
            set => ViewState[nameof(ReportParameters)] = value;
        }

        protected SearchParameters DownloadParameters
        {
            get => (SearchParameters)ViewState[nameof(DownloadParameters)];
            set => ViewState[nameof(DownloadParameters)] = value;
        }

        protected Guid CurrentDepartmentIdentifier
        {
            get => ViewState[nameof(CurrentDepartmentIdentifier)] as Guid? ?? Guid.Empty;
            set => ViewState[nameof(CurrentDepartmentIdentifier)] = value;
        }

        private List<CmdsReportHelper.EmployeeComplianceDepartment> Departments
        {
            get => (List<CmdsReportHelper.EmployeeComplianceDepartment>)ViewState[nameof(Departments)];
            set => ViewState[nameof(Departments)] = value;
        }

        #endregion

        #region Fields

        private Dictionary<string, string> _achievementDisplayMapping = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // The default timeout is 110 seconds. This report can take a long time to run so we are allowing 3 minutes.
            Page.Server.ScriptTimeout = 3 * 60;

            EmployeeIdentifierValidataor.ServerValidate += (s, a) => a.IsValid = EmployeeIdentifier.Enabled;

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) => UpdateEmployeeIdentifier();

            MembershipTypeChanged.Click += (s, a) => UpdateEmployeeIdentifier();

            ReportType.AutoPostBack = true;
            ReportType.SelectedIndexChanged += ReportType_SelectedIndexChanged;

            ReportButton.Click += ReportButton_Click;

            DownloadPdf.Click += DownloadPdf_Click;
            PreDownloadXlsx1.Click += PreDownloadXlsx1_Click;
            PreDownloadXlsx2.Click += PreDownloadXlsx2_Click;
            DownloadXlsx.Click += DownloadXlsx_Click;

            DepartmentRepeater.ItemCommand += DepartmentRepeater_ItemCommand;

            CurrentDepartmentRepeaterWithoutStatuses.ItemCreated += CurrentDepartmentRepeater_ItemCreated;
            CurrentDepartmentRepeaterWithoutStatuses.ItemDataBound += CurrentDepartmentRepeater_ItemDataBound;

            CurrentDepartmentRepeaterWithStatuses.ItemCreated += CurrentDepartmentRepeater_ItemCreated;
            CurrentDepartmentRepeaterWithStatuses.ItemDataBound += CurrentDepartmentRepeater_ItemDataBound;

            HistoryDepartmentRepeaterWithoutStatuses.ItemCreated += HistoryDepartmentRepeater_ItemCreated;
            HistoryDepartmentRepeaterWithoutStatuses.ItemDataBound += HistoryDepartmentRepeater_ItemDataBound;

            HistoryDepartmentRepeaterWithStatuses.ItemCreated += HistoryDepartmentRepeater_ItemCreated;
            HistoryDepartmentRepeaterWithStatuses.ItemDataBound += HistoryDepartmentRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);
            PageHelper.OverrideTitle(this, GetReportTitle(false));

            EnsureAchievementDisplayMapping();
            LoadReportType();
            LoadAchievementTypes();
            UpdateEmployeeIdentifier();

            if (!CmdsReportHelper.HasComplianceSummary())
            {
                NavPanel.Visible = false;
                ScreenStatus.AddMessage(
                    AlertType.Warning,
                    "The data for the compliance summary report is being refreshed. Please wait a few minutes and try again.");
            }
        }

        private void LoadReportType()
        {
            ReportType.Items.Clear();
            ReportType.Items.Add(new System.Web.UI.WebControls.ListItem("Current compliance", "Current"));
            ReportType.Items.Add(new System.Web.UI.WebControls.ListItem("Current and past compliance", "CurrentAndPast"));
            ReportType.Items.Add(new System.Web.UI.WebControls.ListItem("Current and past 3 months compliance", "CurrentAndPastQuarter"));

            ReportType.Items[0].Selected = true;
        }

        private void LoadAchievementTypes()
        {
            var types = UserCompetencyRepository.SelectAchievementTypes(_achievementDisplayMapping);

            AchievementType.LoadItems(types);
            AchievementType.SelectAll();
        }

        private void UpdateEmployeeIdentifier()
        {
            EmployeeIdentifier.Values = null;
            EmployeeIdentifier.Filter.Departments = DepartmentIdentifier.Values;
            EmployeeIdentifier.Filter.RoleType = MembershipType.Items
                .Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Selected).Select(x => x.Value).ToArray();

            var hasData = EmployeeIdentifier.Filter.Departments.Length > 0
                && EmployeeIdentifier.Filter.RoleType.Length > 0
                && EmployeeIdentifier.GetCount() > 0;

            EmployeeIdentifier.Enabled = hasData;
            EmployeeIdentifier.EmptyMessage = hasData ? "All Employees" : "None";
            EmployeeIdentifier.CssClass = hasData ? "fe-not-empty" : string.Empty;
        }

        #endregion

        #region Event handlers

        private void ReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var isDateSelectorVisible = false;
            var isCurrentReportType = ReportType.SelectedValue == "Current";

            Option.Items[2].Enabled = isCurrentReportType;

            if (!isCurrentReportType)
            {
                if (Option.SelectedValue == "3")
                    Option.SelectedValue = "2";

                if (ReportType.SelectedValue == "CurrentAndPast")
                {
                    isDateSelectorVisible = true;

                    ZUserStatusSearch.GetMinimumAndMaximumDates(out var from, out var thru);

                    if (from.HasValue)
                        StartDate.Value = from.Value.DateTime;
                    if (thru.HasValue)
                        EndDate.Value = thru.Value.DateTime;
                }
                else if (ReportType.SelectedValue == "CurrentAndPastQuarter")
                {
                    isDateSelectorVisible = true;

                    StartDate.Value = DateTime.Today.AddMonths(-3);
                    EndDate.Value = DateTime.Today;
                }
            }

            HistoryPanel.Visible = isDateSelectorVisible;
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void DownloadPdf_Click(object sender, EventArgs e)
        {
            var reportHtml = ReportHtmlContent.Value;
            if (reportHtml.IsNotEmpty())
                DownloadToPdf(ReportParameters);
        }

        private void PreDownloadXlsx1_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            DownloadParameters = CreateParameters();

            OnPreDownload();
        }

        private void PreDownloadXlsx2_Click(object sender, EventArgs e)
        {
            DownloadParameters = ReportParameters;

            OnPreDownload();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            if (DownloadParameters == null)
                return;

            EnsureAchievementDisplayMapping();

            ProgressCallback("Reading data", 0, 1);

            using (var excel = new ExcelPackage())
            {
                var redStyle = excel.Workbook.Styles.CreateNamedStyle("RedStyle");
                redStyle.Style.Font.Color.SetColor(Color.Red);

                excel.Workbook.CalcMode = ExcelCalcMode.Manual;

                if (!GenerateXlsx(DownloadParameters, excel))
                {
                    ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                    return;
                }

                excel.Workbook.CalcMode = ExcelCalcMode.Automatic;

                ProgressCallback("OperationComplete", 0, 0, true);

                ReportXlsxHelper.Export(DownloadParameters.ReportTitle, excel);
            }
        }

        private void OnPreDownload()
        {
            Page.ClientScript.RegisterStartupScript(
                typeof(ComplianceSummaryReport),
                "Download",
                $@"
$(window).one('load', function () {{
    document.getElementById('{DownloadXlsx.ClientID}').click();
}});",
                true);
        }

        private void DepartmentRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Show")
            {
                var department = Guid.Parse(e.CommandArgument.ToString());

                LoadDepartmentReport(ReportParameters, department);

                Page.MaintainScrollPositionOnPostBack = false;
            }
        }

        private void CurrentDepartmentRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var employeeRepeater = (Repeater)e.Item.FindControl("EmployeeRepeater");
            employeeRepeater.ItemDataBound += CurrentDepartmentEmployeeRepeater_ItemDataBound;

            var departmentChart = (BarChart)e.Item.FindControl("DepartmentChart");
            departmentChart.Options.Animation.Duration = 0;
        }

        private void CurrentDepartmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var department = (ComplianceSummaryReportHelper.DepartmentInfo)e.Item.DataItem;
            var table = (ComplianceSummaryReportHelper.CurrentReportDataSource)department.Root;

            var departmentAchievements = table.GetAchievements(department);

            var achievementRepeater = (Repeater)e.Item.FindControl("AchievementRepeater");
            achievementRepeater.DataSource = departmentAchievements;
            achievementRepeater.DataBind();

            var employeeRepeater = (Repeater)e.Item.FindControl("EmployeeRepeater");
            employeeRepeater.Visible = ReportParameters.IsShowWorkers;
            employeeRepeater.DataSource = ReportParameters.IsShowWorkers ? department.Children : null;
            employeeRepeater.DataBind();

            var departmentChart = (BarChart)e.Item.FindControl("DepartmentChart");
            departmentChart.Data.Clear();
            departmentChart.Parent.Visible = ReportParameters.IsShowChart;

            if (ReportParameters.IsShowChart)
            {
                var dataset = (BarChartDataset)departmentChart.Data.CreateDataset("achievements");
                foreach (var achievementInfo in departmentAchievements)
                    if (achievementInfo.Data != null)
                    {
                        var item = dataset.NewItem(achievementInfo.Info.Name, (double)(achievementInfo.Data.Score * 100 ?? 0));
                        item.BackgroundColor = GetScoreColor(achievementInfo.Data.Score);
                    }
            }
        }

        private void CurrentDepartmentEmployeeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var employee = (ComplianceSummaryReportHelper.EmployeeInfo)e.Item.DataItem;
            var table = (ComplianceSummaryReportHelper.CurrentReportDataSource)employee.Root;

            var achievementRepeater = (Repeater)e.Item.FindControl("AchievementRepeater");
            achievementRepeater.DataSource = table.GetAchievements(employee);
            achievementRepeater.DataBind();
        }

        private void HistoryDepartmentRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var employeeRepeater = (Repeater)e.Item.FindControl("EmployeeRepeater");
            employeeRepeater.ItemDataBound += HistoryDepartmentEmployeeRepeater_ItemDataBound;

            var departmentChart = (LineChart)e.Item.FindControl("DepartmentChart");
            departmentChart.Options.Animation.Duration = 0;
            departmentChart.Options.Plugins.Tooltip.Callbacks.LabelJsFunction = "report.onHistoryChartTooltipLabelCallback";
        }

        private void HistoryDepartmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var department = (ComplianceSummaryReportHelper.DepartmentInfo)e.Item.DataItem;
            var table = (ComplianceSummaryReportHelper.HistoryReportDataSource)department.Root;

            var employeeRepeater = (Repeater)e.Item.FindControl("EmployeeRepeater");
            employeeRepeater.Visible = ReportParameters.IsShowWorkers;
            employeeRepeater.DataSource = ReportParameters.IsShowWorkers ? department.Children : null;
            employeeRepeater.DataBind();

            var departmentChart = (LineChart)e.Item.FindControl("DepartmentChart");
            departmentChart.Data.Clear();
            departmentChart.Parent.Visible = ReportParameters.IsShowChart;

            var organization = Organization.Code;

            var repeaterData = new List<ComplianceSummaryReportHelper.HistoryDepartmentChartDataItem>();

            foreach (var achievementGroup in department.ChartData.GroupBy(x => x.Sequence).OrderBy(x => x.Key))
            {
                var heading = achievementGroup.First().Heading;
                var achievementDisplay = _achievementDisplayMapping.GetOrDefault(heading, heading);
                var chartDataItem = new ComplianceSummaryReportHelper.HistoryDepartmentChartDataItem { Name = achievementDisplay };

                if (ReportParameters.IsShowChart)
                {
                    var achievementTypeColor = ColorTranslator.FromHtml(AchievementTypes.RetrieveColor(achievementGroup.Key - 1));

                    var dataset = (LineChartDataset)departmentChart.Data.CreateDataset("achievement_" + achievementGroup.Key);
                    dataset.Label = chartDataItem.Name;
                    dataset.BackgroundColor = achievementTypeColor;
                    dataset.BorderColor = dataset.BackgroundColor;
                    dataset.BorderWidth = 2;
                    dataset.Fill = false;
                    dataset.LineTension = 0.05M;
                    dataset.PointRadius = 2;

                    foreach (var groupDataItem in achievementGroup)
                    {
                        var label = groupDataItem.SnapshotDate.HasValue ? $"{groupDataItem.SnapshotDate:MMM d, yyy}" : "Current";

                        dataset.NewItem(label, (double)(groupDataItem.Score ?? 0));

                        SetChartDataItemScore(chartDataItem, groupDataItem.SnapshotDate, groupDataItem.Score);
                    }
                }
                else
                {
                    foreach (var dataItem in achievementGroup)
                        SetChartDataItemScore(chartDataItem, dataItem.SnapshotDate, dataItem.Score);
                }

                repeaterData.Add(chartDataItem);
            }

            var achievementRepeater = (Repeater)e.Item.FindControl("AchievementRepeater");
            achievementRepeater.DataSource = repeaterData;
            achievementRepeater.DataBind();

            void SetChartDataItemScore(ComplianceSummaryReportHelper.HistoryDepartmentChartDataItem dataItem, DateTime? date, decimal? score)
            {
                if (!date.HasValue)
                    dataItem.Score = score;
                else if (date.Value == table.SnapshotDate1)
                    dataItem.Score1 = score;
                else if (date.Value == table.SnapshotDate2)
                    dataItem.Score2 = score;
                else if (date.Value == table.SnapshotDate3)
                    dataItem.Score3 = score;
            }
        }

        private void HistoryDepartmentEmployeeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var employee = (ComplianceSummaryReportHelper.EmployeeInfo)e.Item.DataItem;
            var table = (ComplianceSummaryReportHelper.HistoryReportDataSource)employee.Root;

            var achievementRepeater = (Repeater)e.Item.FindControl("AchievementRepeater");
            achievementRepeater.DataSource = table.GetAchievements(employee);
            achievementRepeater.DataBind();
        }

        #endregion

        #region Data binding

        private SearchParameters CreateParameters()
        {
            var result = new SearchParameters
            {
                ReportType = ReportType.SelectedValue,
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                Departments = DepartmentIdentifier.Values,
                Employees = EmployeeIdentifier.Values,
                MembershipType = MembershipEnum.None,
                StartDate = StartDate.Value,
                EndDate = EndDate.Value,
                Sequences = AchievementType.ValuesAsInt.ToArray(),
                Option = int.Parse(Option.SelectedValue),
                ExcludeUsersWithoutProfile = !IsIncludeUsersWithoutProfile.Checked,
                IsShowWorkers = int.Parse(DisplayMode.SelectedValue) == 0,
                IsShowStatus = IsShowStatus.Checked,
                IsShowChart = IsShowChart.Checked
            };

            if (result.Employees.Length == 0)
                result.Employees = EmployeeIdentifier.GetDataItems().Select(x => x.Value).ToArray();

            foreach (System.Web.UI.WebControls.ListItem item in MembershipType.Items)
            {
                if (!item.Selected)
                    continue;

                if (item.Value == "Department")
                    result.MembershipType |= MembershipEnum.Department;
                else if (item.Value == "Organization")
                    result.MembershipType |= MembershipEnum.Organization;
                else if (item.Value == "Administration")
                    result.MembershipType |= MembershipEnum.Administration;
            }

            return result;
        }

        private void LoadReport()
        {
            var parameters = ReportParameters = CreateParameters();

            Departments = CmdsReportHelper.SelectEmployeeComplianceDepartment(
                parameters.OrganizationIdentifier,
                parameters.Departments,
                parameters.Employees,
                parameters.Sequences,
                parameters.StartDate,
                parameters.EndDate,
                parameters.Option,
                parameters.ExcludeUsersWithoutProfile,
                parameters.MembershipType.HasFlag(MembershipEnum.Department),
                parameters.MembershipType.HasFlag(MembershipEnum.Organization),
                parameters.MembershipType.HasFlag(MembershipEnum.Administration)
            );

            LoadDepartmentReport(parameters, Departments.FirstOrDefault()?.DepartmentIdentifier ?? Guid.Empty);
        }

        private void LoadDepartmentReport(SearchParameters parameters, Guid department)
        {
            EnsureAchievementDisplayMapping();

            CurrentDepartmentIdentifier = department;

            DepartmentRepeater.DataSource = Departments;
            DepartmentRepeater.DataBind();

            CurrentDepartmentRepeaterWithoutStatuses.Visible = false;
            CurrentDepartmentRepeaterWithStatuses.Visible = false;
            HistoryDepartmentRepeaterWithoutStatuses.Visible = false;
            HistoryDepartmentRepeaterWithStatuses.Visible = false;

            if (parameters.ReportType == "Current")
                LoadCurrentReport(parameters);
            else
                LoadHistoryReport(parameters);

            CurrentDepartmentRepeaterWithoutStatuses.DataBind();
            CurrentDepartmentRepeaterWithStatuses.DataBind();
            HistoryDepartmentRepeaterWithoutStatuses.DataBind();
            HistoryDepartmentRepeaterWithStatuses.DataBind();

            DepartmentNoDataAlert.Visible = CurrentDepartmentRepeaterWithoutStatuses.Items.Count == 0
                && CurrentDepartmentRepeaterWithStatuses.Items.Count == 0
                && HistoryDepartmentRepeaterWithoutStatuses.Items.Count == 0
                && HistoryDepartmentRepeaterWithStatuses.Items.Count == 0;
        }

        private void LoadCurrentReport(SearchParameters parameters)
        {
            var reportDataList = GetCurrentReportDataSource(parameters, new[] { CurrentDepartmentIdentifier });
            var reportData = reportDataList.Count > 0 ? reportDataList[0] : new ComplianceSummaryReportHelper.CurrentReportDataSource();

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;
            DownloadCommandsPanel.Visible = true;

            CurrentDepartmentRepeaterWithoutStatuses.Visible = !parameters.IsShowStatus;
            CurrentDepartmentRepeaterWithStatuses.Visible = parameters.IsShowStatus;

            if (parameters.IsShowStatus)
                CurrentDepartmentRepeaterWithStatuses.DataSource = reportData.Columns;
            else
                CurrentDepartmentRepeaterWithoutStatuses.DataSource = reportData.Columns;
        }

        private void LoadHistoryReport(SearchParameters parameters)
        {
            var reportDataList = GetHistoryReportDataSource(parameters, new[] { CurrentDepartmentIdentifier });
            var reportData = reportDataList.Count > 0 ? reportDataList[0] : new ComplianceSummaryReportHelper.HistoryReportDataSource();

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DownloadCommandsPanel.Visible = true;

            HistoryDepartmentRepeaterWithoutStatuses.Visible = !parameters.IsShowStatus;
            HistoryDepartmentRepeaterWithStatuses.Visible = parameters.IsShowStatus;

            if (parameters.IsShowStatus)
                HistoryDepartmentRepeaterWithStatuses.DataSource = reportData.Columns;
            else
                HistoryDepartmentRepeaterWithoutStatuses.DataSource = reportData.Columns;
        }

        private List<ComplianceSummaryReportHelper.CurrentReportDataSource> GetCurrentReportDataSource(SearchParameters parameters, Guid[] departments)
        {
            var rows = CmdsReportHelper.SelectComplianceSummary(
                parameters.OrganizationIdentifier,
                departments,
                parameters.Employees,
                parameters.Sequences,
                parameters.Option,
                parameters.ExcludeUsersWithoutProfile
            );

            var isDepartmentEmployment = parameters.MembershipType.HasFlag(MembershipEnum.Department);
            var isCompanyEmployment = parameters.MembershipType.HasFlag(MembershipEnum.Organization);
            var isDataAccess = parameters.MembershipType.HasFlag(MembershipEnum.Administration);

            var organization = Organization.Code;

            var result = new List<ComplianceSummaryReportHelper.CurrentReportDataSource>();
            var map = new Dictionary<Guid, ComplianceSummaryReportHelper.CurrentReportDataSource>();

            foreach (var row in rows)
            {
                if (UserIsInDepartment(row.UserIdentifier, row.DepartmentIdentifier, isDepartmentEmployment, isCompanyEmployment, isDataAccess))
                {

                    if (!map.TryGetValue(row.DepartmentIdentifier, out var departmentResult))
                    {
                        map.Add(row.DepartmentIdentifier, departmentResult = new ComplianceSummaryReportHelper.CurrentReportDataSource());
                        result.Add(departmentResult);
                    }

                    var department = departmentResult.Columns.GetOrAdd(
                        () => new ComplianceSummaryReportHelper.DepartmentInfo(row.CompanyName, row.DepartmentName),
                        row.DepartmentIdentifier);

                    var employee = department.Children.GetOrAdd(
                        () => new ComplianceSummaryReportHelper.EmployeeInfo
                        {
                            Text = row.UserFullName,
                            PrimaryProfileName = row.PrimaryProfileIdentifier.HasValue
                                ? row.PrimaryProfileTitle
                                : null
                        },
                        row.UserIdentifier);

                    var achievementDisplay = _achievementDisplayMapping.GetOrDefault(row.Heading, row.Heading);
                    var achievement = departmentResult.Rows.GetOrAdd(
                        () => new ComplianceSummaryReportHelper.AchievementInfo(row.Sequence, achievementDisplay),
                        row.Sequence);

                    departmentResult.AddCell(department, achievement, () => new ComplianceSummaryReportHelper.CurrentAchievementData(row), r => r.Append(row));
                    departmentResult.AddCell(employee, achievement, () => new ComplianceSummaryReportHelper.CurrentAchievementData(row), r => r.Append(row));
                }
            }

            result.Sort((x1, x2) => x1.Columns[0].DepartmentName.CompareTo(x2.Columns[0].DepartmentName));

            return result;
        }

        private bool UserIsInDepartment(Guid user, Guid department, bool isDepartment, bool isCompany, bool isAdmin)
        {
            var roleTypes = new List<string>();

            if (isDepartment)
                roleTypes.Add("Department");

            if (isCompany)
                roleTypes.Add("Organization");

            if (isAdmin)
                roleTypes.Add("Administration");

            if (roleTypes.Count == 0)
                return false;

            return MembershipSearch.Exists(
                x => x.UserIdentifier == user
                  && x.GroupIdentifier == department
                  && roleTypes.Contains(x.MembershipType));
        }

        private List<ComplianceSummaryReportHelper.HistoryReportDataSource> GetHistoryReportDataSource(SearchParameters parameters, Guid[] departments)
        {
            var rows = CmdsReportHelper.SelectEmployeeComplianceHistory(
                parameters.OrganizationIdentifier,
                departments,
                parameters.Employees,
                parameters.Sequences,
                parameters.StartDate.Value,
                parameters.EndDate.Value,
                parameters.Option,
                parameters.ExcludeUsersWithoutProfile
            );

            var chartRows = CmdsReportHelper.SelectEmployeeComplianceHistoryChart(
                parameters.OrganizationIdentifier,
                departments,
                parameters.Employees,
                parameters.Sequences,
                parameters.StartDate.Value,
                parameters.EndDate.Value,
                parameters.Option,
                parameters.ExcludeUsersWithoutProfile
            ).GroupBy(x => x.DepartmentIdentifier)
            .ToDictionary(
                x => x.Key,
                x => x.OrderByDescending(y => y.SnapshotDate.HasValue).ThenBy(y => y.SnapshotDate).ToArray()
            );

            NormalizeChartRows(chartRows);

            var organization = Organization.Code;

            var result = new List<ComplianceSummaryReportHelper.HistoryReportDataSource>();
            var map = new Dictionary<Guid, ComplianceSummaryReportHelper.HistoryReportDataSource>();

            foreach (var row in rows)
            {
                if (!map.TryGetValue(row.DepartmentIdentifier, out var departmentResult))
                {
                    map.Add(row.DepartmentIdentifier, departmentResult = new ComplianceSummaryReportHelper.HistoryReportDataSource());
                    result.Add(departmentResult);
                }

                if (!departmentResult.SnapshotDate1.HasValue && row.SnapshotDate1.HasValue)
                    departmentResult.SnapshotDate1 = row.SnapshotDate1.Value;

                if (!departmentResult.SnapshotDate2.HasValue && row.SnapshotDate2.HasValue)
                    departmentResult.SnapshotDate2 = row.SnapshotDate2.Value;

                if (!departmentResult.SnapshotDate3.HasValue && row.SnapshotDate3.HasValue)
                    departmentResult.SnapshotDate3 = row.SnapshotDate3.Value;

                var department = departmentResult.Columns.GetOrAdd(
                    () => new ComplianceSummaryReportHelper.DepartmentInfo(row.CompanyName, row.DepartmentName)
                    {
                        ChartData = chartRows.ContainsKey(row.DepartmentIdentifier)
                            ? chartRows[row.DepartmentIdentifier]
                            : new CmdsReportHelper.EmployeeComplianceHistoryChart[0]
                    },
                    row.DepartmentIdentifier);

                var employee = department.Children.GetOrAdd(
                    () =>
                    {
                        var employeeInfo = new ComplianceSummaryReportHelper.EmployeeInfo { Text = row.UserFullName };

                        if (row.PrimaryProfileIdentifier.HasValue)
                            employeeInfo.PrimaryProfileName = row.PrimaryProfileTitle;

                        return employeeInfo;
                    },
                    row.UserIdentifier);

                var achievementDisplay = _achievementDisplayMapping.GetOrDefault(row.Heading, row.Heading);
                var achievement = departmentResult.Rows.GetOrAdd(
                    () => new ComplianceSummaryReportHelper.AchievementInfo(row.Sequence, achievementDisplay),
                    row.Sequence);

                departmentResult.AddCell(employee, achievement, () => new ComplianceSummaryReportHelper.HistoryAchievementData(row), r => r.Append(row));
            }

            result.Sort((x1, x2) => x1.Columns[0].DepartmentName.CompareTo(x2.Columns[0].DepartmentName));

            return result;
        }

        private static void NormalizeChartRows(Dictionary<Guid, CmdsReportHelper.EmployeeComplianceHistoryChart[]> chartRows)
        {
            var dateSet = new HashSet<DateTime?>();

            foreach (var item in chartRows)
            {
                foreach (var chartRow in item.Value)
                {
                    if (chartRow.SnapshotDate.HasValue)
                        dateSet.Add(chartRow.SnapshotDate.Value.Date);
                }
            }

            var dateList = dateSet.ToList();
            dateList.Sort();
            dateList.Add(null);

            var departments = chartRows.Keys.ToList();
            foreach (var departmentKey in departments)
            {
                var chartRowItems = chartRows[departmentKey];

                if (chartRowItems.Length == dateList.Count)
                    continue;

                var headingRowsDict = chartRowItems
                    .GroupBy(x => x.Heading)
                    .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.SnapshotDate.HasValue).ThenBy(y => y.SnapshotDate).ToList());

                var newChartItems = new List<CmdsReportHelper.EmployeeComplianceHistoryChart>();

                foreach (var heading in headingRowsDict.Keys)
                {
                    var chartItems = headingRowsDict[heading];
                    var chartItemIndex = 0;

                    foreach (var date in dateList)
                    {
                        var chartItem = chartItemIndex < chartItems.Count ? chartItems[chartItemIndex] : null;

                        if (chartItem != null && date == chartItem.SnapshotDate?.Date)
                        {
                            newChartItems.Add(chartItem);
                            chartItemIndex++;
                        }
                        else
                        {
                            newChartItems.Add(new CmdsReportHelper.EmployeeComplianceHistoryChart
                            {
                                DepartmentIdentifier = departmentKey,
                                Sequence = chartItems[0].Sequence,
                                Heading = heading,
                                SnapshotDate = date,
                                Score = null
                            });
                        }
                    }
                }

                chartRows[departmentKey] = newChartItems.ToArray();
            }
        }

        #endregion

        #region Methods (export PDF)

        private void DownloadToPdf(SearchParameters parameters)
        {
            var physPath = MapPath("~/UI/CMDS/Admin/Reports/ComplianceSummaryReport_PdfBody.html");
            var bodyHtml = File.ReadAllText(physPath);
            bodyHtml = bodyHtml
                .Replace("<!-- TITLE -->", parameters.ReportTitle)
                .Replace("<!-- BODY -->", ReportHtmlContent.Value);
            bodyHtml = HtmlHelper.ResolveRelativePaths(Page.Request.Url.Scheme + "://" + Page.Request.Url.Host + Page.Request.RawUrl, bodyHtml);

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),

                MarginTop = 22,

                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/CMDS/Admin/Reports/ComplianceSummaryReport_PdfHeader.html"),
                HeaderSpacing = 7
            };

            var data = HtmlConverter.HtmlToPdf(bodyHtml, settings);
            if (data == null)
                return;

            var filename = StringHelper.Sanitize(parameters.ReportTitle, '-', false);

            Response.SendFile(filename, "pdf", data);
        }

        #endregion

        #region Methods (export XLSX)

        private bool GenerateXlsx(SearchParameters parameters, ExcelPackage excel)
        {
            var blueColor = ColorTranslator.FromHtml("#3d78d8");
            var cellOddColor = ColorTranslator.FromHtml("#f1f1f1");
            var cellEvenColor = ColorTranslator.FromHtml("#fafafa");

            #region Styles Definition

            // Style: Default

            {
                var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                defaultStyle.Font.Name = "Arial";
                defaultStyle.Font.Size = 10;
                defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            // Style: Blue Bold

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Blue Bold");
                newStyle.Style.Font.Color.SetColor(Color.White);
                newStyle.Style.Font.Bold = true;
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(blueColor);
                newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Style: Blue Bold Small

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Blue Bold Small");
                newStyle.Style.Font.Color.SetColor(Color.White);
                newStyle.Style.Font.Bold = true;
                newStyle.Style.Font.Size = 8;
                newStyle.Style.WrapText = true;
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(blueColor);
                newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Style: Blue Bold Big

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Blue Bold Big");
                newStyle.Style.Font.Size = 18;
                newStyle.Style.Font.Color.SetColor(Color.White);
                newStyle.Style.Font.Bold = true;
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(blueColor);
                newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Style: Blue Bold Centered

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Blue Bold Centered");
                newStyle.Style.Font.Color.SetColor(Color.White);
                newStyle.Style.Font.Bold = true;
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(blueColor);
                newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
            }

            // Style: Blue Bold Small Centered

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Blue Bold Small Centered");
                newStyle.Style.Font.Color.SetColor(Color.White);
                newStyle.Style.Font.Bold = true;
                newStyle.Style.Font.Size = 8;
                newStyle.Style.WrapText = true;
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(blueColor);
                newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
            }

            // Style: White Bold

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("White Bold");
                newStyle.Style.Font.Bold = true;
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(Color.White);
                newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Style: White Bold BT

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("White Bold BT");
                newStyle.Style.Font.Bold = true;
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(Color.White);
                newStyle.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                newStyle.Style.Border.Top.Color.SetColor(Color.Black);
                newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Style: White Bold Centered BT

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("White Bold Centered BT");
                newStyle.Style.Font.Bold = true;
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(Color.White);
                newStyle.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                newStyle.Style.Border.Top.Color.SetColor(Color.Black);
                newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Style: White Bold Small Centered BT BR BB BL

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("White Bold Small Centered BT BR BB BL");
                newStyle.Style.Font.Bold = true;
                newStyle.Style.Font.Size = 8;
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(Color.White);
                newStyle.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                newStyle.Style.Border.Top.Color.SetColor(Color.Black);
                newStyle.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                newStyle.Style.Border.Right.Color.SetColor(Color.Black);
                newStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                newStyle.Style.Border.Bottom.Color.SetColor(Color.Black);
                newStyle.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                newStyle.Style.Border.Left.Color.SetColor(Color.Black);
                newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Style: Odd

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Odd");
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(cellOddColor);
            }

            // Style: Even

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Even");
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(cellEvenColor);
            }

            // Style: Odd Centered

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Odd Centered");
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(cellOddColor);
                newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Style: Even Centered

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Even Centered");
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(cellEvenColor);
                newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Style: Separator

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Separator");
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(Color.White);
            }

            // Style: Timestamp

            {
                var newStyle = excel.Workbook.Styles.CreateNamedStyle("Timestamp");
                newStyle.Style.Font.Color.SetColor(blueColor);
                newStyle.Style.Font.Size = 8;
                newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                newStyle.Style.Fill.BackgroundColor.SetColor(Color.White);
                newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            }

            #endregion

            var hasData = parameters.ReportType == "Current"
                ? GenerateXlsxCurrent(parameters, excel)
                : GenerateXlsxHistory(parameters, excel);

            if (!hasData)
                return false;

            excel.Workbook.Properties.Title = parameters.ReportTitle;
            excel.Workbook.Properties.Company = Organization.Name;
            excel.Workbook.Properties.Author = User.FullName;
            excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

            foreach (var sheet in excel.Workbook.Worksheets)
            {
                sheet.PrinterSettings.Orientation = eOrientation.Portrait;
                sheet.PrinterSettings.PaperSize = ePaperSize.A4;
                sheet.PrinterSettings.FitToPage = true;
                sheet.PrinterSettings.FitToWidth = 1;
                sheet.PrinterSettings.FitToHeight = 0;
            }

            return true;
        }

        #region Current

        private bool GenerateXlsxCurrent(SearchParameters parameters, ExcelPackage excel)
        {
            var data = GetCurrentReportDataSource(parameters, parameters.Departments);

            if (data.Count == 0)
                return false;

            if (parameters.IsShowStatus)
                GenerateXlsxCurrentWithStatus(parameters, excel, data);
            else
                GenerateXlsxCurrentWithoutStatus(parameters, excel, data);

            return true;
        }

        private void GenerateXlsxCurrentWithoutStatus(SearchParameters parameters, ExcelPackage excel, List<ComplianceSummaryReportHelper.CurrentReportDataSource> data)
        {
            foreach (var departmentData in data)
            {
                var sheetName = departmentData.Columns[0].DepartmentName;
                var sheet = excel.Workbook.Worksheets.Add(sheetName);

                GenerateXlsxCurrentWithoutStatus(parameters, sheet, departmentData);
            }
        }

        private ExcelWorksheet GenerateXlsxCurrentWithoutStatus(SearchParameters parameters, ExcelWorksheet sheet, ComplianceSummaryReportHelper.CurrentReportDataSource data)
        {
            sheet.Workbook.CalcMode = ExcelCalcMode.Manual;

            sheet.Column(1).Width = 30;
            sheet.Column(2).Width = 30;
            sheet.Column(3).Width = 10;
            sheet.Column(4).Width = 10;
            sheet.Column(5).Width = 10;
            sheet.Column(6).Width = 10;
            sheet.Column(7).Width = 10;

            var rowNumber = 1;
            var isFirstRow1 = true;
            var operationNumber = 0;
            var operationCount = data.Columns.Count;

            if (parameters.IsShowChart)
                operationCount += data.Columns.Count * ChartOperationWeight;

            var charts = new List<ComplianceSummaryReportHelper.ChartData>();

            ComplianceSummaryReportHelper.Timestamp(sheet, ref rowNumber, 7);

            foreach (var departmentData in data.Columns)
            {
                ProgressCallback(OperationWritingData, operationNumber++, operationCount);

                if (!isFirstRow1)
                    rowNumber = ComplianceSummaryReportHelper.Separator(sheet, rowNumber, 7, 30);
                else
                    isFirstRow1 = false;

                ComplianceSummaryReportHelper.CompanyName(sheet, ref rowNumber, 7, departmentData.CompanyName);
                ComplianceSummaryReportHelper.ReportTitle(sheet, ref rowNumber, 7, parameters.ReportTitle);

                // Column Names

                {
                    var cell1 = sheet.Cells[rowNumber, 1];
                    cell1.Value = string.Empty;
                    cell1.StyleName = "Blue Bold";

                    var cell2 = sheet.Cells[rowNumber, 2];
                    cell2.Value = "Compliance Item";
                    cell2.StyleName = "Blue Bold";

                    var cell3 = sheet.Cells[rowNumber, 3];
                    cell3.Value = "N/A";
                    cell3.StyleName = "Blue Bold Centered";

                    var cell4 = sheet.Cells[rowNumber, 4];
                    cell4.Value = "Submitted";
                    cell4.StyleName = "Blue Bold Centered";

                    var cell5 = sheet.Cells[rowNumber, 5];
                    cell5.Value = "Validated";
                    cell5.StyleName = "Blue Bold Centered";

                    var cell6 = sheet.Cells[rowNumber, 6];
                    cell6.Value = "Required";
                    cell6.StyleName = "Blue Bold Centered";

                    var cell7 = sheet.Cells[rowNumber, 7];
                    cell7.Value = "Score";
                    cell7.StyleName = "Blue Bold Centered";

                    rowNumber++;
                }

                ComplianceSummaryReportHelper.DepartmentName(sheet, ref rowNumber, 7, departmentData.DepartmentName);

                // Department Summary Table

                var achievements = data.GetAchievements(departmentData);

                AchievementTable(achievements);

                if (parameters.IsShowChart)
                {
                    rowNumber = ComplianceSummaryReportHelper.Separator(sheet, rowNumber, 7);
                    rowNumber = ComplianceSummaryReportHelper.AddBarChartData(sheet, rowNumber, departmentData.DepartmentName + " Chart", 7, achievements, charts);
                }

                if (parameters.IsShowWorkers)
                {
                    // Separator

                    rowNumber = ComplianceSummaryReportHelper.Separator(sheet, rowNumber, 7, 20);

                    // Employee Table

                    {
                        var isFirstRow2 = true;

                        foreach (var employeeData in departmentData.Children)
                        {
                            if (!isFirstRow2)
                                rowNumber = ComplianceSummaryReportHelper.Separator(sheet, rowNumber, 7, 20);
                            else
                                isFirstRow2 = !isFirstRow2;

                            // Header

                            {
                                var row = sheet.Row(rowNumber);
                                row.Height = 20;

                                var cell1_2 = sheet.Cells[rowNumber, 1, rowNumber, 2];
                                cell1_2.Value = string.Empty;
                                cell1_2.StyleName = "White Bold Small Centered BT BR BB BL";
                                cell1_2.Merge = true;

                                var cell3 = sheet.Cells[rowNumber, 3];
                                cell3.Value = "NA";
                                cell3.StyleName = "White Bold Small Centered BT BR BB BL";

                                var cell4 = sheet.Cells[rowNumber, 4];
                                cell4.Value = "SV";
                                cell4.StyleName = "White Bold Small Centered BT BR BB BL";

                                var cell5 = sheet.Cells[rowNumber, 5];
                                cell5.Value = "Validated";
                                cell5.StyleName = "White Bold Small Centered BT BR BB BL";

                                var cell6 = sheet.Cells[rowNumber, 6];
                                cell6.Value = "Required";
                                cell6.StyleName = "White Bold Small Centered BT BR BB BL";

                                var cell7 = sheet.Cells[rowNumber, 7];
                                cell7.Value = "Score";
                                cell7.StyleName = "White Bold Small Centered BT BR BB BL";

                                rowNumber++;
                            }
                            ComplianceSummaryReportHelper.EmployeeName(sheet, ref rowNumber, 7, employeeData.Text, GetOptionText(parameters, employeeData));

                            AchievementTable(data.GetAchievements(employeeData));
                        }
                    }
                }
            }

            foreach (var chartData in charts)
            {
                ProgressCallback(OperationAddingCharts, operationNumber, operationCount);
                operationNumber += ChartOperationWeight;
                ComplianceSummaryReportHelper.AddBarChart(sheet, chartData);
            }

            if (rowNumber > 1)
                sheet.PrinterSettings.PrintArea = sheet.Cells[1, 1, rowNumber - 1, 7];

            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;

            return sheet;

            void AchievementTable(IEnumerable<ComplianceSummaryReportHelper.CurrentAchievementItem> achievements)
            {
                var number = 1;

                foreach (var item in achievements)
                {
                    string nameCellStyle, dataCellStyle;

                    if (number % 2 == 0)
                    {
                        nameCellStyle = "Even";
                        dataCellStyle = "Even Centered";
                    }
                    else
                    {
                        nameCellStyle = "Odd";
                        dataCellStyle = "Odd Centered";
                    }

                    var cell1 = sheet.Cells[rowNumber, 1];
                    cell1.Value = string.Empty;
                    cell1.StyleName = nameCellStyle;

                    var cell2 = sheet.Cells[rowNumber, 2];
                    cell2.Value = item.Info.Name;
                    cell2.StyleName = nameCellStyle;

                    if (item.Data != null)
                    {
                        var cell3 = sheet.Cells[rowNumber, 3];
                        cell3.Value = item.Data.NotApplicable;
                        cell3.StyleName = dataCellStyle;
                        cell3.Style.Numberformat.Format = "#,##0";

                        var cell4 = sheet.Cells[rowNumber, 4];
                        cell4.Value = item.Data.Submitted;
                        cell4.StyleName = dataCellStyle;
                        cell4.Style.Numberformat.Format = "#,##0";

                        var cell5 = sheet.Cells[rowNumber, 5];
                        cell5.Value = item.Data.Satisfied;
                        cell5.StyleName = dataCellStyle;
                        cell5.Style.Numberformat.Format = "#,##0";

                        var cell6 = sheet.Cells[rowNumber, 6];
                        cell6.Value = item.Data.Required;
                        cell6.StyleName = dataCellStyle;
                        cell6.Style.Numberformat.Format = "#,##0";

                        var cell7 = sheet.Cells[rowNumber, 7];
                        cell7.StyleName = dataCellStyle;
                        cell7.Style.Numberformat.Format = "0.0%";
                        cell7.Value = item.Data.Score == null ? "NA" : (object)(item.Data.Score);

                        if (item.Data.Score.HasValue)
                            cell7.Style.Font.Color.SetColor(item.Data.Score >= 1 ? Color.Green : Color.Red);
                    }

                    rowNumber++;
                    number++;
                }
            }
        }

        private void GenerateXlsxCurrentWithStatus(SearchParameters parameters, ExcelPackage excel, List<ComplianceSummaryReportHelper.CurrentReportDataSource> data)
        {
            foreach (var departmentData in data)
            {
                var sheetName = departmentData.Columns[0].DepartmentName;
                var sheet = excel.Workbook.Worksheets.Add(sheetName);

                GenerateXlsxCurrentWithStatus(parameters, sheet, departmentData);
            }
        }

        private void GenerateXlsxCurrentWithStatus(SearchParameters parameters, ExcelWorksheet sheet, ComplianceSummaryReportHelper.CurrentReportDataSource data)
        {
            sheet.Workbook.CalcMode = ExcelCalcMode.Manual;

            sheet.Column(1).Width = 30;
            sheet.Column(2).Width = 10;
            sheet.Column(3).Width = 10;
            sheet.Column(4).Width = 10;
            sheet.Column(5).Width = 10;
            sheet.Column(6).Width = 10;
            sheet.Column(7).Width = 10;
            sheet.Column(8).Width = 10;
            sheet.Column(9).Width = 10;
            sheet.Column(10).Width = 10;

            var rowNumber = 1;
            var isFirstRow1 = true;
            var operationNumber = 0;
            var operationCount = data.Columns.Count;

            if (parameters.IsShowChart)
                operationCount += data.Columns.Count * ChartOperationWeight;

            var charts = new List<ComplianceSummaryReportHelper.ChartData>();

            ComplianceSummaryReportHelper.Timestamp(sheet, ref rowNumber, 10);

            foreach (var departmentData in data.Columns)
            {
                ProgressCallback(OperationWritingData, operationNumber++, operationCount);

                if (!isFirstRow1)
                    rowNumber = ComplianceSummaryReportHelper.Separator(sheet, rowNumber, 10, 30);
                else
                    isFirstRow1 = false;

                ComplianceSummaryReportHelper.CompanyName(sheet, ref rowNumber, 10, departmentData.CompanyName);
                ComplianceSummaryReportHelper.ReportTitle(sheet, ref rowNumber, 10, parameters.ReportTitle);

                // Column Names

                {
                    var row = sheet.Row(rowNumber);
                    row.Height = 33;

                    var cell1 = sheet.Cells[rowNumber, 1];
                    cell1.Value = string.Empty;
                    cell1.StyleName = "Blue Bold";

                    var cell2 = sheet.Cells[rowNumber, 2];
                    cell2.Value = "Expired";
                    cell2.StyleName = "Blue Bold Small Centered";

                    var cell3 = sheet.Cells[rowNumber, 3];
                    cell3.Value = "Not Completed";
                    cell3.StyleName = "Blue Bold Small Centered";

                    var cell4 = sheet.Cells[rowNumber, 4];
                    cell4.Value = "Not Applicable";
                    cell4.StyleName = "Blue Bold Small Centered";

                    var cell5 = sheet.Cells[rowNumber, 5];
                    cell5.Value = "Needs Training";
                    cell5.StyleName = "Blue Bold Small Centered";

                    var cell6 = sheet.Cells[rowNumber, 6];
                    cell6.Value = "Self Assessed";
                    cell6.StyleName = "Blue Bold Small Centered";

                    var cell7 = sheet.Cells[rowNumber, 7];
                    cell7.Value = "Submitted for Validation";
                    cell7.StyleName = "Blue Bold Small Centered";

                    var cell8 = sheet.Cells[rowNumber, 8];
                    cell8.Value = "Validated";
                    cell8.StyleName = "Blue Bold Small Centered";

                    var cell9 = sheet.Cells[rowNumber, 9];
                    cell9.Value = "Required";
                    cell9.StyleName = "Blue Bold Small Centered";

                    var cell10 = sheet.Cells[rowNumber, 10];
                    cell10.Value = "Score";
                    cell10.StyleName = "Blue Bold Small Centered";

                    rowNumber++;
                }

                ComplianceSummaryReportHelper.DepartmentName(sheet, ref rowNumber, 10, departmentData.DepartmentName);

                // Department Summary Table

                var achievements = data.GetAchievements(departmentData);

                AchievementTable(achievements);

                var departmentSummaryEndRow = rowNumber - 1;

                if (parameters.IsShowChart)
                {
                    rowNumber = ComplianceSummaryReportHelper.Separator(sheet, rowNumber, 10);
                    rowNumber = ComplianceSummaryReportHelper.AddBarChartData(sheet, rowNumber, departmentData.DepartmentName + " Chart", 10, achievements, charts);
                }

                if (parameters.IsShowWorkers)
                {
                    rowNumber = ComplianceSummaryReportHelper.Separator(sheet, rowNumber, 10, 20);

                    var isFirstRow2 = true;

                    foreach (var employeeData in departmentData.Children)
                    {
                        if (!isFirstRow2)
                            rowNumber = ComplianceSummaryReportHelper.Separator(sheet, rowNumber, 10, 20);
                        else
                            isFirstRow2 = !isFirstRow2;

                        // Header

                        {
                            var row = sheet.Row(rowNumber);
                            row.Height = 20;

                            var cell1 = sheet.Cells[rowNumber, 1];
                            cell1.Value = string.Empty;
                            cell1.StyleName = "White Bold Small Centered BT BR BB BL";

                            var cell2 = sheet.Cells[rowNumber, 2];
                            cell2.Value = "Expired";
                            cell2.StyleName = "White Bold Small Centered BT BR BB BL";

                            var cell3 = sheet.Cells[rowNumber, 3];
                            cell3.Value = "NC";
                            cell3.StyleName = "White Bold Small Centered BT BR BB BL";

                            var cell4 = sheet.Cells[rowNumber, 4];
                            cell4.Value = "NA";
                            cell4.StyleName = "White Bold Small Centered BT BR BB BL";

                            var cell5 = sheet.Cells[rowNumber, 5];
                            cell5.Value = "NT";
                            cell5.StyleName = "White Bold Small Centered BT BR BB BL";

                            var cell6 = sheet.Cells[rowNumber, 6];
                            cell6.Value = "SA";
                            cell6.StyleName = "White Bold Small Centered BT BR BB BL";

                            var cell7 = sheet.Cells[rowNumber, 7];
                            cell7.Value = "SV";
                            cell7.StyleName = "White Bold Small Centered BT BR BB BL";

                            var cell8 = sheet.Cells[rowNumber, 8];
                            cell8.Value = "Validated";
                            cell8.StyleName = "White Bold Small Centered BT BR BB BL";

                            var cell9 = sheet.Cells[rowNumber, 9];
                            cell9.Value = "Required";
                            cell9.StyleName = "White Bold Small Centered BT BR BB BL";

                            var cell10 = sheet.Cells[rowNumber, 10];
                            cell10.Value = "Score";
                            cell10.StyleName = "White Bold Small Centered BT BR BB BL";

                            rowNumber++;
                        }

                        ComplianceSummaryReportHelper.EmployeeName(sheet, ref rowNumber, 10, employeeData.Text, GetOptionText(parameters, employeeData));

                        AchievementTable(data.GetAchievements(employeeData));
                    }
                }
            }

            foreach (var chartData in charts)
            {
                ProgressCallback(OperationAddingCharts, operationNumber, operationCount);
                operationNumber += ChartOperationWeight;
                ComplianceSummaryReportHelper.AddBarChart(sheet, chartData);
            }

            if (rowNumber > 1)
                sheet.PrinterSettings.PrintArea = sheet.Cells[1, 1, rowNumber - 1, 10];

            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;

            void AchievementTable(IEnumerable<ComplianceSummaryReportHelper.CurrentAchievementItem> achievements)
            {
                var number = 1;

                foreach (var item in achievements)
                {
                    string nameCellStyle, dataCellStyle;

                    if (number % 2 == 0)
                    {
                        nameCellStyle = "Even";
                        dataCellStyle = "Even Centered";
                    }
                    else
                    {
                        nameCellStyle = "Odd";
                        dataCellStyle = "Odd Centered";
                    }

                    var cell1 = sheet.Cells[rowNumber, 1];
                    cell1.Value = item.Info.Name;
                    cell1.StyleName = nameCellStyle;

                    if (item.Data != null)
                    {
                        var cell2 = sheet.Cells[rowNumber, 2];
                        cell2.Value = item.Data.Expired;
                        cell2.StyleName = dataCellStyle;
                        cell2.Style.Numberformat.Format = "#,##0";

                        var cell3 = sheet.Cells[rowNumber, 3];
                        cell3.Value = item.Data.NotCompleted;
                        cell3.StyleName = dataCellStyle;
                        cell3.Style.Numberformat.Format = "#,##0";

                        var cell4 = sheet.Cells[rowNumber, 4];
                        cell4.Value = item.Data.NotApplicable;
                        cell4.StyleName = dataCellStyle;
                        cell4.Style.Numberformat.Format = "#,##0";

                        var cell5 = sheet.Cells[rowNumber, 5];
                        cell5.Value = item.Data.NeedsTraining;
                        cell5.StyleName = dataCellStyle;
                        cell5.Style.Numberformat.Format = "#,##0";

                        var cell6 = sheet.Cells[rowNumber, 6];
                        cell6.Value = item.Data.SelfAssessed;
                        cell6.StyleName = dataCellStyle;
                        cell6.Style.Numberformat.Format = "#,##0";

                        var cell7 = sheet.Cells[rowNumber, 7];
                        cell7.Value = item.Data.Submitted;
                        cell7.StyleName = dataCellStyle;
                        cell7.Style.Numberformat.Format = "#,##0";

                        var cell8 = sheet.Cells[rowNumber, 8];
                        cell8.Value = item.Data.Satisfied;
                        cell8.StyleName = dataCellStyle;
                        cell8.Style.Numberformat.Format = "#,##0";

                        var cell9 = sheet.Cells[rowNumber, 9];
                        cell9.Value = item.Data.Required;
                        cell9.StyleName = dataCellStyle;
                        cell9.Style.Numberformat.Format = "#,##0";

                        var cell10 = sheet.Cells[rowNumber, 10];
                        cell10.StyleName = dataCellStyle;
                        cell10.Style.Numberformat.Format = "0.0%";
                        cell10.Value = item.Data.Score == null ? "NA" : (object)(item.Data.Score);

                        if (item.Data.Score.HasValue)
                            cell10.Style.Font.Color.SetColor(item.Data.Score >= 1 ? Color.Green : Color.Red);
                    }

                    rowNumber++;
                    number++;
                }
            }
        }

        #endregion

        #region History

        private bool GenerateXlsxHistory(SearchParameters parameters, ExcelPackage excel)
        {
            var data = GetHistoryReportDataSource(parameters, parameters.Departments);

            if (data.Count == 0)
                return false;

            int i = 0;
            if (parameters.IsShowStatus)
                GenerateXlsxHistoryWithStatus(parameters, excel, data, GetSheetName);
            else
                GenerateXlsxHistoryWithoutStatus(parameters, excel, data, GetSheetName);

            return true;

            // A worksheet name in Excel cannot exceed 31 characters and must begin with a letter. If we do not ensure all worksheet names are unique
            // then we get a run-time exception. This function truncates the name to 25 characters and adds a unique prefix with an auto-incrementing
            // sequence number.
            string GetSheetName(string letter, string name)
                => $"{letter}{++i}-" + StringHelper.TruncateString(name, 25);
        }

        private void GenerateXlsxHistoryWithStatus(SearchParameters parameters, ExcelPackage excel, List<ComplianceSummaryReportHelper.HistoryReportDataSource> data, Func<string, string, string> getSheetName)
        {
            foreach (var reportData in data)
            {
                var department = reportData.Columns[0].DepartmentName;
                var reportSheet = excel.Workbook.Worksheets.Add(getSheetName("R", department));
                var chartSheet = excel.Workbook.Worksheets.Add(getSheetName("C", department));

                GenerateXlsxHistoryWithStatus(parameters, reportSheet, chartSheet, reportData);
            }
        }

        private void GenerateXlsxHistoryWithStatus(SearchParameters parameters, ExcelWorksheet reportSheet, ExcelWorksheet chartSheet, ComplianceSummaryReportHelper.HistoryReportDataSource data)
        {
            reportSheet.Column(1).Width = 30;
            reportSheet.Column(2).Width = 10;
            reportSheet.Column(3).Width = 10;
            reportSheet.Column(4).Width = 10;
            reportSheet.Column(5).Width = 10;
            reportSheet.Column(6).Width = 10;
            reportSheet.Column(7).Width = 10;
            reportSheet.Column(8).Width = 10;
            reportSheet.Column(9).Width = 10;
            reportSheet.Column(10).Width = 10;
            reportSheet.Column(11).Width = 10;
            reportSheet.Column(12).Width = 10;
            reportSheet.Column(13).Width = 10;

            var reportRowNumber = 1;
            var chartRowNumber = 1;
            var isFirstReportRow1 = true;
            var operationNumber = 0;
            var operationCount = data.Columns.Count;

            if (parameters.IsShowChart)
                operationCount += data.Columns.Count * ChartOperationWeight;

            var charts = new List<ComplianceSummaryReportHelper.LineChartData>();

            ComplianceSummaryReportHelper.Timestamp(reportSheet, ref reportRowNumber, 13);

            foreach (var departmentData in data.Columns)
            {
                ProgressCallback(OperationWritingData, operationNumber++, operationCount);

                if (!isFirstReportRow1)
                    reportRowNumber = ComplianceSummaryReportHelper.Separator(reportSheet, reportRowNumber, 13, 30);
                else
                    isFirstReportRow1 = false;

                ComplianceSummaryReportHelper.CompanyName(reportSheet, ref reportRowNumber, 13, departmentData.CompanyName);
                ComplianceSummaryReportHelper.ReportTitle(reportSheet, ref reportRowNumber, 13, parameters.ReportTitle);
                ComplianceSummaryReportHelper.DepartmentName(reportSheet, ref reportRowNumber, 13, departmentData.DepartmentName);

                if (parameters.IsShowChart)
                    ComplianceSummaryReportHelper.AddLineChartData(reportSheet, ref reportRowNumber, chartSheet, ref chartRowNumber, departmentData, Organization.Code, 13, charts);

                // Table Summary Header

                {
                    var row = reportSheet.Row(reportRowNumber);
                    row.Height = 20;

                    var cell1 = reportSheet.Cells[reportRowNumber, 1];
                    cell1.Value = "Compliance Item";
                    cell1.StyleName = "White Bold BT";

                    var cell2 = reportSheet.Cells[reportRowNumber, 2];
                    cell2.Value = string.Empty;
                    cell2.StyleName = "White Bold BT";

                    var cell3 = reportSheet.Cells[reportRowNumber, 3];
                    cell3.Value = string.Empty;
                    cell3.StyleName = "White Bold BT";

                    var cell4 = reportSheet.Cells[reportRowNumber, 4];
                    cell4.Value = string.Empty;
                    cell4.StyleName = "White Bold BT";

                    var cell5 = reportSheet.Cells[reportRowNumber, 5];
                    cell5.Value = string.Empty;
                    cell5.StyleName = "White Bold BT";

                    var cell6 = reportSheet.Cells[reportRowNumber, 6];
                    cell6.Value = string.Empty;
                    cell6.StyleName = "White Bold BT";

                    var cell7 = reportSheet.Cells[reportRowNumber, 7];
                    cell7.Value = string.Empty;
                    cell7.StyleName = "White Bold BT";

                    var cell8 = reportSheet.Cells[reportRowNumber, 8];
                    cell8.Value = string.Empty;
                    cell8.StyleName = "White Bold BT";

                    var cell9 = reportSheet.Cells[reportRowNumber, 9];
                    cell9.Value = string.Empty;
                    cell9.StyleName = "White Bold BT";

                    var cell10 = reportSheet.Cells[reportRowNumber, 10];
                    cell10.Value = data.SnapshotDate3;
                    cell10.StyleName = "White Bold Centered BT";
                    cell10.Style.Numberformat.Format = "MMM d";

                    var cell11 = reportSheet.Cells[reportRowNumber, 11];
                    cell11.Value = data.SnapshotDate2;
                    cell11.StyleName = "White Bold Centered BT";
                    cell11.Style.Numberformat.Format = "MMM d";

                    var cell12 = reportSheet.Cells[reportRowNumber, 12];
                    cell12.Value = data.SnapshotDate1;
                    cell12.StyleName = "White Bold Centered BT";
                    cell12.Style.Numberformat.Format = "MMM d";

                    var cell13 = reportSheet.Cells[reportRowNumber, 13];
                    cell13.Value = "Current";
                    cell13.StyleName = "White Bold Centered BT";

                    reportRowNumber++;
                }

                // Table Summary Body

                {
                    var organization = Organization.Code;

                    var summaryData = departmentData.ChartData
                        .GroupBy(x => x.Sequence)
                        .OrderBy(x => x.Key)
                        .Select(group =>
                        {
                            string name = null;
                            decimal? score = null, score1 = null, score2 = null, score3 = null;

                            foreach (var dataItem in group)
                            {
                                if (name == null)
                                    name = _achievementDisplayMapping.GetOrDefault(dataItem.Heading, dataItem.Heading);

                                if (!dataItem.SnapshotDate.HasValue)
                                    score = dataItem.Score;
                                else if (dataItem.SnapshotDate.Value == data.SnapshotDate1)
                                    score1 = dataItem.Score;
                                else if (dataItem.SnapshotDate.Value == data.SnapshotDate2)
                                    score2 = dataItem.Score;
                                else if (dataItem.SnapshotDate.Value == data.SnapshotDate3)
                                    score3 = dataItem.Score;

                                if (score.HasValue && score1.HasValue && score2.HasValue && score3.HasValue)
                                    break;
                            }

                            return new
                            {
                                Name = name,
                                Score1 = score1,
                                Score2 = score2,
                                Score3 = score3,
                                Score = score
                            };
                        });

                    var number = 1;

                    foreach (var summaryItem in summaryData)
                    {
                        string nameCellStyle, dataCellStyle;

                        if (number % 2 == 0)
                        {
                            nameCellStyle = "Even";
                            dataCellStyle = "Even Centered";
                        }
                        else
                        {
                            nameCellStyle = "Odd";
                            dataCellStyle = "Odd Centered";
                        }

                        var cell1 = reportSheet.Cells[reportRowNumber, 1];
                        cell1.Value = summaryItem.Name;
                        cell1.StyleName = nameCellStyle;

                        var cell2 = reportSheet.Cells[reportRowNumber, 2];
                        cell2.Value = string.Empty;
                        cell2.StyleName = dataCellStyle;

                        var cell3 = reportSheet.Cells[reportRowNumber, 3];
                        cell3.Value = string.Empty;
                        cell3.StyleName = dataCellStyle;

                        var cell4 = reportSheet.Cells[reportRowNumber, 4];
                        cell4.Value = string.Empty;
                        cell4.StyleName = dataCellStyle;

                        var cell5 = reportSheet.Cells[reportRowNumber, 5];
                        cell5.Value = string.Empty;
                        cell5.StyleName = dataCellStyle;

                        var cell6 = reportSheet.Cells[reportRowNumber, 6];
                        cell6.Value = string.Empty;
                        cell6.StyleName = dataCellStyle;

                        var cell7 = reportSheet.Cells[reportRowNumber, 7];
                        cell7.Value = string.Empty;
                        cell7.StyleName = dataCellStyle;

                        var cell8 = reportSheet.Cells[reportRowNumber, 8];
                        cell8.Value = string.Empty;
                        cell8.StyleName = dataCellStyle;

                        var cell9 = reportSheet.Cells[reportRowNumber, 9];
                        cell9.Value = string.Empty;
                        cell9.StyleName = dataCellStyle;

                        var cell10 = reportSheet.Cells[reportRowNumber, 10];
                        cell10.Value = summaryItem.Score3;
                        cell10.StyleName = dataCellStyle;
                        cell10.Style.Numberformat.Format = "0.0%";
                        cell10.Style.Font.Color.SetColor(!summaryItem.Score3.HasValue ? Color.Empty : (summaryItem.Score3.Value >= 1 ? Color.Green : Color.Red));

                        var cell11 = reportSheet.Cells[reportRowNumber, 11];
                        cell11.Value = summaryItem.Score2;
                        cell11.StyleName = dataCellStyle;
                        cell11.Style.Numberformat.Format = "0.0%";
                        cell11.Style.Font.Color.SetColor(!summaryItem.Score2.HasValue ? Color.Empty : (summaryItem.Score2.Value >= 1 ? Color.Green : Color.Red));

                        var cell12 = reportSheet.Cells[reportRowNumber, 12];
                        cell12.Value = summaryItem.Score1;
                        cell12.StyleName = dataCellStyle;
                        cell12.Style.Numberformat.Format = "0.0%";
                        cell12.Style.Font.Color.SetColor(!summaryItem.Score1.HasValue ? Color.Empty : (summaryItem.Score1.Value >= 1 ? Color.Green : Color.Red));

                        var cell13 = reportSheet.Cells[reportRowNumber, 13];
                        cell13.Value = summaryItem.Score;
                        cell13.StyleName = dataCellStyle;
                        cell13.Style.Numberformat.Format = "0.0%";
                        cell13.Style.Font.Color.SetColor(!summaryItem.Score.HasValue ? Color.Empty : (summaryItem.Score.Value >= 1 ? Color.Green : Color.Red));

                        reportRowNumber++;
                        number++;
                    }
                }

                if (parameters.IsShowWorkers)
                {
                    reportRowNumber = ComplianceSummaryReportHelper.Separator(reportSheet, reportRowNumber, 13, 20);

                    // Workers Summary Header

                    {
                        var row = reportSheet.Row(reportRowNumber);
                        row.Height = 20;

                        var cell1_9 = reportSheet.Cells[reportRowNumber, 1, reportRowNumber, 9];
                        cell1_9.Value = $"{parameters.ReportTitle} for {departmentData.DepartmentName}";
                        cell1_9.Merge = true;
                        cell1_9.StyleName = "Blue Bold";

                        var cell10_13 = reportSheet.Cells[reportRowNumber, 10, reportRowNumber, 13];
                        cell10_13.Value = "Score";
                        cell10_13.Merge = true;
                        cell10_13.StyleName = "Blue Bold Centered";
                        cell10_13.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        reportRowNumber++;
                    }

                    var isFirstRow2 = true;

                    foreach (var employeeData in departmentData.Children)
                    {
                        if (!isFirstRow2)
                            reportRowNumber = ComplianceSummaryReportHelper.Separator(reportSheet, reportRowNumber, 13, 20);
                        else
                            isFirstRow2 = !isFirstRow2;

                        // Header

                        {
                            var row = reportSheet.Row(reportRowNumber);
                            row.Height = 33;

                            var cell1 = reportSheet.Cells[reportRowNumber, 1];
                            cell1.Value = "Compliance Item";
                            cell1.StyleName = "Blue Bold Small";
                            cell1.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell2 = reportSheet.Cells[reportRowNumber, 2];
                            cell2.Value = "Expired";
                            cell2.StyleName = "Blue Bold Small Centered";
                            cell2.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell3 = reportSheet.Cells[reportRowNumber, 3];
                            cell3.Value = "Not Completed";
                            cell3.StyleName = "Blue Bold Small Centered";
                            cell3.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell4 = reportSheet.Cells[reportRowNumber, 4];
                            cell4.Value = "Not Applicable";
                            cell4.StyleName = "Blue Bold Small Centered";
                            cell4.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell5 = reportSheet.Cells[reportRowNumber, 5];
                            cell5.Value = "Needs Training";
                            cell5.StyleName = "Blue Bold Small Centered";
                            cell5.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell6 = reportSheet.Cells[reportRowNumber, 6];
                            cell6.Value = "Self Assessed";
                            cell6.StyleName = "Blue Bold Small Centered";
                            cell6.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell7 = reportSheet.Cells[reportRowNumber, 7];
                            cell7.Value = "Submitted for Validation";
                            cell7.StyleName = "Blue Bold Small Centered";
                            cell7.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell8 = reportSheet.Cells[reportRowNumber, 8];
                            cell8.Value = "Validated";
                            cell8.StyleName = "Blue Bold Small Centered";
                            cell8.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell9 = reportSheet.Cells[reportRowNumber, 9];
                            cell9.Value = "Required";
                            cell9.StyleName = "Blue Bold Small Centered";
                            cell9.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell10 = reportSheet.Cells[reportRowNumber, 10];
                            cell10.Value = data.SnapshotDate3;
                            cell10.StyleName = "Blue Bold Small Centered";
                            cell10.Style.Numberformat.Format = "MMM d";
                            cell10.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell11 = reportSheet.Cells[reportRowNumber, 11];
                            cell11.Value = data.SnapshotDate2;
                            cell11.StyleName = "Blue Bold Small Centered";
                            cell11.Style.Numberformat.Format = "MMM d";
                            cell11.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell12 = reportSheet.Cells[reportRowNumber, 12];
                            cell12.Value = data.SnapshotDate1;
                            cell12.StyleName = "Blue Bold Small Centered";
                            cell12.Style.Numberformat.Format = "MMM d";
                            cell12.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            var cell13 = reportSheet.Cells[reportRowNumber, 13];
                            cell13.Value = "Current";
                            cell13.StyleName = "Blue Bold Small Centered";
                            cell13.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            reportRowNumber++;
                        }

                        // Employee Name

                        {
                            var row = reportSheet.Row(reportRowNumber);
                            row.Height = 20;

                            var cell = reportSheet.Cells[reportRowNumber, 1, reportRowNumber, 13];
                            cell.Value = $"{employeeData.Text} ({GetOptionText(parameters, employeeData)})";
                            cell.StyleName = "White Bold";
                            cell.Merge = true;

                            reportRowNumber++;
                        }

                        // Body

                        {
                            var number = 1;

                            foreach (var achievement in data.GetAchievements(employeeData))
                            {
                                string nameCellStyle, dataCellStyle;

                                if (number % 2 == 0)
                                {
                                    nameCellStyle = "Even";
                                    dataCellStyle = "Even Centered";
                                }
                                else
                                {
                                    nameCellStyle = "Odd";
                                    dataCellStyle = "Odd Centered";
                                }

                                var cell1 = reportSheet.Cells[reportRowNumber, 1];
                                cell1.Value = achievement.Info.Name;
                                cell1.StyleName = nameCellStyle;

                                var cell2 = reportSheet.Cells[reportRowNumber, 2];
                                cell2.Value = AdjustAmount(achievement.Data, achievement.Data.Expired);
                                cell2.StyleName = dataCellStyle;
                                cell2.Style.Numberformat.Format = "#,##0";

                                var cell3 = reportSheet.Cells[reportRowNumber, 3];
                                cell3.Value = AdjustAmount(achievement.Data, achievement.Data.NotCompleted);
                                cell3.StyleName = dataCellStyle;
                                cell3.Style.Numberformat.Format = "#,##0";

                                var cell4 = reportSheet.Cells[reportRowNumber, 4];
                                cell4.Value = AdjustAmount(achievement.Data, achievement.Data.NotApplicable);
                                cell4.StyleName = dataCellStyle;
                                cell4.Style.Numberformat.Format = "#,##0";

                                var cell5 = reportSheet.Cells[reportRowNumber, 5];
                                cell5.Value = AdjustAmount(achievement.Data, achievement.Data.NeedsTraining);
                                cell5.StyleName = dataCellStyle;
                                cell5.Style.Numberformat.Format = "#,##0";

                                var cell6 = reportSheet.Cells[reportRowNumber, 6];
                                cell6.Value = AdjustAmount(achievement.Data, achievement.Data.SelfAssessed);
                                cell6.StyleName = dataCellStyle;
                                cell6.Style.Numberformat.Format = "#,##0";

                                var cell7 = reportSheet.Cells[reportRowNumber, 7];
                                cell7.Value = AdjustAmount(achievement.Data, achievement.Data.Submitted);
                                cell7.StyleName = dataCellStyle;
                                cell7.Style.Numberformat.Format = "#,##0";

                                var cell8 = reportSheet.Cells[reportRowNumber, 8];
                                cell8.Value = AdjustAmount(achievement.Data, achievement.Data.Satisfied);
                                cell8.StyleName = dataCellStyle;
                                cell8.Style.Numberformat.Format = "#,##0";

                                var cell9 = reportSheet.Cells[reportRowNumber, 9];
                                cell9.Value = AdjustAmount(achievement.Data, achievement.Data.Required);
                                cell9.StyleName = dataCellStyle;
                                cell9.Style.Numberformat.Format = "#,##0";

                                var cell10 = reportSheet.Cells[reportRowNumber, 10];
                                cell10.Value = AdjustScore(achievement.Data, achievement.Data.Score3);
                                cell10.StyleName = dataCellStyle;
                                cell10.Style.Numberformat.Format = "0.0%";
                                cell10.Style.Font.Color.SetColor(!achievement.Data.Score3.HasValue ? Color.Empty : (achievement.Data.Score3.Value >= 1 ? Color.Green : Color.Red));

                                var cell11 = reportSheet.Cells[reportRowNumber, 11];
                                cell11.Value = AdjustScore(achievement.Data, achievement.Data.Score2);
                                cell11.StyleName = dataCellStyle;
                                cell11.Style.Numberformat.Format = "0.0%";
                                cell11.Style.Font.Color.SetColor(!achievement.Data.Score2.HasValue ? Color.Empty : (achievement.Data.Score2.Value >= 1 ? Color.Green : Color.Red));

                                var cell12 = reportSheet.Cells[reportRowNumber, 12];
                                cell12.Value = AdjustScore(achievement.Data, achievement.Data.Score1);
                                cell12.StyleName = dataCellStyle;
                                cell12.Style.Numberformat.Format = "0.0%";
                                cell12.Style.Font.Color.SetColor(!achievement.Data.Score1.HasValue ? Color.Empty : (achievement.Data.Score1.Value >= 1 ? Color.Green : Color.Red));

                                var cell13 = reportSheet.Cells[reportRowNumber, 13];
                                cell13.Value = AdjustScore(achievement.Data, achievement.Data.Score);
                                cell13.StyleName = dataCellStyle;
                                cell13.Style.Numberformat.Format = "0.0%";
                                cell13.Style.Font.Color.SetColor(achievement.Data.Score >= 1 ? Color.Green : Color.Red);

                                reportRowNumber++;
                                number++;
                            }
                        }
                    }
                }
            }

            foreach (var chartData in charts)
            {
                ProgressCallback(OperationAddingCharts, operationNumber, operationCount);
                operationNumber += ChartOperationWeight;
                ComplianceSummaryReportHelper.AddLineChart(reportSheet, chartSheet, chartData);
            }

            if (reportRowNumber > 1)
                reportSheet.PrinterSettings.PrintArea = reportSheet.Cells[1, 1, reportRowNumber - 1, 13];

            chartSheet.Hidden = eWorkSheetHidden.Hidden;
        }

        private void GenerateXlsxHistoryWithoutStatus(SearchParameters parameters, ExcelPackage excel, List<ComplianceSummaryReportHelper.HistoryReportDataSource> data, Func<string, string, string> getSheetName)
        {
            foreach (var reportData in data)
            {
                var department = reportData.Columns[0].DepartmentName;
                var reportSheet = excel.Workbook.Worksheets.Add(getSheetName("R", department));
                var chartSheet = excel.Workbook.Worksheets.Add(getSheetName("C", department));

                GenerateXlsxHistoryWithoutStatus(parameters, reportSheet, chartSheet, reportData);
            }
        }

        private ExcelWorksheet GenerateXlsxHistoryWithoutStatus(SearchParameters parameters, ExcelWorksheet reportSheet, ExcelWorksheet chartSheet, ComplianceSummaryReportHelper.HistoryReportDataSource data)
        {
            reportSheet.Column(1).Width = 20;
            reportSheet.Column(2).Width = 30;
            reportSheet.Column(3).Width = 12;
            reportSheet.Column(4).Width = 12;
            reportSheet.Column(5).Width = 12;
            reportSheet.Column(6).Width = 12;
            reportSheet.Column(7).Width = 12;
            reportSheet.Column(8).Width = 12;
            reportSheet.Column(9).Width = 12;

            var reportRowNumber = 1;
            var chartRowNumber = 1;
            var isFirstReportRow1 = true;
            var operationNumber = 0;
            var operationCount = data.Columns.Count;

            if (parameters.IsShowChart)
                operationCount += data.Columns.Count * ChartOperationWeight;

            var charts = new List<ComplianceSummaryReportHelper.LineChartData>();

            ComplianceSummaryReportHelper.Timestamp(reportSheet, ref reportRowNumber, 9);

            foreach (var departmentData in data.Columns)
            {
                ProgressCallback(OperationWritingData, operationNumber++, operationCount);

                if (!isFirstReportRow1)
                    reportRowNumber = ComplianceSummaryReportHelper.Separator(reportSheet, reportRowNumber, 9, 30);
                else
                    isFirstReportRow1 = false;

                ComplianceSummaryReportHelper.CompanyName(reportSheet, ref reportRowNumber, 9, departmentData.CompanyName);
                ComplianceSummaryReportHelper.ReportTitle(reportSheet, ref reportRowNumber, 9, parameters.ReportTitle);
                ComplianceSummaryReportHelper.DepartmentName(reportSheet, ref reportRowNumber, 9, departmentData.DepartmentName);

                if (parameters.IsShowChart)
                    ComplianceSummaryReportHelper.AddLineChartData(reportSheet, ref reportRowNumber, chartSheet, ref chartRowNumber, departmentData, Organization.Code, 9, charts);

                // Table Summary Header

                {
                    var row = reportSheet.Row(reportRowNumber);
                    row.Height = 20;

                    var cell1 = reportSheet.Cells[reportRowNumber, 1];
                    cell1.Value = string.Empty;
                    cell1.StyleName = "White Bold BT";

                    var cell2 = reportSheet.Cells[reportRowNumber, 2];
                    cell2.Value = "Compliance Item";
                    cell2.StyleName = "White Bold BT";

                    var cell3 = reportSheet.Cells[reportRowNumber, 3];
                    cell3.Value = string.Empty;
                    cell3.StyleName = "White Bold BT";

                    var cell4 = reportSheet.Cells[reportRowNumber, 4];
                    cell4.Value = string.Empty;
                    cell4.StyleName = "White Bold BT";

                    var cell5 = reportSheet.Cells[reportRowNumber, 5];
                    cell5.Value = string.Empty;
                    cell5.StyleName = "White Bold BT";

                    var cell6 = reportSheet.Cells[reportRowNumber, 6];
                    cell6.Value = data.SnapshotDate3;
                    cell6.StyleName = "White Bold Centered BT";
                    cell6.Style.Numberformat.Format = "MMM d";

                    var cell7 = reportSheet.Cells[reportRowNumber, 7];
                    cell7.Value = data.SnapshotDate2;
                    cell7.StyleName = "White Bold Centered BT";
                    cell7.Style.Numberformat.Format = "MMM d";

                    var cell8 = reportSheet.Cells[reportRowNumber, 8];
                    cell8.Value = data.SnapshotDate1;
                    cell8.StyleName = "White Bold Centered BT";
                    cell8.Style.Numberformat.Format = "MMM d";

                    var cell9 = reportSheet.Cells[reportRowNumber, 9];
                    cell9.Value = "Current";
                    cell9.StyleName = "White Bold Centered BT";

                    reportRowNumber++;
                }

                // Table Summary Body

                {
                    var organization = Organization.Code;

                    var summaryData = departmentData.ChartData
                        .GroupBy(x => x.Sequence)
                        .OrderBy(x => x.Key)
                        .Select(group =>
                        {
                            string name = null;
                            decimal? score = null, score1 = null, score2 = null, score3 = null;

                            foreach (var dataItem in group)
                            {
                                if (name == null)
                                    name = _achievementDisplayMapping.GetOrDefault(dataItem.Heading, dataItem.Heading);

                                if (!dataItem.SnapshotDate.HasValue)
                                    score = dataItem.Score;
                                else if (dataItem.SnapshotDate.Value == data.SnapshotDate1)
                                    score1 = dataItem.Score;
                                else if (dataItem.SnapshotDate.Value == data.SnapshotDate2)
                                    score2 = dataItem.Score;
                                else if (dataItem.SnapshotDate.Value == data.SnapshotDate3)
                                    score3 = dataItem.Score;

                                if (score.HasValue && score1.HasValue && score2.HasValue && score3.HasValue)
                                    break;
                            }

                            return new
                            {
                                Name = name,
                                Score1 = score1,
                                Score2 = score2,
                                Score3 = score3,
                                Score = score
                            };
                        });

                    var number = 1;

                    foreach (var summaryItem in summaryData)
                    {
                        string nameCellStyle, dataCellStyle;

                        if (number % 2 == 0)
                        {
                            nameCellStyle = "Even";
                            dataCellStyle = "Even Centered";
                        }
                        else
                        {
                            nameCellStyle = "Odd";
                            dataCellStyle = "Odd Centered";
                        }

                        var cell1 = reportSheet.Cells[reportRowNumber, 1];
                        cell1.Value = string.Empty;
                        cell1.StyleName = nameCellStyle;

                        var cell2 = reportSheet.Cells[reportRowNumber, 2];
                        cell2.Value = summaryItem.Name;
                        cell2.StyleName = nameCellStyle;

                        var cell3 = reportSheet.Cells[reportRowNumber, 3];
                        cell3.Value = string.Empty;
                        cell3.StyleName = dataCellStyle;

                        var cell4 = reportSheet.Cells[reportRowNumber, 4];
                        cell4.Value = string.Empty;
                        cell4.StyleName = dataCellStyle;

                        var cell5 = reportSheet.Cells[reportRowNumber, 5];
                        cell5.Value = string.Empty;
                        cell5.StyleName = dataCellStyle;

                        var cell6 = reportSheet.Cells[reportRowNumber, 6];
                        cell6.Value = summaryItem.Score3;
                        cell6.StyleName = dataCellStyle;
                        cell6.Style.Numberformat.Format = "0.0%";
                        cell6.Style.Font.Color.SetColor(!summaryItem.Score3.HasValue ? Color.Empty : (summaryItem.Score3.Value >= 1 ? Color.Green : Color.Red));

                        var cell7 = reportSheet.Cells[reportRowNumber, 7];
                        cell7.Value = summaryItem.Score2;
                        cell7.StyleName = dataCellStyle;
                        cell7.Style.Numberformat.Format = "0.0%";
                        cell7.Style.Font.Color.SetColor(!summaryItem.Score2.HasValue ? Color.Empty : (summaryItem.Score2.Value >= 1 ? Color.Green : Color.Red));

                        var cell8 = reportSheet.Cells[reportRowNumber, 8];
                        cell8.Value = summaryItem.Score1;
                        cell8.StyleName = dataCellStyle;
                        cell8.Style.Numberformat.Format = "0.0%";
                        cell8.Style.Font.Color.SetColor(!summaryItem.Score1.HasValue ? Color.Empty : (summaryItem.Score1.Value >= 1 ? Color.Green : Color.Red));

                        var cell9 = reportSheet.Cells[reportRowNumber, 9];
                        cell9.Value = summaryItem.Score;
                        cell9.StyleName = dataCellStyle;
                        cell9.Style.Numberformat.Format = "0.0%";
                        cell9.Style.Font.Color.SetColor(!summaryItem.Score.HasValue ? Color.Empty : (summaryItem.Score.Value >= 1 ? Color.Green : Color.Red));

                        reportRowNumber++;
                        number++;
                    }
                }

                if (parameters.IsShowWorkers)
                {
                    reportRowNumber = ComplianceSummaryReportHelper.Separator(reportSheet, reportRowNumber, 9, 20);

                    // Workers Summary Header

                    {
                        var row = reportSheet.Row(reportRowNumber);
                        row.Height = 20;

                        var cell1_5 = reportSheet.Cells[reportRowNumber, 1, reportRowNumber, 5];
                        cell1_5.Value = $"{parameters.ReportTitle} for {departmentData.DepartmentName}";
                        cell1_5.Merge = true;
                        cell1_5.StyleName = "Blue Bold";

                        var cell6_9 = reportSheet.Cells[reportRowNumber, 6, reportRowNumber, 9];
                        cell6_9.Value = "Score";
                        cell6_9.Merge = true;
                        cell6_9.StyleName = "Blue Bold Centered";
                        cell6_9.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        reportRowNumber++;
                    }

                    var isFirstRow2 = true;

                    foreach (var employeeData in departmentData.Children)
                    {
                        if (!isFirstRow2)
                            reportRowNumber = ComplianceSummaryReportHelper.Separator(reportSheet, reportRowNumber, 9, 20);
                        else
                            isFirstRow2 = !isFirstRow2;

                        // Header

                        {
                            var row = reportSheet.Row(reportRowNumber);
                            row.Height = 20;

                            var cell1 = reportSheet.Cells[reportRowNumber, 1];
                            cell1.Value = "Worker";
                            cell1.StyleName = "Blue Bold";

                            var cell2 = reportSheet.Cells[reportRowNumber, 2];
                            cell2.Value = "Compliance Item";
                            cell2.StyleName = "Blue Bold";

                            var cell3 = reportSheet.Cells[reportRowNumber, 3];
                            cell3.Value = "Submitted";
                            cell3.StyleName = "Blue Bold Centered";
                            cell3.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var cell4 = reportSheet.Cells[reportRowNumber, 4];
                            cell4.Value = "Validated";
                            cell4.StyleName = "Blue Bold Centered";
                            cell4.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var cell5 = reportSheet.Cells[reportRowNumber, 5];
                            cell5.Value = "Required";
                            cell5.StyleName = "Blue Bold Centered";
                            cell5.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var cell6 = reportSheet.Cells[reportRowNumber, 6];
                            cell6.Value = data.SnapshotDate3;
                            cell6.StyleName = "Blue Bold Centered";
                            cell6.Style.Numberformat.Format = "MMM d";
                            cell6.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var cell7 = reportSheet.Cells[reportRowNumber, 7];
                            cell7.Value = data.SnapshotDate2;
                            cell7.StyleName = "Blue Bold Centered";
                            cell7.Style.Numberformat.Format = "MMM d";
                            cell7.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var cell8 = reportSheet.Cells[reportRowNumber, 8];
                            cell8.Value = data.SnapshotDate1;
                            cell8.StyleName = "Blue Bold Centered";
                            cell8.Style.Numberformat.Format = "MMM d";
                            cell8.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var cell9 = reportSheet.Cells[reportRowNumber, 9];
                            cell9.Value = "Current";
                            cell9.StyleName = "Blue Bold Centered";
                            cell9.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            reportRowNumber++;
                        }

                        // Employee Name

                        {
                            var row = reportSheet.Row(reportRowNumber);
                            row.Height = 20;

                            var cell = reportSheet.Cells[reportRowNumber, 1, reportRowNumber, 9];
                            cell.Value = $"{employeeData.Text} ({GetOptionText(parameters, employeeData)})";
                            cell.StyleName = "White Bold";
                            cell.Merge = true;

                            reportRowNumber++;
                        }

                        // Body

                        {
                            var number = 1;

                            foreach (var achievement in data.GetAchievements(employeeData))
                            {
                                string nameCellStyle, dataCellStyle;

                                if (number % 2 == 0)
                                {
                                    nameCellStyle = "Even";
                                    dataCellStyle = "Even Centered";
                                }
                                else
                                {
                                    nameCellStyle = "Odd";
                                    dataCellStyle = "Odd Centered";
                                }

                                var cell1 = reportSheet.Cells[reportRowNumber, 1];
                                cell1.Value = string.Empty;
                                cell1.StyleName = nameCellStyle;

                                var cell2 = reportSheet.Cells[reportRowNumber, 2];
                                cell2.Value = achievement.Info.Name;
                                cell2.StyleName = nameCellStyle;

                                var cell3 = reportSheet.Cells[reportRowNumber, 3];
                                cell3.Value = AdjustAmount(achievement.Data, achievement.Data.Submitted);
                                cell3.StyleName = dataCellStyle;
                                cell3.Style.Numberformat.Format = "#,##0";

                                var cell4 = reportSheet.Cells[reportRowNumber, 4];
                                cell4.Value = AdjustAmount(achievement.Data, achievement.Data.Satisfied);
                                cell4.StyleName = dataCellStyle;
                                cell4.Style.Numberformat.Format = "#,##0";

                                var cell5 = reportSheet.Cells[reportRowNumber, 5];
                                cell5.Value = AdjustAmount(achievement.Data, achievement.Data.Required);
                                cell5.StyleName = dataCellStyle;
                                cell5.Style.Numberformat.Format = "#,##0";

                                var cell6 = reportSheet.Cells[reportRowNumber, 6];
                                cell6.Value = AdjustScore(achievement.Data, achievement.Data.Score3);
                                cell6.StyleName = dataCellStyle;
                                cell6.Style.Numberformat.Format = "0.0%";
                                cell6.Style.Font.Color.SetColor(!achievement.Data.Score3.HasValue ? Color.Empty : (achievement.Data.Score3.Value >= 1 ? Color.Green : Color.Red));

                                var cell7 = reportSheet.Cells[reportRowNumber, 7];
                                cell7.Value = AdjustScore(achievement.Data, achievement.Data.Score2);
                                cell7.StyleName = dataCellStyle;
                                cell7.Style.Numberformat.Format = "0.0%";
                                cell7.Style.Font.Color.SetColor(!achievement.Data.Score2.HasValue ? Color.Empty : (achievement.Data.Score2.Value >= 1 ? Color.Green : Color.Red));

                                var cell8 = reportSheet.Cells[reportRowNumber, 8];
                                cell8.Value = AdjustScore(achievement.Data, achievement.Data.Score1);
                                cell8.StyleName = dataCellStyle;
                                cell8.Style.Numberformat.Format = "0.0%";
                                cell8.Style.Font.Color.SetColor(!achievement.Data.Score1.HasValue ? Color.Empty : (achievement.Data.Score1.Value >= 1 ? Color.Green : Color.Red));

                                var cell9 = reportSheet.Cells[reportRowNumber, 9];
                                cell9.Value = AdjustScore(achievement.Data, achievement.Data.Score);
                                cell9.StyleName = dataCellStyle;
                                cell9.Style.Numberformat.Format = "0.0%";
                                cell9.Style.Font.Color.SetColor(achievement.Data.Score >= 1 ? Color.Green : Color.Red);

                                reportRowNumber++;
                                number++;
                            }
                        }
                    }
                }
            }

            foreach (var chartData in charts)
            {
                ProgressCallback(OperationAddingCharts, operationNumber, operationCount);
                operationNumber += ChartOperationWeight;
                ComplianceSummaryReportHelper.AddLineChart(reportSheet, chartSheet, chartData);
            }

            if (reportRowNumber > 1)
                reportSheet.PrinterSettings.PrintArea = reportSheet.Cells[1, 1, reportRowNumber - 1, 9];

            chartSheet.Hidden = eWorkSheetHidden.Hidden;

            return reportSheet;
        }

        #endregion

        #endregion

        #region Helper methods

        public static string GetReportTitle(bool isShowStatus = false)
        {
            var key = "Compliance Summary Report";

            var language = Shift.Common.Language.Default;

            var instructionsUrl = Organization.PlatformCustomization.InlineInstructionsUrl;

            var labelsUrl = Organization.PlatformCustomization.InlineLabelsUrl;

            var help = new InlineHelp(instructionsUrl, labelsUrl);

            var label = help.GetLabel(key, language);

            return label + (isShowStatus ? " (with Statuses)" : string.Empty);
        }

        private void EnsureAchievementDisplayMapping()
        {
            if (_achievementDisplayMapping == null)
                _achievementDisplayMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector.CreateAchievementLabelMapping(Organization.Code);
        }

        private bool ProgressCallback(string status, int currentPosition, int positionMax, bool isComplete = false)
        {
            DownloadProgress.UpdateContext(context =>
            {
                var progressBar = (InSite.Common.Web.UI.ProgressIndicator.ContextData)context.Items["Progress"];
                progressBar.Total = positionMax;
                progressBar.Value = currentPosition;

                context.Variables["status"] = status;

                context.IsComplete = isComplete;
            });

            return Page.Response.IsClientConnected;
        }

        private string GetOptionText(SearchParameters parameters, ComplianceSummaryReportHelper.EmployeeInfo employee)
        {
            if (parameters.Option == 1)
                return employee.PrimaryProfileName;

            if (parameters.Option == 2)
                return "Profiles that require compliance";

            return "All Profiles";
        }

        protected string GetOptionText(SearchParameters parameters, object employee)
        {
            return GetOptionText(parameters, (ComplianceSummaryReportHelper.EmployeeInfo)employee);
        }

        protected static int? AdjustAmount(ComplianceSummaryReportHelper.HistoryAchievementData data, int? value)
        {
            return data.Satisfied == 0 && data.Required == 0
                ? (int?)null
                : value;
        }

        protected static decimal? AdjustScore(ComplianceSummaryReportHelper.HistoryAchievementData data, decimal? value)
        {
            return data.Satisfied == 0 && data.Required == 0
                ? (decimal?)null
                : value;
        }

        protected static string AdjustAmountAndFormat(object dataObj, object valueObj)
        {
            var data = (ComplianceSummaryReportHelper.HistoryAchievementData)dataObj;

            return data.Satisfied == 0 && data.Required == 0
                ? string.Empty
                : string.Format("{0:n0}", valueObj);
        }

        protected static string AdjustScoreAndFormat(object dataObj, object valueObj)
        {
            var data = (ComplianceSummaryReportHelper.HistoryAchievementData)dataObj;
            var value = (decimal?)valueObj;

            return data.Satisfied == 0 && data.Required == 0
                ? string.Empty
                : FormatScore(value, string.Empty);
        }

        protected static string FormatScore(decimal? score, string nullValue = "NA")
        {
            return !score.HasValue ? nullValue : $"{Math.Truncate(100 * score.Value) / 100:p0}";
        }

        protected static Color GetScoreColor(decimal? score)
        {
            return !score.HasValue ? Color.Empty : score.Value >= 1 ? Color.SeaGreen : Color.Firebrick;
        }

        protected static string GetScoreHtmlColor(decimal? score)
        {
            return !score.HasValue ? "inherit" : score.Value >= 1 ? "#008000" : "#ff0000";
        }

        #endregion
    }
}