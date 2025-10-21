using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using InSite.Persistence.Plugin.CMDS;

using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;

using Shift.Common;

namespace InSite.Cmds.Admin.Reports.Forms
{
    public class ComplianceSummaryReportHelper
    {
        #region Helper methods

        private static int? Sum(int? value1, int? value2)
        {
            if (value1 == 0)
                value1 = null;

            if (value2 == 0)
                value2 = null;

            if (value1.HasValue && value2.HasValue)
                return value1.Value + value2.Value;

            return value1 ?? value2;
        }

        #endregion

        #region Classes

        public class DepartmentInfo : GroupNode<EmployeeInfo>, IComparable<DepartmentInfo>
        {
            #region Construction

            public DepartmentInfo(string company, string department)
            {
                CompanyName = company;
                DepartmentName = department;
            }

            #endregion

            #region Methods

            public int CompareTo(DepartmentInfo other)
            {
                return string.Compare(DepartmentName, other.DepartmentName, StringComparison.Ordinal);
            }

            #endregion

            #region Properties

            public string CompanyName { get; }
            public string DepartmentName { get; }

            public CmdsReportHelper.EmployeeComplianceHistoryChart[] ChartData { get; internal set; }

            #endregion
        }

        public class EmployeeInfo : DefaultGroupLeaf
        {
            public string PrimaryProfileName { get; set; }
        }

        public class AchievementInfo : GroupLeaf, IComparable<AchievementInfo>
        {
            #region Construction

            public AchievementInfo(int sequeunce, string name)
            {
                Sequeunce = sequeunce;
                Name = name;
            }

            #endregion

            #region Methods

            public int CompareTo(AchievementInfo other)
            {
                return Sequeunce.CompareTo(Sequeunce);
            }

            #endregion

            #region Properties

            public int Sequeunce { get; }
            public string Name { get; }

            #endregion
        }

        public class ChartData
        {
            public int RowNumber { get; set; }
            public string ChartName { get; set; }
            public int Width { get; set; }
            public int DataRowCount { get; set; }
        }

        public class LineChartSerieData
        {
            public int Row { get; set; }
            public int DateRow { get; set; }
            public int Column { get; set; }
            public int AchievementIndex { get; set; }
        }

        public class LineChartData
        {
            public string ChartName { get; set; }
            public int ReportRowNumber { get; set; }
            public int Width { get; set; }
            public List<LineChartSerieData> Series { get; set; }
        }

        #endregion

        #region Classes (current)

        public class CurrentReportDataSource : GroupTable<DepartmentInfo, AchievementInfo, CurrentAchievementData>
        {
            public IEnumerable<CurrentAchievementItem> GetAchievements(GroupLeaf col)
            {
                return Rows
                    .Select(x => new CurrentAchievementItem
                    {
                        Info = x,
                        Data = GetCell(col, x)
                    })
                    .OrderBy(x => x.Info.Sequeunce);
            }
        }

        public class CurrentAchievementItem
        {
            #region Properties

            public AchievementInfo Info { get; set; }
            public CurrentAchievementData Data
            {
                get => _data;
                set => _data = value ?? CurrentAchievementData.Empty;
            }

            #endregion

            #region Fields

            private CurrentAchievementData _data = CurrentAchievementData.Empty;

            #endregion
        }

        public class CurrentAchievementData
        {
            #region Properties

            public static CurrentAchievementData Empty { get; } = new CurrentAchievementData();

            #endregion

            #region Construction

            private CurrentAchievementData()
            {
                _isEmpty = true;
            }

            public CurrentAchievementData(CmdsReportHelper.ComplianceSummary row)
            {
                Append(row);
            }

            #endregion

            #region Methods

            public void Append(CmdsReportHelper.ComplianceSummary row)
            {
                if (_isEmpty)
                    throw new ApplicationError("The operation is not applicable to this object.");

                Expired = Sum(Expired, row.Expired);
                NotCompleted = Sum(NotCompleted, row.NotCompleted);
                NotApplicable = Sum(NotApplicable, row.NotApplicable);
                NeedsTraining = Sum(NeedsTraining, row.NeedsTraining);
                SelfAssessed = Sum(SelfAssessed, row.SelfAssessed);
                Submitted = Sum(Submitted, row.Submitted);
                Required += row.Required;
                Satisfied += row.Satisfied;
            }

            #endregion

            #region Properties

