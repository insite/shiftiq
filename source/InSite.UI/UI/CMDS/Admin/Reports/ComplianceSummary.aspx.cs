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

using AlertType = Shift.Constant.AlertType;
using Color = System.Drawing.Color;

namespace InSite.Cmds.Admin.Reports.Forms
{
    public partial class ComplianceSummaryReport : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const int ChartOperationWeight = 12;
        private const int CompanyChartWidth = 10;
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
            public string ReportTitle => IsCompanySummary
                ? GetReportTitle() + " (Company Summary)"
                : GetReportTitle(IsShowStatus);
            public string FileName => IsCompanySummary
                ? "company-compliance-summary"
                : "compliance-summary" + (IsShowStatus ? "-with-statuses" : string.Empty);
            public Guid OrganizationIdentifier { get; set; }
            public Guid[] Departments { get; set; }
            public Guid[] Employees { get; set; }
            public MembershipEnum MembershipType { get; set; }
            public int[] Sequences { get; set; }
            public int Option { get; set; }
            public bool ExcludeUsersWithoutProfile { get; set; }
            public bool IsShowWorkers { get; set; }
            public bool IsShowStatus { get; set; }
            public bool IsShowChart { get; set; }
            public bool IsIncludeUsersWithoutProfile { get; set; }
            public bool IsCompanySummary { get; set; }
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

            ReportButton.Click += ReportButton_Click;

            DownloadPdf.Click += DownloadPdf_Click;
            PreDownloadXlsx1.Click += PreDownloadXlsx1_Click;
            PreDownloadXlsx2.Click += PreDownloadXlsx2_Click;
            PreDownloadCompanyXlsx.Click += PreDownloadCompanyXlsx_Click;
            DownloadXlsx.Click += DownloadXlsx_Click;

            DepartmentRepeater.ItemCommand += DepartmentRepeater_ItemCommand;

            CurrentDepartmentRepeaterWithoutStatuses.ItemCreated += CurrentDepartmentRepeater_ItemCreated;
            CurrentDepartmentRepeaterWithoutStatuses.ItemDataBound += CurrentDepartmentRepeater_ItemDataBound;

            CurrentDepartmentRepeaterWithStatuses.ItemCreated += CurrentDepartmentRepeater_ItemCreated;
            CurrentDepartmentRepeaterWithStatuses.ItemDataBound += CurrentDepartmentRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var title = GetReportTitle(false);

            PageHelper.AutoBindHeader(this, null, null, title);

            PageHelper.OverrideTitle(this, title);

            EnsureAchievementDisplayMapping();
            LoadAchievementTypes();
            UpdateEmployeeIdentifier();
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

        private void PreDownloadCompanyXlsx_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            DownloadParameters = CreateCompanyParameters();

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

                ReportXlsxHelper.ExportToXlsx(DownloadParameters.ReportTitle, excel.GetAsByteArray(), DownloadParameters.FileName);
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

        #endregion

        #region Data binding

