using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Common.Web;
using InSite.Common.Web.UI.Chart;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;

using Shift.Common;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;
using Color = System.Drawing.Color;

namespace InSite.Cmds.Admin.Reports.Forms
{
    public partial class ComplianceTrendReport : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        protected class SearchParameters
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid[] Departments { get; set; }
            public int[] Sequences { get; set; }
            public int Months { get; set; }
            public int Option { get; set; }
        }

        private class DepartmentTrend
        {
            public string CompanyName { get; set; }
            public string DepartmentName { get; set; }
            public IReadOnlyList<CmdsReportHelper.ComplianceTrend> Rows { get; set; }
        }

        #endregion

        #region Constants

        // One colour per achievement type, indexed by the item number the Compliance Summary report uses.

        private static readonly Color[] SeriesColors =
        {
            Color.FromArgb(31, 119, 180),
            Color.FromArgb(255, 127, 14),
            Color.FromArgb(44, 160, 44),
            Color.FromArgb(214, 39, 40),
            Color.FromArgb(148, 103, 189),
            Color.FromArgb(140, 86, 75),
            Color.FromArgb(227, 119, 194),
            Color.FromArgb(127, 127, 127),
            Color.FromArgb(188, 189, 34),
            Color.FromArgb(23, 190, 207),
            Color.FromArgb(65, 68, 135),
            Color.FromArgb(0, 128, 128)
        };

        #endregion

        #region Properties

        protected SearchParameters ReportParameters
        {
            get => (SearchParameters)ViewState[nameof(ReportParameters)];
            set => ViewState[nameof(ReportParameters)] = value;
        }

        #endregion

        #region Fields

        private Dictionary<string, string> _achievementDisplayMapping = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // The trend reads the weekly snapshot table, which holds several years of history, so allow more than
            // the default 110 seconds.
            Page.Server.ScriptTimeout = 3 * 60;

            ReportButton.Click += ReportButton_Click;

            DownloadXlsxFromCriteria.Click += DownloadXlsxFromCriteria_Click;
            DownloadXlsxFromReport.Click += DownloadXlsxFromReport_Click;
            DownloadPdf.Click += DownloadPdf_Click;

            DepartmentRepeater.ItemDataBound += DepartmentRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            EnsureAchievementDisplayMapping();
            LoadAchievementTypes();
        }

        private void LoadAchievementTypes()
        {
            var types = UserCompetencyRepository.SelectAchievementTypes(_achievementDisplayMapping);

            AchievementType.LoadItems(types);
            AchievementType.SelectAll();
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void DepartmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var department = (DepartmentTrend)e.Item.DataItem;

            var months = department.Rows.Select(x => x.SnapshotDate).Distinct().OrderBy(x => x).ToList();
            var items = department.Rows
                .GroupBy(x => new { x.ItemNumber, x.Heading })
                .OrderBy(x => x.Key.ItemNumber)
                .ToList();

            // A department can hold snapshot rows where nothing was ever required, which leaves every score
            // NULL. Plotting that gives an empty canvas under a full legend, which reads as a broken chart
            // rather than as "nothing was required", so say it in words and keep the table for the detail.

            var hasScores = department.Rows.Any(x => x.Score.HasValue);

            ((HtmlGenericControl)e.Item.FindControl("ChartContainer")).Visible = hasScores;
            ((HtmlGenericControl)e.Item.FindControl("NoScoreNote")).Visible = !hasScores;

            var chart = (LineChart)e.Item.FindControl("DepartmentChart");
            chart.Data.Clear();
            chart.Options.Animation.Duration = 0;
            chart.Options.Plugins.Tooltip.Callbacks.LabelJsFunction = "report.onChartTooltipLabelCallback";

            foreach (var item in items)
            {
                var color = SeriesColors[(item.Key.ItemNumber - 1) % SeriesColors.Length];

                var dataset = (DateTimeChartDataset)chart.Data.CreateDataset("item" + item.Key.ItemNumber);
                dataset.Label = GetAchievementName(item.Key.Heading);
                dataset.BackgroundColor = color;
                dataset.BorderColor = color;
                dataset.BorderWidth = 2;
                dataset.Fill = false;
                dataset.LineTension = 0.05M;
                dataset.PointRadius = 2;

                // A month where nothing was required has no score. Leaving the point out keeps the line honest
                // rather than drawing a zero or carrying the previous month forward.

                foreach (var point in item.OrderBy(x => x.SnapshotDate))
                    if (point.Score.HasValue)
                        dataset.NewItem(point.SnapshotDate, (double)(point.Score.Value * 100));
            }

            var scoreTable = (Literal)e.Item.FindControl("ScoreTable");
            scoreTable.Text = BuildScoreTable(months, items);
        }

        #endregion

        #region Data binding

        private SearchParameters CreateParameters()
        {
            return new SearchParameters
            {
                OrganizationIdentifier = Organization.Identifier,
                Departments = DepartmentIdentifier.Values,
                Sequences = AchievementType.ValuesAsInt.ToArray(),
                Months = int.Parse(MonthCount.SelectedValue),
                Option = int.Parse(Option.SelectedValue)
            };
        }

        private List<DepartmentTrend> LoadDepartments(SearchParameters parameters)
        {
            var rows = CmdsReportHelper.SelectComplianceTrend(
                parameters.OrganizationIdentifier,
                parameters.Departments,
                parameters.Sequences,
                parameters.Months,
                parameters.Option);

            return rows
                .GroupBy(x => x.DepartmentIdentifier)
                .Select(x => new DepartmentTrend
                {
                    CompanyName = Organization.Name,
                    DepartmentName = x.First().DepartmentName,
                    Rows = x.ToList()
                })
                .OrderBy(x => x.DepartmentName)
                .ToList();
        }

        private void LoadReport()
        {
            EnsureAchievementDisplayMapping();

            var parameters = ReportParameters = CreateParameters();

            var data = LoadDepartments(parameters);

            if (data.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Information, "There is no data matching your criteria.");
                return;
            }

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DepartmentRepeater.DataSource = data;
            DepartmentRepeater.DataBind();
        }

        #endregion

        #region Methods

        private string BuildScoreTable(
            IReadOnlyList<DateTime> months,
            IEnumerable<IGrouping<dynamic, CmdsReportHelper.ComplianceTrend>> items)
        {
            var html = new StringBuilder();

            html.Append("<div class=\"table-responsive\"><table class=\"table table-sm table-trend\"><thead><tr>");
            html.Append("<th>Compliance Item</th>");

            foreach (var month in months)
                html.Append("<th class=\"text-center\">").Append(month.ToString("yyyy-MM")).Append("</th>");

            html.Append("</tr></thead><tbody>");

            var number = 1;

            foreach (var item in items)
            {
                var byMonth = item.ToDictionary(x => x.SnapshotDate);

                html.Append("<tr class=\"").Append(number % 2 == 0 ? "even" : "odd").Append("\">");
                html.Append("<td class=\"item-column\">")
                    .Append(HttpUtility.HtmlEncode(GetAchievementName((string)item.Key.Heading)))
                    .Append("</td>");

                foreach (var month in months)
                {
                    html.Append("<td class=\"data-column\"");

                    if (byMonth.TryGetValue(month, out var cell))
                    {
                        html.Append(" style=\"color:").Append(GetScoreHtmlColor(cell.Score)).Append(";\"");
                        html.Append(" title=\"")
                            .Append(cell.Satisfied.ToString("n0"))
                            .Append(" of ")
                            .Append(cell.Required.ToString("n0"))
                            .Append(" satisfied\">");
                        html.Append(FormatScore(cell.Score));
                    }
                    else
                        html.Append('>');

                    html.Append("</td>");
                }

                html.Append("</tr>");

                number++;
            }

            html.Append("</tbody></table></div>");

            return html.ToString();
        }

        private void DownloadXlsxFromCriteria_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                DownloadXlsx(CreateParameters());
        }

        private void DownloadXlsxFromReport_Click(object sender, EventArgs e)
        {
            var parameters = ReportParameters;

            if (parameters != null)
                DownloadXlsx(parameters);
        }

        private void DownloadPdf_Click(object sender, EventArgs e)
        {
            var reportHtml = ReportHtmlContent.Value;
            var hasHtml = reportHtml.IsNotEmpty();

            if (!hasHtml)
                return;

            var physPath = MapPath("~/UI/CMDS/Admin/Reports/ComplianceTrend_PdfBody.html");

            var bodyHtml = File.ReadAllText(physPath)
                .Replace("<!-- TITLE -->", Route.Title)
                .Replace("<!-- BODY -->", reportHtml);

            bodyHtml = HtmlHelper.ResolveRelativePaths(Page.Request.Url.Scheme + "://" + Page.Request.Url.Host + Page.Request.RawUrl, bodyHtml);

            // Landscape, because a 24-month table is wider than it is tall.
            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                PageOrientation = Shift.Constant.PageOrientationType.Landscape,
                Viewport = new HtmlConverterSettings.ViewportSize(1400, 980),

                MarginTop = 22,

                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/CMDS/Admin/Reports/ComplianceTrend_PdfHeader.html"),
                HeaderSpacing = 8
            };

            var data = HtmlConverter.HtmlToPdf(bodyHtml, settings);

            if (data == null)
                return;

            var filename = StringHelper.Sanitize(Route.Title, '-', false);

            Response.SendFile(filename, "pdf", data);
        }

        private void DownloadXlsx(SearchParameters parameters)
        {
            EnsureAchievementDisplayMapping();

            var data = LoadDepartments(parameters);

            if (data.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            using (var excel = new ExcelPackage())
            {
                var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var index = 0;

                foreach (var department in data)
                {
                    var sheetName = ComplianceSummaryReportHelper.CreateSheetName(department.DepartmentName, used);
                    var sheet = excel.Workbook.Worksheets.Add(sheetName);

                    index++;

                    WriteDepartmentSheet(sheet, department, index);
                }

                excel.Workbook.Properties.Title = Route.Title;
                excel.Workbook.Properties.Company = Organization.Name;
                excel.Workbook.Properties.Author = User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                ReportXlsxHelper.Export(Route.Title, excel);
            }
        }

        private void WriteDepartmentSheet(ExcelWorksheet sheet, DepartmentTrend department, int index)
        {
            var months = department.Rows.Select(x => x.SnapshotDate).Distinct().OrderBy(x => x).ToList();
            var items = department.Rows
                .GroupBy(x => new { x.ItemNumber, x.Heading })
                .OrderBy(x => x.Key.ItemNumber)
                .ToList();

            var width = months.Count + 1;

            sheet.Column(1).Width = 40;

            for (var column = 2; column <= width; column++)
                sheet.Column(column).Width = 11;

            var rowNumber = 1;

            var titleCell = sheet.Cells[rowNumber, 1, rowNumber, width];
            titleCell.Merge = true;
            titleCell.Value = department.DepartmentName;
            titleCell.Style.Font.Size = 14;
            titleCell.Style.Font.Bold = true;
            rowNumber++;

            var companyCell = sheet.Cells[rowNumber, 1, rowNumber, width];
            companyCell.Merge = true;
            companyCell.Value = department.CompanyName + " - " + Route.Title;
            companyCell.Style.Font.Color.SetColor(Color.FromArgb(61, 120, 216));
            rowNumber++;

            var stampCell = sheet.Cells[rowNumber, 1, rowNumber, width];
            stampCell.Merge = true;
            stampCell.Value = $"Generated by {User.FullName} on {DateTimeOffset.Now.DateTime:yyyy-MM-dd HH:mm}";
            stampCell.Style.Font.Size = 8;
            stampCell.Style.Font.Color.SetColor(Color.Gray);
            rowNumber += 2;

            var headerRow = rowNumber;

            var headingCell = sheet.Cells[headerRow, 1];
            headingCell.Value = "Compliance Item";
            headingCell.Style.Font.Bold = true;

            for (var i = 0; i < months.Count; i++)
            {
                var cell = sheet.Cells[headerRow, i + 2];
                cell.Value = months[i].ToString("yyyy-MM");
                cell.Style.Font.Bold = true;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            rowNumber++;

            var firstDataRow = rowNumber;

            foreach (var item in items)
            {
                var byMonth = item.ToDictionary(x => x.SnapshotDate);

                sheet.Cells[rowNumber, 1].Value = GetAchievementName((string)item.Key.Heading);

                for (var i = 0; i < months.Count; i++)
                {
                    var cell = sheet.Cells[rowNumber, i + 2];
                    cell.Style.Numberformat.Format = "0%";
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    var hasPoint = byMonth.TryGetValue(months[i], out var point);

                    // A month where nothing was required has no score. Its cell stays empty so the chart leaves a
                    // gap there; text such as "NA" is plotted as zero, which reads as a collapse in compliance.
                    if (hasPoint && point.Score.HasValue)
                    {
                        var score = TruncateScore(point.Score.Value);

                        cell.Value = score;
                        cell.Style.Font.Color.SetColor(score >= 1 ? Color.Green : Color.Red);
                    }
                }

                rowNumber++;
            }

            var lastDataRow = rowNumber - 1;

            // A department whose scores are all NULL charts as an empty plot under a full legend, so the
            // workbook says it in words instead, matching what the page shows for the same department.
            var hasScores = department.Rows.Any(x => x.Score.HasValue);

            if (!hasScores)
            {
                var noteCell = sheet.Cells[rowNumber + 1, 1, rowNumber + 1, width];
                noteCell.Merge = true;
                noteCell.Value = "No compliance requirements were recorded for this department in this period";
                noteCell.Style.Font.Italic = true;
                noteCell.Style.Font.Color.SetColor(Color.Gray);
            }
            else if (months.Count > 1 && lastDataRow >= firstDataRow)
                AddTrendChart(sheet, department, index, headerRow, firstDataRow, lastDataRow, months.Count, rowNumber + 1);
        }

        private static void AddTrendChart(ExcelWorksheet sheet, DepartmentTrend department, int index,
            int headerRow, int firstDataRow, int lastDataRow, int monthCount, int chartRow)
        {
            var chart = (ExcelLineChart)sheet.Drawings.AddChart("trend" + index, eChartType.Line);

            chart.Title.Text = department.DepartmentName;
            chart.DisplayBlanksAs = eDisplayBlanksAs.Gap;

            for (var row = firstDataRow; row <= lastDataRow; row++)
            {
                var serie = chart.Series.Add(
                    sheet.Cells[row, 2, row, monthCount + 1],
                    sheet.Cells[headerRow, 2, headerRow, monthCount + 1]);

                serie.Header = sheet.Cells[row, 1].Text;
            }

            var valueAxis = chart.Axis[1];
            valueAxis.MinValue = 0;
            valueAxis.MaxValue = 1;
            valueAxis.Format = "0%";

            chart.From.Row = chartRow;
            chart.From.Column = 0;
            chart.To.Row = chartRow + 20;
            chart.To.Column = Math.Max(monthCount + 1, 8);
        }

        private void EnsureAchievementDisplayMapping()
        {
            if (_achievementDisplayMapping == null)
                _achievementDisplayMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector.CreateAchievementLabelMapping(Organization.Code);
        }

        private string GetAchievementName(string heading)
        {
            return _achievementDisplayMapping.GetOrDefault(heading, heading);
        }

        // Scores are cut to a whole percent rather than rounded, so 99.6% never reads as 100%. The page and the
        // workbook both go through here, so the same month shows the same number in each.
        private static decimal TruncateScore(decimal score)
        {
            return Math.Truncate(100 * score) / 100;
        }

        protected static string FormatScore(decimal? score, string nullValue = "NA")
        {
            return !score.HasValue ? nullValue : $"{TruncateScore(score.Value):p0}";
        }

        protected static string GetScoreHtmlColor(decimal? score)
        {
            return !score.HasValue ? "inherit" : score.Value >= 1 ? "#008000" : "#ff0000";
        }

        #endregion
    }
}