            public int? Expired { get; private set; }
            public int? NotCompleted { get; private set; }
            public int? NotApplicable { get; private set; }
            public int? NeedsTraining { get; private set; }
            public int? SelfAssessed { get; private set; }
            public int? Submitted { get; private set; }
            public int Required { get; private set; }
            public int Satisfied { get; private set; }

            public decimal? Score
            {
                get
                {
                    if (Required == 0 && Satisfied == 0)
                        return null;

                    if (Required == 0)
                        return 1m;

                    return Math.Round(Satisfied / (decimal)Required, 3);
                }
            }

            #endregion

            #region Fields

            private bool _isEmpty = false;

            #endregion
        }

        #endregion

        #region Classes (history)

        public class HistoryReportDataSource : GroupTable<DepartmentInfo, AchievementInfo, HistoryAchievementData>
        {
            public DateTime? SnapshotDate1 { get; set; }
            public DateTime? SnapshotDate2 { get; set; }
            public DateTime? SnapshotDate3 { get; set; }

            public IEnumerable<HistoryAchievementItem> GetAchievements(GroupLeaf col)
            {
                return Rows
                    .Select(x => new HistoryAchievementItem
                    {
                        Info = x,
                        Data = GetCell(col, x)
                    })
                    .OrderBy(x => x.Info.Sequeunce);
            }
        }

        public class HistoryAchievementItem
        {
            #region Properties

            public AchievementInfo Info { get; set; }

            public HistoryAchievementData Data
            {
                get => _data;
                set => _data = value ?? HistoryAchievementData.Empty;
            }

            #endregion

            #region Fields

            private HistoryAchievementData _data = null;

            #endregion
        }

        public class HistoryAchievementData
        {
            #region Properties

            public static HistoryAchievementData Empty { get; } = new HistoryAchievementData();

            #endregion

            #region Construction

            private HistoryAchievementData()
            {
                _isEmpty = true;
            }

            public HistoryAchievementData(CmdsReportHelper.EmployeeComplianceHistory row)
            {
                Append(row);
            }

            #endregion

            #region Methods

            public void Append(CmdsReportHelper.EmployeeComplianceHistory row)
            {
                if (_isEmpty)
                    throw new ApplicationError("The operation is not applicable to this object.");

                Expired = Sum(Expired, row.Expired);
                NotCompleted = Sum(NotCompleted, row.NotCompleted);
                NotApplicable = Sum(NotApplicable, row.NotApplicable);
                NeedsTraining = Sum(NeedsTraining, row.NeedsTraining);
                SelfAssessed = Sum(SelfAssessed, row.SelfAssessed);
                Submitted = Sum(Submitted, row.Submitted);
                Required += row.Required;
                Satisfied += row.Satisfied;

                _score.Add(row.Score);

                if (row.Score1.HasValue)
                    _score1.Add(row.Score1.Value);

                if (row.Score2.HasValue)
                    _score2.Add(row.Score2.Value);

                if (row.Score3.HasValue)
                    _score3.Add(row.Score3.Value);
            }

            #endregion

            #region Properties

            public int? Expired { get; private set; }
            public int? NotCompleted { get; private set; }
            public int? NotApplicable { get; private set; }
            public int? NeedsTraining { get; private set; }
            public int? SelfAssessed { get; private set; }
            public int? Submitted { get; private set; }
            public int Required { get; private set; }
            public int Satisfied { get; private set; }
            public decimal? Score => _score.Count == 0 ? (decimal?)null : _score.Average();
            public decimal? Score1 => _score1.Count == 0 ? (decimal?)null : _score1.Average();
            public decimal? Score2 => _score2.Count == 0 ? (decimal?)null : _score2.Average();
            public decimal? Score3 => _score3.Count == 0 ? (decimal?)null : _score3.Average();

            #endregion

            #region Fields

            private readonly bool _isEmpty = false;
            private List<decimal> _score { get; } = new List<decimal>();
            private List<decimal> _score1 { get; } = new List<decimal>();
            private List<decimal> _score2 { get; } = new List<decimal>();
            private List<decimal> _score3 { get; } = new List<decimal>();

            #endregion
        }

        public class HistoryDepartmentChartDataItem
        {
            public string Name { get; set; }
            public decimal? Score { get; set; }
            public decimal? Score1 { get; set; }
            public decimal? Score2 { get; set; }
            public decimal? Score3 { get; set; }
        }

        #endregion

        #region Methods (charts)

