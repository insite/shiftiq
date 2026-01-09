using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Cmds.Actions.Reporting.Report;
using InSite.Common.Web;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.Reports.Forms
{
    public partial class DepartmentProfileSummary : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid? DepartmentIdentifier { get; set; }
            public Guid? ProfileStandardIdentifier { get; set; }
        }

        #endregion

        #region Properties

        private PersonFinderSecurityInfoWrapper FinderSecurityInfo => _finderSecurityInfo
            ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        private SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        #endregion

        #region Fields

        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportButton.Click += ReportButton_Click;

            Department.AutoPostBack = true;
            Department.ValueChanged += Department_ValueChanged;

            DataRepeater.ItemCreated += DataRepeater_ItemCreated;
            DataRepeater.ItemDataBound += DataRepeater_ItemDataBound;

            DownloadXlsx.Click += DownloadXlsx_Click;
            DownloadPdf.Click += DownloadPdf_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            FinderSecurityInfo.LoadPermissions();

            InitSelectors();

            Department.Value = null;
            CurrentProfile.Value = null;
        }

        private void InitSelectors()
        {
            Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!Identity.HasAccessToAllCompanies)
                Department.Filter.UserIdentifier = User.UserIdentifier;

            Department.Value = null;

            CurrentProfile.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            CurrentProfile.Filter.DepartmentIdentifier = Department.Value;
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void Department_ValueChanged(object sender, EventArgs e)
        {
            CurrentProfile.Value = null;

            CurrentProfile.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            CurrentProfile.Filter.DepartmentIdentifier = Department.Value;
        }

        private void DataRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var employeeRepeater = (Repeater)e.Item.FindControl("EmployeeRepeater");
            employeeRepeater.ItemDataBound += EmployeeRepeater_ItemDataBound;
        }

        private void DataRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (CmdsReportHelper.DepartmentProfileSummary)e.Item.DataItem;

            var employeeRepeater = (Repeater)e.Item.FindControl("EmployeeRepeater");
            employeeRepeater.DataSource = dataItem.Employees;
            employeeRepeater.DataBind();
        }

        private void EmployeeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var employee = (CmdsReportHelper.DepartmentProfileSummary.Employee)e.Item.DataItem;

            var employeeStatusRepeater = (Repeater)e.Item.FindControl("EmployeeStatusRepeater");
            employeeStatusRepeater.DataSource = employee.EmployeeStatuses;
            employeeStatusRepeater.DataBind();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            if (CurrentParameters == null)
                return;

            var dataSource = CmdsReportHelper.SelectDepartmentProfileSummary(CurrentParameters.OrganizationIdentifier, CurrentParameters.DepartmentIdentifier, CurrentParameters.ProfileStandardIdentifier);

            if (!dataSource.Any())
                return;

            var personNameCache = ServiceLocator.PersonSearch.GetNames(CurrentParameters.OrganizationIdentifier);

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

                sheet.Column(1).Width = 20;
                sheet.Column(2).Width = 20;
                sheet.Column(3).Width = 10;

                var rowNumber = 1;

                var headerCell = sheet.Cells[rowNumber, 1, rowNumber, 3];
                headerCell.Merge = true;
                headerCell.Value = Route.Title;
                headerCell.StyleName = reportTitleStyle.Name;

                sheet.Row(rowNumber++).Height = 22.5;
                sheet.Row(rowNumber++).Height = 10;

                foreach (var department in dataSource)
                {
                    var departmentCell = sheet.Cells[rowNumber, 1, rowNumber, 3];
                    departmentCell.Merge = true;
                    departmentCell.Value = $"{department.CompanyName}: {department.DepartmentName}";
                    departmentCell.Style.Font.Size = 15;

                    sheet.Row(rowNumber).Height = 18;

                    rowNumber++;

                    var profileTitleCell = sheet.Cells[rowNumber, 1, rowNumber, 3];
                    profileTitleCell.Merge = true;
                    profileTitleCell.Value = department.ProfileTitle;
                    profileTitleCell.Style.Font.Size = 13;
                    rowNumber++;

                    var competencyCountCell = sheet.Cells[rowNumber, 1, rowNumber, 3];
                    competencyCountCell.Merge = true;
                    competencyCountCell.Value = $"{department.CompetencyCount} Competencies as at {DateTime.Today.ToString("MMM d, yyy")}";

                    rowNumber++;
                    sheet.Row(rowNumber++).Height = 10;

                    var userHeaderCell = sheet.Cells[rowNumber, 1];
                    userHeaderCell.Value = "Name";
                    userHeaderCell.StyleName = tableHeaderStyle.Name;

                    var devPlanHeaderCell = sheet.Cells[rowNumber, 2, rowNumber, 3];
                    devPlanHeaderCell.Merge = true;
                    devPlanHeaderCell.Value = "Development Plan";
                    devPlanHeaderCell.StyleName = tableHeaderStyle.Name;

                    foreach (var employee in department.Employees)
                    {
                        rowNumber++;

                        var employeeCell = sheet.Cells[rowNumber, 1];
                        employeeCell.Value = GetPersonFullName(personNameCache, employee.UserIdentifier, department.OrganizationIdentifier);
                        employeeCell.StyleName = tableCellStyle.Name;

                        foreach (var status in employee.EmployeeStatuses)
                        {
                            var statusCell = sheet.Cells[rowNumber, 2];
                            statusCell.Value = status.ValidationStatus;
                            statusCell.StyleName = tableCellStyle.Name;

                            var competencyCell = sheet.Cells[rowNumber, 3];
                            competencyCell.Value = status.CompetencyCount;
                            competencyCell.StyleName = tableCellStyle.Name;
                            competencyCell.Style.WrapText = true;

                            sheet.Row(rowNumber).Style.WrapText = true;

                            rowNumber++;
                        }
                    }

                    rowNumber++;

                    sheet.Row(rowNumber).Height = 20;

                    rowNumber++;
                }

                excel.Workbook.Properties.Title = Route.Title;

                excel.Workbook.Properties.Company = Organization.Name;
                excel.Workbook.Properties.Author = User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                if (rowNumber > 1)
                    sheet.PrinterSettings.PrintArea = sheet.Cells[1, 1, rowNumber - 1, 3];

                sheet.PrinterSettings.Orientation = eOrientation.Portrait;
                sheet.PrinterSettings.PaperSize = ePaperSize.A4;
                sheet.PrinterSettings.FitToPage = true;
                sheet.PrinterSettings.FitToWidth = 1;
                sheet.PrinterSettings.FitToHeight = 0;

                ReportXlsxHelper.Export(Route.Title, excel);
            }
        }

        private string GetPersonFullName(List<PersonName> cache, Guid userId, Guid organizationIdentifier)
        {
            var item = cache.FirstOrDefault(x => x.UserId == userId && x.OrganizationId == organizationIdentifier);

            if (item != null)
                return item.FullPerson ?? item.FullUser;

            return "(Missing Full Name)";
        }

        private void DownloadPdf_Click(object sender, EventArgs e)
        {
            var bodyPath = MapPath("~/UI/CMDS/Admin/Reports/DepartmentProfileSummary_PdfBody.html");
            var bodyHtml = File.ReadAllText(bodyPath);

            var sb = new StringBuilder();

            using (var stringWriter = new StringWriter(sb))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                {
                    DataRepeater.RenderControl(htmlWriter);
                }
            }

            var dataHtml = HtmlHelper.ResolveRelativePaths(Page.Request.Url.Scheme + "://" + Page.Request.Url.Host + Page.Request.RawUrl, sb);

            bodyHtml = bodyHtml
                .Replace("<!-- PUT TITLE HERE -->", Route.Title)
                .Replace("<!-- PUT HTML CODE HERE -->", dataHtml);

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),

                MarginTop = 22,

                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/CMDS/Admin/Reports/DepartmentProfileSummary_PdfHeader.html"),
                HeaderSpacing = 8
            };

            var data = HtmlConverter.HtmlToPdf(bodyHtml, settings);
            if (data == null)
                return;

            var filename = StringHelper.Sanitize(Route.Title, '-', false);

            Response.SendFile(filename, "pdf", data);
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                DepartmentIdentifier = Department.Value,
                ProfileStandardIdentifier = CurrentProfile.Value
            };

            var dataSource = CmdsReportHelper.SelectDepartmentProfileSummary(CurrentParameters.OrganizationIdentifier, CurrentParameters.DepartmentIdentifier, CurrentParameters.ProfileStandardIdentifier)
                .OrderBy(x => x.CompanyName)
                .ThenBy(x => x.DepartmentName)
                .ThenBy(x => x.ProfileTitle);

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DataRepeater.DataSource = dataSource;
            DataRepeater.DataBind();

            DownloadCommandsPanel.Visible = dataSource.Any();
        }

        #endregion
    }
}