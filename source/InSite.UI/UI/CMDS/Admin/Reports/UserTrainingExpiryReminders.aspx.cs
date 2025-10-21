using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Common.Web;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.Reports.Forms
{
    public partial class UserTrainingExpiryReminders : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid[] Departments { get; set; }
            public Guid[] Achievements { get; set; }
            public bool? IsRequired { get; set; }
        }

        private class ReportDataItem
        {
            #region Fields

            private readonly CmdsReportHelper.UserTrainingExpiryReminder _row;

            #endregion

            #region Construction

            public ReportDataItem(CmdsReportHelper.UserTrainingExpiryReminder dbRow)
            {
                _row = dbRow;
            }

            #endregion

            #region Properties

            public string Employee => _row.FullName;

            public string Achievement =>
                _row.AchievementTitle
                + (_row.ExpirationDate.HasValue && _row.ExpirationDate.Value.UtcDateTime > DateTime.UtcNow
                    ? $" (expires {_row.ExpirationDate.Value:MMM d, yyyy})"
                    : string.Empty);

            public string Line1 => $"Please be aware that your {_row.AchievementTitle} ticket has expired and "
                                   + $"our client has requested that we forward an updated copy. In order to be compliant, "
                                   + $"please call {_row.CompanyName} reception so this achievement can be booked immediately. "
                                   + $"If you have supplied us with a valid ticket already, please call {_row.CompanyName} reception today.";

            public string Line2 => $"Upon completion of this achievement, send a copy of your {_row.AchievementTitle} certificate to {_row.CompanyName}.";

            #endregion
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
            DownloadPdf.Click += DownloadPdf_Click;

            ReportButton.Click += ReportButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            OrganizationIdentifier.Value = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            DepartmentField.Visible = OrganizationIdentifier.HasValue;
            DepartmentIdentifier.Filter.OrganizationIdentifier = OrganizationIdentifier.Value.Value;

            if (!Identity.HasAccessToAllCompanies)
                DepartmentIdentifier.Filter.UserIdentifier = User.UserIdentifier;

            var hasDepartments = DepartmentIdentifier.GetCount() > 0;

            DepartmentIdentifier.Enabled = hasDepartments;
            DepartmentIdentifier.EmptyMessage = hasDepartments ? "All Departments" : "None";

            LoadAchievements();
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

            var dataSource = GetReportDataSource();
            if (!dataSource.Any())
                return;

            using (var excel = new ExcelPackage())
            {
                excel.Workbook.CalcMode = ExcelCalcMode.Manual;

                var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                defaultStyle.Font.Name = "Arial";
                defaultStyle.Font.Size = 8;
                defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;

                var employeeStyle = excel.Workbook.Styles.CreateNamedStyle("EmployeeCell");
                employeeStyle.Style.Font.Bold = true;

                var achievementStyle = excel.Workbook.Styles.CreateNamedStyle("AchievementCell");
                achievementStyle.Style.Font.Bold = true;
                achievementStyle.Style.Font.UnderLine = true;
                achievementStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                var line1Style = excel.Workbook.Styles.CreateNamedStyle("Line1");
                line1Style.Style.WrapText = true;

                var line2Style = excel.Workbook.Styles.CreateNamedStyle("Line2");
                line2Style.Style.Font.Bold = true;
                line2Style.Style.Font.Italic = true;
                line2Style.Style.WrapText = true;
                line2Style.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                line2Style.Style.Border.Bottom.Color.SetColor(Color.Black);

                var sheet = excel.Workbook.Worksheets.Add(Route.Title);

                sheet.Column(1).Width = 30;
                sheet.Column(2).Width = 80;

                var rowNumber = 1;
                ExcelRow currentRow;

                foreach (var dataItem in dataSource)
                {
                    var employeeCell = sheet.Cells[rowNumber, 1];
                    employeeCell.Value = dataItem.Employee;
                    employeeCell.StyleName = employeeStyle.Name;

                    var achievementCell = sheet.Cells[rowNumber, 2];
                    achievementCell.Value = dataItem.Achievement;
                    achievementCell.StyleName = achievementStyle.Name;

                    sheet.Row(rowNumber++).Style.ShrinkToFit = true;
                    sheet.Row(rowNumber++).Height = 8;

                    var line1Cell = sheet.Cells[rowNumber, 1, rowNumber, 2];
                    line1Cell.Merge = true;
                    line1Cell.Value = dataItem.Line1;
                    line1Cell.StyleName = line1Style.Name;

                    currentRow = sheet.Row(rowNumber++);
                    currentRow.Style.ShrinkToFit = true;
                    currentRow.Height = 45;

                    var line2Cell = sheet.Cells[rowNumber, 1, rowNumber, 2];
                    line2Cell.Value = dataItem.Line2;
                    line2Cell.Merge = true;
                    line2Cell.StyleName = line2Style.Name;

                    currentRow = sheet.Row(rowNumber++);
                    currentRow.Height = 22.5;
                    currentRow.Style.ShrinkToFit = true;

                    sheet.Row(rowNumber++).Height = 8;
                }

                excel.Workbook.Properties.Title = Route.Title;
                excel.Workbook.Properties.Comments = "This report is used to send reminder notices to employees and contractors for ticket/training items that are near expiration.";

                excel.Workbook.Properties.Company = Organization.Name;
                excel.Workbook.Properties.Author = User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                if (rowNumber > 1)
                    sheet.PrinterSettings.PrintArea = sheet.Cells[1, 1, rowNumber - 1, 2];

                sheet.PrinterSettings.Orientation = eOrientation.Portrait;
                sheet.PrinterSettings.PaperSize = ePaperSize.A4;
                sheet.PrinterSettings.FitToPage = true;
                sheet.PrinterSettings.FitToWidth = 1;
                sheet.PrinterSettings.FitToHeight = 0;

                excel.Workbook.CalcMode = ExcelCalcMode.Automatic;

                ReportXlsxHelper.Export(Route.Title, excel);
            }
        }

        private void DownloadPdf_Click(object sender, EventArgs e)
        {
            var bodyPath = MapPath("~/UI/CMDS/Admin/Reports/UserTrainingExpiryReminders_PdfBody.html");
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

                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/CMDS/Admin/Reports/UserTrainingExpiryReminders_PdfHeader.html"),
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
                Departments = DepartmentIdentifier.Values,
                Achievements = AchievementSelector.GetSelectedAchievements(),
                IsRequired = GetIsRequired()
            };

            if (CurrentParameters.Departments.Length == 0)
                CurrentParameters.Departments = DepartmentIdentifier.GetDataItems().Select(x => x.Value).ToArray();

            if (CurrentParameters.Departments.Length == 0)
                return;

            var dataSource = GetReportDataSource();

            if (!dataSource.Any())
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DataRepeater.DataSource = dataSource;
            DataRepeater.DataBind();
        }

        private IEnumerable<ReportDataItem> GetReportDataSource()
        {
            return CmdsReportHelper.SelectUserTrainingExpiryReminder(CurrentParameters.Departments, CurrentParameters.Achievements, CurrentParameters.IsRequired)
                .Select(x => new ReportDataItem(x))
                .ToArray();
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

        #endregion
    }
}