        public static int AddBarChartData(ExcelWorksheet sheet, int rowNumber, string chartName, int width, IEnumerable<CurrentAchievementItem> achievements, List<ChartData> charts)
        {
            // Data Table

            var dataTableRowNumber = rowNumber;

            foreach (var item in achievements)
            {
                decimal? score = null;
                if (item.Data?.Score != null)
                    score = item.Data.Score;

                var cell1 = sheet.Cells[dataTableRowNumber, 1];
                cell1.Value = item.Info.Name;

                var cell2 = sheet.Cells[dataTableRowNumber, 2];
                cell2.Style.Numberformat.Format = "0%";
                cell2.Value = score.HasValue && score >= 1 ? (object)score : "";

                var cell3 = sheet.Cells[dataTableRowNumber, 3];
                cell3.Style.Numberformat.Format = "0%";
                cell3.Value = score.HasValue && score < 1 ? (object)score : "";

                dataTableRowNumber++;
            }

            // Chart

            charts.Add(new ChartData { RowNumber = rowNumber, ChartName = chartName, Width = width, DataRowCount = achievements.Count() });

            return rowNumber + (int)(achievements.Count() * 1.5d) + 1;
        }

        public static void AddBarChart(ExcelWorksheet sheet, ChartData chartData)
        {
            var chart = (ExcelBarChart)sheet.Drawings.AddChart(chartData.ChartName, eChartType.BarStacked);

            var serieSuccess = (ExcelBarChartSerie)chart.Series.Add(
                sheet.Cells[chartData.RowNumber, 2, chartData.RowNumber + chartData.DataRowCount - 1, 2],
                sheet.Cells[chartData.RowNumber, 1, chartData.RowNumber + chartData.DataRowCount - 1, 1]);

            serieSuccess.Fill.Style = eFillStyle.SolidFill;
            serieSuccess.Fill.Color = Color.SeaGreen;
            serieSuccess.Fill.Transparancy = 0;

            var serieFail = (ExcelBarChartSerie)chart.Series.Add(
                sheet.Cells[chartData.RowNumber, 3, chartData.RowNumber + chartData.DataRowCount - 1, 3],
                sheet.Cells[chartData.RowNumber, 1, chartData.RowNumber + chartData.DataRowCount - 1, 1]);

            serieFail.Fill.Style = eFillStyle.SolidFill;
            serieFail.Fill.Color = Color.Firebrick;
            serieFail.Fill.Transparancy = 0;

            chart.RoundedCorners = false;

            chart.Fill.Style = eFillStyle.SolidFill;
            chart.Fill.Color = Color.White;
            chart.Fill.Transparancy = 0;

            chart.PlotArea.Fill.Style = eFillStyle.SolidFill;
            chart.PlotArea.Fill.Color = Color.White;
            chart.PlotArea.Fill.Transparancy = 0;
            chart.PlotArea.Border.LineStyle = eLineStyle.Solid;
            chart.PlotArea.Border.LineCap = eLineCap.Square;
            chart.PlotArea.Border.Fill.Style = eFillStyle.SolidFill;
            chart.PlotArea.Border.Fill.Color = Color.Gainsboro;
            chart.PlotArea.Border.Fill.Transparancy = 0;

            // X-Axis
            var xAxis = chart.Axis[0];
            xAxis.Orientation = eAxisOrientation.MaxMin;

            xAxis.MajorGridlines.Fill.Style = eFillStyle.SolidFill;
            xAxis.MajorGridlines.Fill.Color = Color.Gainsboro;
            xAxis.MajorGridlines.Fill.Transparancy = 0;
            xAxis.MajorTickMark = eAxisTickMark.None;
            xAxis.MinorTickMark = eAxisTickMark.None;

            // Y-Axis
            var yAxis = chart.Axis[1];
            yAxis.LabelPosition = eTickLabelPosition.High;
            yAxis.MaxValue = 1;

            yAxis.MajorGridlines.Fill.Style = eFillStyle.SolidFill;
            yAxis.MajorGridlines.Fill.Color = Color.Gainsboro;
            yAxis.MajorGridlines.Fill.Transparancy = 0;
            yAxis.MajorTickMark = eAxisTickMark.None;
            yAxis.MinorTickMark = eAxisTickMark.None;

            chart.From.Row = chartData.RowNumber - 1;
            chart.From.Column = 0;

            chartData.RowNumber += (int)(chartData.DataRowCount * 1.5d);

            chart.To.Row = chartData.RowNumber;
            chart.To.Column = chartData.Width;
            chart.Legend.Remove();
        }

