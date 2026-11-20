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
                    throw new InvalidOperationException("The operation is not applicable to this object.");

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

        #region Classes (company summary)

        // One row per department and one column per achievement type, plus a company total per type. The cells
        // are built with the same CurrentAchievementData as the per-department report, so the figures agree.
        // The total is the sum of the department figures: a person in two departments counts in both.
        public class CompanySummary
        {
            #region Classes

            public class Department
            {
                public Guid Identifier { get; set; }
                public string Name { get; set; }
            }

            #endregion

            #region Properties

            public string CompanyName { get; private set; }
            public List<Department> Departments { get; private set; }
            public List<AchievementInfo> AchievementTypes { get; private set; }

            #endregion

            #region Fields

            private readonly Dictionary<(Guid, int), CurrentAchievementData> _cells = new Dictionary<(Guid, int), CurrentAchievementData>();
            private readonly Dictionary<int, CurrentAchievementData> _totals = new Dictionary<int, CurrentAchievementData>();

            #endregion

            #region Construction

            private CompanySummary()
            {
            }

            public static CompanySummary Create(IEnumerable<CmdsReportHelper.ComplianceSummary> rows, Dictionary<string, string> achievementDisplayMapping)
            {
                var result = new CompanySummary();
                var departments = new Dictionary<Guid, Department>();
                var achievements = new Dictionary<int, AchievementInfo>();

                foreach (var row in rows)
                {
                    if (result.CompanyName == null)
                        result.CompanyName = row.CompanyName;

                    if (!departments.ContainsKey(row.DepartmentIdentifier))
                        departments.Add(row.DepartmentIdentifier, new Department { Identifier = row.DepartmentIdentifier, Name = row.DepartmentName });

                    if (!achievements.ContainsKey(row.Sequence))
                        achievements.Add(row.Sequence, new AchievementInfo(row.Sequence, achievementDisplayMapping.GetOrDefault(row.Heading, row.Heading)));

                    var key = (row.DepartmentIdentifier, row.Sequence);

                    if (result._cells.TryGetValue(key, out var cell))
                        cell.Append(row);
                    else
                        result._cells.Add(key, new CurrentAchievementData(row));

                    if (result._totals.TryGetValue(row.Sequence, out var total))
                        total.Append(row);
                    else
                        result._totals.Add(row.Sequence, new CurrentAchievementData(row));
                }

                result.Departments = departments.Values
                    .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // A type with nothing required and nothing validated anywhere scores NA in every department.
                result.AchievementTypes = achievements.Values
                    .Where(x => result._totals[x.Sequeunce].Score.HasValue)
                    .OrderBy(x => x.Sequeunce)
                    .ToList();

                return result;
            }

            #endregion

            #region Methods

            public CurrentAchievementData GetCell(Guid department, int sequence)
            {
                return _cells.TryGetValue((department, sequence), out var cell) ? cell : CurrentAchievementData.Empty;
            }

            public CurrentAchievementData GetTotal(int sequence)
            {
                return _totals.TryGetValue(sequence, out var total) ? total : CurrentAchievementData.Empty;
            }

            #endregion
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

        #endregion

        #region Methods (sheet names)

        // Excel rejects a worksheet name longer than 31 characters or containing any of []:*?/\ and treats names
        // case-insensitively, so a department name is cleaned, cut to fit and given a numbered suffix when two
        // departments would otherwise collide.
        public static string CreateSheetName(string name, HashSet<string> used)
        {
            var source = string.IsNullOrWhiteSpace(name) ? "Department" : name;

            var clean = new string(source.Select(c => "[]:*?/\\".IndexOf(c) >= 0 ? ' ' : c).ToArray()).Trim();

            if (clean.Length == 0)
                clean = "Department";

            if (clean.Length > 31)
                clean = clean.Substring(0, 31);

            var candidate = clean;
            var suffix = 2;
            var isUnique = used.Add(candidate);

            while (!isUnique)
            {
                var tail = " (" + suffix + ")";
                suffix++;

                candidate = clean.Length + tail.Length > 31
                    ? clean.Substring(0, 31 - tail.Length) + tail
                    : clean + tail;

                isUnique = used.Add(candidate);
            }

            return candidate;
        }

        #endregion

        #region Methods (titles)

        public static void Timestamp(ExcelWorksheet sheet, ref int rowNumber, int width)
        {
            var user = CurrentSessionState.Identity.User;
            var userTime = TimeZones.ConvertTime(DateTimeOffset.Now, user.TimeZone);
            var timezoneAbbrv = TimeZones.GetAbbreviation(user.TimeZone);
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