        private SearchParameters CreateParameters(bool resolveEmployees = true)
        {
            var result = new SearchParameters
            {
                OrganizationIdentifier = Organization.Identifier,
                Departments = DepartmentIdentifier.Values,
                Employees = EmployeeIdentifier.Values,
                MembershipType = MembershipEnum.None,
                Sequences = AchievementType.ValuesAsInt.ToArray(),
                Option = int.Parse(Option.SelectedValue),
                ExcludeUsersWithoutProfile = !IsIncludeUsersWithoutProfile.Checked,
                IsShowWorkers = int.Parse(DisplayMode.SelectedValue) == 0,
                IsShowStatus = IsShowStatus.Checked,
                IsShowChart = IsShowChart.Checked
            };

            if (resolveEmployees && result.Employees.Length == 0)
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

        // Every department the signed-in account can reach, whatever the department selector holds. The people
        // are resolved the same way as for the regular report, so a department's figures agree between the two.
        private SearchParameters CreateCompanyParameters()
        {
            var result = CreateParameters(false);

            result.IsCompanySummary = true;
            result.Departments = DepartmentIdentifier.GetAccessibleDepartmentIdentifiers();
            result.Employees = EmployeeIdentifier.GetDataItems(result.Departments).Select(x => x.Value).ToArray();
            result.IsShowWorkers = false;
            result.IsShowStatus = false;

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

            LoadCurrentReport(parameters);

            CurrentDepartmentRepeaterWithoutStatuses.DataBind();
            CurrentDepartmentRepeaterWithStatuses.DataBind();

            DepartmentNoDataAlert.Visible = CurrentDepartmentRepeaterWithoutStatuses.Items.Count == 0
                && CurrentDepartmentRepeaterWithStatuses.Items.Count == 0;
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

        private List<ComplianceSummaryReportHelper.CurrentReportDataSource> GetCurrentReportDataSource(SearchParameters parameters, Guid[] departments)
        {
            var rows = CmdsReportHelper.SelectComplianceSummary(
                parameters.OrganizationIdentifier,
                departments,
                parameters.Employees,
                parameters.Sequences,
                parameters.Option,
                parameters.ExcludeUsersWithoutProfile,
                parameters.MembershipType.HasFlag(MembershipEnum.Department),
                parameters.MembershipType.HasFlag(MembershipEnum.Organization),
                parameters.MembershipType.HasFlag(MembershipEnum.Administration)
            );

            var organization = Organization.Code;

            var result = new List<ComplianceSummaryReportHelper.CurrentReportDataSource>();
            var map = new Dictionary<Guid, ComplianceSummaryReportHelper.CurrentReportDataSource>();

            foreach (var row in rows)
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

            result.Sort((x1, x2) => x1.Columns[0].DepartmentName.CompareTo(x2.Columns[0].DepartmentName));

            return result;
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

            Response.SendFile(parameters.FileName, "pdf", data);
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

            var hasData = parameters.IsCompanySummary
                ? GenerateXlsxCompany(parameters, excel)
                : GenerateXlsxCurrent(parameters, excel);

            if (!hasData)
                return false;

            excel.Workbook.Properties.Title = parameters.ReportTitle;
            excel.Workbook.Properties.Company = Organization.Name;
            excel.Workbook.Properties.Author = User.FullName;
            excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

            foreach (var sheet in excel.Workbook.Worksheets)
            {
                sheet.PrinterSettings.Orientation = parameters.IsCompanySummary ? eOrientation.Landscape : eOrientation.Portrait;
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
            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var departmentData in data)
            {
                var sheetName = ComplianceSummaryReportHelper.CreateSheetName(departmentData.Columns[0].DepartmentName, used);
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
            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var departmentData in data)
            {
                var sheetName = ComplianceSummaryReportHelper.CreateSheetName(departmentData.Columns[0].DepartmentName, used);
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

        #region Company

        private bool GenerateXlsxCompany(SearchParameters parameters, ExcelPackage excel)
        {
            var rows = CmdsReportHelper.SelectComplianceSummary(
                parameters.OrganizationIdentifier,
                parameters.Departments,
                parameters.Employees,
                parameters.Sequences,
                parameters.Option,
                parameters.ExcludeUsersWithoutProfile,
                parameters.MembershipType.HasFlag(MembershipEnum.Department),
                parameters.MembershipType.HasFlag(MembershipEnum.Organization),
                parameters.MembershipType.HasFlag(MembershipEnum.Administration)
            );

            var summary = ComplianceSummaryReportHelper.CompanySummary.Create(rows, _achievementDisplayMapping);

            if (summary.Departments.Count == 0 || summary.AchievementTypes.Count == 0)
                return false;

            var sheet = excel.Workbook.Worksheets.Add("Company Summary");
            var types = summary.AchievementTypes;
            var width = 1 + 3 * types.Count;

            var operationNumber = 0;
            var operationCount = 1 + (parameters.IsShowChart ? types.Count * ChartOperationWeight : 0);

            ProgressCallback(OperationWritingData, operationNumber++, operationCount);

            sheet.Column(1).Width = 40;
            for (var column = 2; column <= width; column++)
                sheet.Column(column).Width = 10;

            var rowNumber = 1;

            ComplianceSummaryReportHelper.Timestamp(sheet, ref rowNumber, width);
            ComplianceSummaryReportHelper.CompanyName(sheet, ref rowNumber, width, summary.CompanyName);
            ComplianceSummaryReportHelper.ReportTitle(sheet, ref rowNumber, width, parameters.ReportTitle);

            {
                var row = sheet.Row(rowNumber);
                row.Height = 20;

                var cell = sheet.Cells[rowNumber, 1, rowNumber, width];
                cell.Merge = true;
                cell.Value = $"Company summary for {summary.Departments.Count:n0} departments";
                cell.StyleName = "White Bold";

                rowNumber++;
            }

            // Column names: one merged heading per achievement type over its three figures

            var headerRow = rowNumber;

            {
                sheet.Row(rowNumber).Height = 33;
                sheet.Row(rowNumber + 1).Height = 20;

                var cell1 = sheet.Cells[rowNumber, 1, rowNumber + 1, 1];
                cell1.Merge = true;
                cell1.Value = "Department";
                cell1.StyleName = "Blue Bold";

                var names = new[] { "Validated", "Required", "Score" };

                for (var i = 0; i < types.Count; i++)
                {
                    var column = 2 + 3 * i;

                    var heading = sheet.Cells[rowNumber, column, rowNumber, column + 2];
                    heading.Merge = true;
                    heading.Value = types[i].Name;
                    heading.StyleName = "Blue Bold Small Centered";

                    for (var j = 0; j < names.Length; j++)
                    {
                        var cell = sheet.Cells[rowNumber + 1, column + j];
                        cell.Value = names[j];
                        cell.StyleName = "Blue Bold Small Centered";
                    }
                }

                rowNumber += 2;
            }

            sheet.View.FreezePanes(rowNumber, 2);

            var number = 1;

            foreach (var department in summary.Departments)
            {
                var isEven = number % 2 == 0;

                WriteCompanyRow(
                    department.Name,
                    sequence => summary.GetCell(department.Identifier, sequence),
                    isEven ? "Even" : "Odd",
                    isEven ? "Even Centered" : "Odd Centered");

                number++;
            }

            WriteCompanyRow("Company total", summary.GetTotal, "White Bold BT", "White Bold Centered BT");

            var tableEndRow = rowNumber - 1;

            if (parameters.IsShowChart)
            {
                var charts = new List<ComplianceSummaryReportHelper.ChartData>();

                foreach (var type in types)
                {
                    rowNumber = ComplianceSummaryReportHelper.Separator(sheet, rowNumber, width, 20);

                    {
                        var row = sheet.Row(rowNumber);
                        row.Height = 20;

                        var cell = sheet.Cells[rowNumber, 1, rowNumber, CompanyChartWidth];
                        cell.Merge = true;
                        cell.Value = type.Name + " score by department";
                        cell.StyleName = "White Bold";

                        rowNumber++;
                    }

                    var items = summary.Departments
                        .Select((department, index) => new ComplianceSummaryReportHelper.CurrentAchievementItem
                        {
                            Info = new ComplianceSummaryReportHelper.AchievementInfo(index, department.Name),
                            Data = summary.GetCell(department.Identifier, type.Sequeunce)
                        })
                        .ToList();

                    items.Add(new ComplianceSummaryReportHelper.CurrentAchievementItem
                    {
                        Info = new ComplianceSummaryReportHelper.AchievementInfo(items.Count, "Company total"),
                        Data = summary.GetTotal(type.Sequeunce)
                    });

                    rowNumber = ComplianceSummaryReportHelper.AddBarChartData(sheet, rowNumber, type.Name + " Chart", CompanyChartWidth, items, charts);
                }

                foreach (var chartData in charts)
                {
                    ProgressCallback(OperationAddingCharts, operationNumber, operationCount);
                    operationNumber += ChartOperationWeight;
                    ComplianceSummaryReportHelper.AddBarChart(sheet, chartData);
                }
            }

            var printEndRow = parameters.IsShowChart ? rowNumber - 1 : tableEndRow;

            sheet.PrinterSettings.PrintArea = sheet.Cells[1, 1, printEndRow, width];
            sheet.PrinterSettings.RepeatRows = sheet.Cells[$"{headerRow}:{headerRow + 1}"];

            return true;

            void WriteCompanyRow(string name, Func<int, ComplianceSummaryReportHelper.CurrentAchievementData> getData, string nameCellStyle, string dataCellStyle)
            {
                var cell1 = sheet.Cells[rowNumber, 1];
                cell1.Value = name;
                cell1.StyleName = nameCellStyle;

                for (var i = 0; i < types.Count; i++)
                {
                    var column = 2 + 3 * i;
                    var data = getData(types[i].Sequeunce);

                    var validated = sheet.Cells[rowNumber, column];
                    validated.Value = data.Satisfied;
                    validated.StyleName = dataCellStyle;
                    validated.Style.Numberformat.Format = "#,##0";

                    var required = sheet.Cells[rowNumber, column + 1];
                    required.Value = data.Required;
                    required.StyleName = dataCellStyle;
                    required.Style.Numberformat.Format = "#,##0";

                    var score = sheet.Cells[rowNumber, column + 2];
                    score.StyleName = dataCellStyle;
                    score.Style.Numberformat.Format = "0%";

                    if (data.Score.HasValue)
                    {
                        var value = TruncateScore(data.Score.Value);

                        score.Value = value;
                        score.Style.Font.Color.SetColor(value >= 1 ? Color.Green : Color.Red);
                    }
                    else
                    {
                        score.Value = "NA";
                    }
                }

                rowNumber++;
            }
        }

        #endregion

        #endregion

        #region Helper methods

        public static string GetReportTitle(bool isShowStatus = false)
        {
            return "Compliance Summary Report" + (isShowStatus ? " (with Statuses)" : string.Empty);
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

        // Scores are cut to a whole percent rather than rounded, so 99.6% never reads as 100%.
        private static decimal TruncateScore(decimal score)
        {
            return Math.Truncate(100 * score) / 100;
        }

        protected static string FormatScore(decimal? score, string nullValue = "NA")
        {
            return !score.HasValue ? nullValue : $"{TruncateScore(score.Value):p0}";
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