        public static void AddLineChartData(
            ExcelWorksheet reportSheet, ref int reportRowNumber,
            ExcelWorksheet chartSheet, ref int chartRowNumber,
            DepartmentInfo departmentData,
            string organization,
            int width,
            List<LineChartData> charts)
        {
            var achievementTypeMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector.CreateAchievementLabelMapping(organization);

            var chartData = new LineChartData
            {
                ChartName = departmentData.DepartmentName + " Chart",
                ReportRowNumber = reportRowNumber,
                Width = width,
                Series = new List<LineChartSerieData>()
            };

            charts.Add(chartData);

            // Data Table

            chartSheet.Cells[chartRowNumber++, 1].Value = $"Chart Data for {departmentData.CompanyName}: {departmentData.DepartmentName}";

            var chartDateRow = chartRowNumber;
            var dates = departmentData.ChartData
                .Where(x => x.SnapshotDate.HasValue)
                .Select(x => x.SnapshotDate.Value)
                .OrderBy(x => x)
                .Distinct()
                .ToList();

            var columnNumber = 2;

            foreach (var date in dates)
            {
                var cell = chartSheet.Cells[chartRowNumber, columnNumber++];
                cell.Value = date;
                cell.Style.Numberformat.Format = "MMM d, yyyy";
            }

            {
                dates.Add(DateTime.MaxValue);

                var cell = chartSheet.Cells[chartRowNumber, columnNumber];
                cell.Value = "Current";
            }

            chartRowNumber++;

            foreach (var achievementData in departmentData.ChartData.GroupBy(x => x.Sequence).OrderBy(x => x.Key))
            {
                columnNumber = 1;

                var heading = achievementData.First().Heading;
                var name = achievementTypeMapping.GetOrDefault(heading, heading);

                chartSheet.Cells[chartRowNumber, columnNumber++].Value = name;

                var scoresByDate = achievementData.ToDictionary(x => x.SnapshotDate ?? DateTime.MaxValue, x => x.Score);
                foreach (var d in dates)
                {
                    var cell = chartSheet.Cells[chartRowNumber, columnNumber++];
                    cell.Value = scoresByDate.ContainsKey(d) ? (scoresByDate[d] ?? 0) : 0;
                    cell.Style.Numberformat.Format = "0%";
                }

                chartData.Series.Add(new LineChartSerieData { Row = chartRowNumber, DateRow = chartDateRow, Column = columnNumber - 1, AchievementIndex = achievementData.Key - 1 });

                chartRowNumber++;
            }

            chartRowNumber++;
            reportRowNumber += 21;
        }

        public static void AddLineChart(ExcelWorksheet reportSheet, ExcelWorksheet chartSheet, LineChartData chartData)
        {
            var chart = (ExcelLineChart)reportSheet.Drawings.AddChart(chartData.ChartName, eChartType.Line);

            foreach (var serieData in chartData.Series)
            {
                var serie = (ExcelLineChartSerie)chart.Series.Add(
                    chartSheet.Cells[serieData.Row, 2, serieData.Row, serieData.Column],
                    chartSheet.Cells[serieData.DateRow, 2, serieData.DateRow, serieData.Column]);

                serie.HeaderAddress = chartSheet.Cells[serieData.Row, 1];

                var achievementTypeColor = ColorTranslator.FromHtml(AchievementTypes.RetrieveColor(serieData.AchievementIndex));

                var achievementTypeMarker = eMarkerStyle.Dot;

                if (Enum.TryParse(AchievementTypes.RetrieveMarker(serieData.AchievementIndex), out eMarkerStyle marker))
                    achievementTypeMarker = marker;

                serie.Marker.Style = marker;
                serie.Marker.Border.Fill.Color = achievementTypeColor;
                serie.Border.Width = 1;
                serie.Border.Fill.Color = achievementTypeColor;
            }

            chart.RoundedCorners = false;

            chart.Fill.Style = eFillStyle.SolidFill;
            chart.Fill.Color = Color.White;
            chart.Fill.Transparancy = 0;

            chart.PlotArea.Fill.Style = eFillStyle.SolidFill;
            chart.PlotArea.Fill.Color = Color.White;
            chart.PlotArea.Fill.Transparancy = 0;
            chart.PlotArea.Border.LineStyle = eLineStyle.Solid;
            chart.PlotArea.Border.LineCap = eLineCap.Square;
            chart.PlotArea.Border.Fill.Style = eFillStyle.SolidFill;
            chart.PlotArea.Border.Fill.Color = Color.Gainsboro;
            chart.PlotArea.Border.Fill.Transparancy = 0;

            // X-Axis
            var xAxis = chart.Axis[0];

            xAxis.MajorGridlines.Fill.Style = eFillStyle.SolidFill;
            xAxis.MajorGridlines.Fill.Color = Color.Gainsboro;
            xAxis.MajorGridlines.Fill.Transparancy = 0;
            xAxis.MajorTickMark = eAxisTickMark.None;
            xAxis.MinorTickMark = eAxisTickMark.None;

            // Y-Axis
            var yAxis = chart.Axis[1];
            yAxis.MaxValue = 1;

            yAxis.MajorGridlines.Fill.Style = eFillStyle.SolidFill;
            yAxis.MajorGridlines.Fill.Color = Color.Gainsboro;
            yAxis.MajorGridlines.Fill.Transparancy = 0;
            yAxis.MajorTickMark = eAxisTickMark.None;
            yAxis.MinorTickMark = eAxisTickMark.None;

            // Position

            chart.From.Row = chartData.ReportRowNumber;
            chart.From.Column = 0;

            chart.To.Row = chartData.ReportRowNumber + 19;
            chart.To.Column = chartData.Width;

            chart.Legend.Position = eLegendPosition.Top;
        }

