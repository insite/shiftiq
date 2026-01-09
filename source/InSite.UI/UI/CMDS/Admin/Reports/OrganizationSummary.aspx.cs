using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Common.Web;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;

using Color = System.Drawing.Color;

namespace InSite.Cmds.Admin.Reports.Forms
{
    public partial class CompanySummary : AdminBasePage
    {
        private const string ReportName = "Organization Summary";

        [Serializable]
        private class SearchParameters
        {
            public Guid? OrganizationIdentifier { get; set; }
        }

        private SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DataRepeater.ItemDataBound += DataRepeater_ItemDataBound;

            DownloadXlsx.Click += DownloadXlsx_Click;
            DownloadPdf.Click += DownloadPdf_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                LoadReport();
            }
        }

        private void DataRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (CmdsReportHelper.CompanySummary)e.Item.DataItem;

            var departmentRepeater = (Repeater)e.Item.FindControl("DepartmentRepeater");
            departmentRepeater.DataSource = dataItem.Departments;
            departmentRepeater.DataBind();

            var roleRepeater = (Repeater)e.Item.FindControl("RoleRepeater");
            roleRepeater.DataSource = dataItem.Roles;
            roleRepeater.DataBind();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            if (CurrentParameters == null)
                return;

            var dataSource = CmdsReportHelper.SelectCompanySummary(CurrentParameters.OrganizationIdentifier);
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

                var sheet = excel.Workbook.Worksheets.Add(ReportName);

                sheet.Column(1).Width = 20;
                sheet.Column(2).Width = 11;
                sheet.Column(3).Width = 30;
                sheet.Column(4).Width = 20;

                var rowNumber = 1;

                var headerCell = sheet.Cells[rowNumber, 1, rowNumber, 4];
                headerCell.Merge = true;
                headerCell.Value = ReportName;
                headerCell.StyleName = reportTitleStyle.Name;

                sheet.Row(rowNumber++).Height = 22.5;
                sheet.Row(rowNumber++).Height = 10;

                foreach (var company in dataSource)
                {
                    var companyNameCell = sheet.Cells[rowNumber, 1, rowNumber, 4];
                    companyNameCell.Merge = true;
                    companyNameCell.Value = company.Name;
                    rowNumber++;

                    sheet.Row(rowNumber++).Height = 10;

                    var depNameHeaderCell = sheet.Cells[rowNumber, 1];
                    depNameHeaderCell.Value = "Name";
                    depNameHeaderCell.StyleName = tableHeaderStyle.Name;

                    var depUserCountHeaderCell = sheet.Cells[rowNumber, 2];
                    depUserCountHeaderCell.Value = "User Count";
                    depUserCountHeaderCell.StyleName = tableHeaderStyle.Name;
                    depUserCountHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    rowNumber++;

                    var allDepCell = sheet.Cells[rowNumber, 1];
                    allDepCell.Value = "All Departments";
                    allDepCell.StyleName = tableCellStyle.Name;

                    var allDepUserCountCell = sheet.Cells[rowNumber, 2];
                    allDepUserCountCell.Value = company.UserCount;
                    allDepUserCountCell.StyleName = tableCellStyle.Name;
                    allDepUserCountCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    rowNumber++;

                    foreach (var department in company.Departments)
                    {
                        var depNameCell = sheet.Cells[rowNumber, 1];
                        depNameCell.Value = department.DepartmentName;
                        depNameCell.StyleName = tableCellStyle.Name;

                        var depUserCountCell = sheet.Cells[rowNumber, 2];
                        depUserCountCell.Value = department.UserCount;
                        depUserCountCell.StyleName = tableCellStyle.Name;
                        depUserCountCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        sheet.Row(rowNumber++).Style.WrapText = true;
                    }

                    rowNumber++;

                    var userEmployeeHeaderCell = sheet.Cells[rowNumber, 1];
                    userEmployeeHeaderCell.Value = "Employee";
                    userEmployeeHeaderCell.StyleName = tableHeaderStyle.Name;

                    var userDepartmentsHeaderCell = sheet.Cells[rowNumber, 2, rowNumber, 3];
                    userDepartmentsHeaderCell.Merge = true;
                    userDepartmentsHeaderCell.Value = "Department(s)";
                    userDepartmentsHeaderCell.StyleName = tableHeaderStyle.Name;

                    var userLastAuthenticatedHeaderCell = sheet.Cells[rowNumber, 4];
                    userLastAuthenticatedHeaderCell.Value = "Last Authenticated";
                    userLastAuthenticatedHeaderCell.StyleName = tableHeaderStyle.Name;

                    rowNumber++;

                    var roleNameHeaderCell = sheet.Cells[rowNumber, 1];
                    roleNameHeaderCell.Value = "Role";
                    roleNameHeaderCell.StyleName = tableHeaderStyle.Name;

                    var roleUserCountHeaderCell = sheet.Cells[rowNumber, 2];
                    roleUserCountHeaderCell.Value = "# of Users";
                    roleUserCountHeaderCell.StyleName = tableHeaderStyle.Name;
                    roleUserCountHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    rowNumber++;

                    foreach (var role in company.Roles)
                    {
                        var roleNameCell = sheet.Cells[rowNumber, 1];
                        roleNameCell.Value = role.RoleName;
                        roleNameCell.StyleName = tableCellStyle.Name;

                        var roleUserCountCell = sheet.Cells[rowNumber, 2];
                        roleUserCountCell.Value = role.UserCount;
                        roleUserCountCell.StyleName = tableCellStyle.Name;
                        roleUserCountCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        sheet.Row(rowNumber++).Style.WrapText = true;
                    }

                    sheet.Row(rowNumber++).Height = 20;
                }

                excel.Workbook.Properties.Title = ReportName;

                excel.Workbook.Properties.Company = Organization.Name;
                excel.Workbook.Properties.Author = User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                if (rowNumber > 1)
                    sheet.PrinterSettings.PrintArea = sheet.Cells[1, 1, rowNumber - 1, 4];

                sheet.PrinterSettings.Orientation = eOrientation.Portrait;
                sheet.PrinterSettings.PaperSize = ePaperSize.A4;
                sheet.PrinterSettings.FitToPage = true;
                sheet.PrinterSettings.FitToWidth = 1;
                sheet.PrinterSettings.FitToHeight = 0;

                ReportXlsxHelper.Export(ReportName, excel);
            }
        }

        private void DownloadPdf_Click(object sender, EventArgs e)
        {
            var bodyPath = MapPath("~/UI/CMDS/Admin/Reports/CompanySummary_PdfBody.html");
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
                .Replace("<!-- PUT TITLE HERE -->", ReportName)
                .Replace("<!-- PUT HTML CODE HERE -->", dataHtml);

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),

                MarginTop = 22,

                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/CMDS/Admin/Reports/CompanySummary_PdfHeader.html"),
                HeaderSpacing = 8
            };

            var data = HtmlConverter.HtmlToPdf(bodyHtml, settings);
            if (data == null)
                return;

            var filename = StringHelper.Sanitize(ReportName, '-', false);

            Response.SendFile(filename, "pdf", data);
        }

        private void LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier
            };

            var dataSource = CmdsReportHelper.SelectCompanySummary(CurrentParameters.OrganizationIdentifier);

            PreviewSection.Visible = true;

            DataRepeater.DataSource = dataSource;
            DataRepeater.DataBind();

            DownloadCommandsPanel.Visible = dataSource.Any();
        }

    }
}