        #endregion

        #region Methods (titles)

        public static void Timestamp(ExcelWorksheet sheet, ref int rowNumber, int width)
        {
            var user = CurrentSessionState.Identity.User;
            var userTime = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, user.TimeZone);
            var timezoneAbbrv = user.TimeZone.GetAbbreviation();
            var zz = timezoneAbbrv.GetAbbreviation(userTime);

            var cell = sheet.Cells[rowNumber, 1, rowNumber, width];
            cell.Merge = true;
            cell.Value = $"This report was generated by {user.FullName} on {userTime:MMMM d, yyyy} at {userTime:h:mm tt} {zz}";
            cell.StyleName = "Timestamp";

            rowNumber++;
        }

        public static void CompanyName(ExcelWorksheet sheet, ref int rowNumber, int width, string value)
        {
            var row = sheet.Row(rowNumber);
            row.Height = 40;

            var cell = sheet.Cells[rowNumber, 1, rowNumber, width];
            cell.Merge = true;
            cell.Value = value;
            cell.StyleName = "Blue Bold Big";

            rowNumber++;
        }

        public static void DepartmentName(ExcelWorksheet sheet, ref int rowNumber, int width, string value)
        {
            var row = sheet.Row(rowNumber);
            row.Height = 20;

            var cell = sheet.Cells[rowNumber, 1, rowNumber, width];
            cell.Merge = true;
            cell.Value = "Department Summary for " + value;
            cell.StyleName = "White Bold";

            rowNumber++;
        }

        public static void EmployeeName(ExcelWorksheet sheet, ref int rowNumber, int width, string employeeName, string optionText)
        {
            var row = sheet.Row(rowNumber);
            row.Height = 20;

            var cell1 = sheet.Cells[rowNumber, 1];
            cell1.Value = employeeName;
            cell1.StyleName = "White Bold BT";

            var cell2 = sheet.Cells[rowNumber, 2, rowNumber, width];
            cell2.Merge = true;
            cell2.Value = optionText;
            cell2.StyleName = "White Bold BT";

            rowNumber++;
        }

        public static void ReportTitle(ExcelWorksheet sheet, ref int rowNumber, int width, string value)
        {
            var row = sheet.Row(rowNumber);
            row.Height = 20;

            var cell = sheet.Cells[rowNumber, 1, rowNumber, width];
            cell.Merge = true;
            cell.Value = value;
            cell.StyleName = "Blue Bold";

            rowNumber++;
        }

        public static int Separator(ExcelWorksheet sheet, int rowNumber, int width, int? height = null)
        {
            if (height.HasValue)
            {
                var row = sheet.Row(rowNumber);
                row.Height = height.Value;
            }

            var cell = sheet.Cells[rowNumber, 1, rowNumber, width];
            cell.Merge = true;
            cell.StyleName = "Separator";

            rowNumber++;

            return rowNumber;
        }

        #endregion
    }
}