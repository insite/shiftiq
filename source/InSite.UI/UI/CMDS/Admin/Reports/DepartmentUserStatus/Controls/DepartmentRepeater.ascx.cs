using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Common.Web.UI;
using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Persistence.Plugin.CMDS;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;

namespace InSite.Custom.CMDS.Reports.Controls
{
    public partial class DepartmentRepeater : BaseUserControl
    {
        #region Classes

        private class RepeaterRow
        {
            public string DepartmentName { get; set; }

            public GridRow[] GridRows { get; set; }
        }

        private class DataGroup : IEquatable<DataGroup>
        {
            public DateTimeOffset AsAt { get; set; }
            public string StatisticType { get; set; }
            public string StatisticName { get; set; }

            public override int GetHashCode()
            {
                var hash = 17;

                hash = hash * 23 + AsAt.GetHashCode();
                hash = hash * 23 + StatisticType.GetHashCode();
                hash = hash * 23 + StatisticName.GetHashCode();

                return hash;
            }

            public override bool Equals(object obj) =>
                Equals(obj as DataGroup);

            public bool Equals(DataGroup other)
            {
                return AsAt == other.AsAt
                    && StatisticType == other.StatisticType
                    && StatisticName == other.StatisticName;
            }
        }

        private class GridRow
        {
            public DateTimeOffset? AsAt { get; set; }
            public string StatisticName { get; set; }

            public int CountCP { get; set; }
            public int CountEX { get; set; }
            public int CountNC { get; set; }
            public int CountNA { get; set; }
            public int CountNT { get; set; }
            public int CountSA { get; set; }
            public int CountSV { get; set; }
            public int CountVA { get; set; }
            public int CountVN { get; set; }
            public int CountRQ { get; set; }
            public int CountOP { get; set; }

            public double? Score { get; set; }
            public double? Progress { get; set; }
        }

        private class XlsxColumnInfo
        {
            public string Name { get; set; }
            public double Width { get; set; }
            public ExcelHorizontalAlignment HorizontalAlignment { get; set; } = ExcelHorizontalAlignment.General;
            public Action<ExcelRange, GridRow> SetupCell { get; set; }
        }

        #endregion

        #region Properties

        private TUserStatusFilter Filter
        {
            get => (TUserStatusFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        #endregion

        #region Fields

        private HashSet<string> _visibleColumns;

        private static readonly XlsxColumnInfo[] _xlsxColumns = new[]
        {
            new XlsxColumnInfo
            {
                Name = "As At",
                Width = 20,
                SetupCell = (range, data) =>
                {
                    range.Value = data.AsAt.HasValue
                        ? (DateTime?)TimeZoneInfo.ConvertTime(data.AsAt.Value, User.TimeZone).DateTime
                        : null;
                    range.Style.Numberformat.Format = "yyyy-MM-dd HH:mm";
                }
            },
            new XlsxColumnInfo
            {
                Name = "Statistic",
                Width = 40,
                SetupCell = (range, data) =>
                {
                    range.Value = data.StatisticName;
                }
            },
            new XlsxColumnInfo
            {
                Name = "CP",
                Width = 8,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = NullIfZero(data.CountCP);
                    range.Style.Numberformat.Format = "#,##0";
                }
            },
            new XlsxColumnInfo
            {
                Name = "EX",
                Width = 8,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = NullIfZero(data.CountEX);
                    range.Style.Numberformat.Format = "#,##0";
                }
            },
            new XlsxColumnInfo
            {
                Name = "NC",
                Width = 8,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = NullIfZero(data.CountNC);
                    range.Style.Numberformat.Format = "#,##0";
                }
            },
            new XlsxColumnInfo
            {
                Name = "NA",
                Width = 8,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = NullIfZero(data.CountNA);
                    range.Style.Numberformat.Format = "#,##0";
                }
            },
            new XlsxColumnInfo
            {
                Name = "NT",
                Width = 8,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = NullIfZero(data.CountNT);
                    range.Style.Numberformat.Format = "#,##0";
                }
            },
            new XlsxColumnInfo
            {
                Name = "SA",
                Width = 8,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = NullIfZero(data.CountSA);
                    range.Style.Numberformat.Format = "#,##0";
                }
            },
            new XlsxColumnInfo
            {
                Name = "SV",
                Width = 8,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value =  NullIfZero(data.CountSV);
                    range.Style.Numberformat.Format = "#,##0";
                }
            },
            new XlsxColumnInfo
            {
                Name = "VA",
                Width = 8,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = NullIfZero(data.CountVA);
                    range.Style.Numberformat.Format = "#,##0";
                }
            },
            new XlsxColumnInfo
            {
                Name = "VN",
                Width = 8,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = NullIfZero(data.CountVN);
                    range.Style.Numberformat.Format = "#,##0";
                }
            },
            new XlsxColumnInfo
            {
                Name = "RQ",
                Width = 8,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = NullIfZero(data.CountRQ);
                    range.Style.Numberformat.Format = "#,##0";
                }
            },
            new XlsxColumnInfo
            {
                Name = "Score",
                Width = 10,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = data.Score;
                    range.Style.Numberformat.Format = "#0%";
                }
            },
            new XlsxColumnInfo
            {
                Name = "Progress",
                Width = 10,
                HorizontalAlignment = ExcelHorizontalAlignment.Right,
                SetupCell = (range, data) =>
                {
                    range.Value = data.Progress;
                    range.Style.Numberformat.Format = "#0%";
                }
            },
        };

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemDataBound += DepartmentRepeater_ItemDataBound;

            DownloadXlsx.Click += DownloadXlsx_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ScriptManager.GetCurrent(Page).RegisterPostBackControl(DownloadXlsx);
        }

        #endregion

        #region Public methods

        public bool LoadData(TUserStatusFilter filter)
        {
            Filter = filter;

            _visibleColumns = Filter.ShowColumns != null
                ? new HashSet<string>(Filter.ShowColumns, StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>();

            var departments = GetData();

            Repeater.DataSource = departments;
            Repeater.DataBind();

            return departments.Length > 0;
        }

        public void Clear()
        {
            Repeater.DataSource = null;
            Repeater.DataBind();
        }

        #endregion

        #region Event handlers

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            var data = GetData();
            if (data.Length == 0)
                return;

            var columns = new List<XlsxColumnInfo>(_xlsxColumns.Length);

            if (Filter.ShowColumns.IsNotEmpty())
            {
                foreach (var c in _xlsxColumns)
                {
                    if (Filter.ShowColumns.Any(x => string.Equals(x, c.Name, StringComparison.OrdinalIgnoreCase)))
                        columns.Add(c);
                }
            }

            if (columns.Count == 0)
                columns.AddRange(_xlsxColumns);

            using (var excel = new ExcelPackage())
            {
                #region Styles Definition

                var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                defaultStyle.Font.Name = "Arial";
                defaultStyle.Font.Size = 10;
                defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;

                var styleTitle = excel.Workbook.Styles.CreateNamedStyle("Department Title");
                styleTitle.Style.Font.Bold = true;
                styleTitle.Style.Font.Size = 14;
                //reportTitleStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //reportTitleStyle.Style.Border.Bottom.Color.SetColor(Color.Black);

                var styleHeader = excel.Workbook.Styles.CreateNamedStyle("Table Header");
                styleHeader.Style.Font.Bold = true;
                styleHeader.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                styleHeader.Style.Border.Bottom.Color.SetColor(Color.Black);

                var styleCell = excel.Workbook.Styles.CreateNamedStyle("Table Cell");
                styleCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                styleCell.Style.Border.Top.Color.SetColor(Color.Silver);
                styleCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                styleCell.Style.Border.Right.Color.SetColor(Color.Silver);
                styleCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                styleCell.Style.Border.Bottom.Color.SetColor(Color.Silver);
                styleCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                styleCell.Style.Border.Left.Color.SetColor(Color.Silver);

                #endregion

                foreach (var group in data)
                {
                    var sheet = excel.Workbook.Worksheets.Add(group.DepartmentName);

                    for (var i = 0; i < columns.Count; i++)
                        sheet.Column(i + 1).Width = columns[i].Width;

                    var rowNumber = 1;

                    {
                        var headerCell = sheet.Cells[rowNumber, 1, rowNumber, columns.Count];
                        headerCell.Merge = true;
                        headerCell.Value = group.DepartmentName;
                        headerCell.StyleName = styleTitle.Name;
                    }

                    sheet.Row(rowNumber++).Height = 22.5;
                    sheet.Row(rowNumber++).Height = 10;

                    for (var i = 0; i < columns.Count; i++)
                    {
                        var column = columns[i];
                        var headerCell = sheet.Cells[rowNumber, i + 1];
                        headerCell.Value = column.Name;
                        headerCell.StyleName = styleHeader.Name;
                        headerCell.Style.HorizontalAlignment = column.HorizontalAlignment;
                    }

                    rowNumber++;

                    foreach (var row in group.GridRows)
                    {
                        for (var i = 0; i < columns.Count; i++)
                            columns[i].SetupCell(sheet.Cells[rowNumber, i + 1], row);

                        rowNumber++;
                    }
                }

                var reportTitle = "Department User Statuses";

                excel.Workbook.Properties.Title = reportTitle;
                excel.Workbook.Properties.Company = Organization.Name;
                excel.Workbook.Properties.Author = User.FullName;
                excel.Workbook.Properties.Created = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, User.TimeZone).DateTime;

                foreach (var sheet in excel.Workbook.Worksheets)
                {
                    sheet.PrinterSettings.Orientation = eOrientation.Portrait;
                    sheet.PrinterSettings.PaperSize = ePaperSize.A4;
                    sheet.PrinterSettings.FitToPage = true;
                    sheet.PrinterSettings.FitToWidth = 1;
                    sheet.PrinterSettings.FitToHeight = 0;
                }

                ReportXlsxHelper.Export(reportTitle + $" {DateTime.UtcNow:yyyyMMdd} {DateTime.UtcNow:HHmmss}", excel);
            }
        }

        private void DepartmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (RepeaterRow)e.Item.DataItem;
            var departmentGrid = (Grid)e.Item.FindControl("DepartmentGrid");

            foreach (DataControlField column in departmentGrid.Columns)
            {
                if (!string.IsNullOrEmpty(column.HeaderText))
                    column.Visible = _visibleColumns.Count == 0 || _visibleColumns.Contains(column.HeaderText);
            }

            departmentGrid.DataSource = item.GridRows;
            departmentGrid.DataBind();
        }

        #endregion

        #region Methods (data binding)

        private RepeaterRow[] GetData()
        {
            var includeAsAt = Filter.ShowColumns.IsEmpty() || Filter.ShowColumns.Any(c => string.Equals(c, "As At", StringComparison.OrdinalIgnoreCase));
            var achievementDisplayMapping = AchievementTypeSelector.CreateAchievementLabelMapping(Organization.Code);
            var data = new TUserStatusSearch().Select(Filter);

            return data.GroupBy(d => d.DepartmentName).Select(depGroup =>
            {
                var department = new RepeaterRow { DepartmentName = depGroup.Key };

                var dataGroup = includeAsAt
                    ? depGroup.GroupBy(row => new DataGroup { AsAt = row.AsAt, StatisticType = row.ListDomain, StatisticName = row.ItemName })
                    : depGroup.GroupBy(row => new DataGroup { AsAt = DateTimeOffset.MinValue, StatisticType = row.ListDomain, StatisticName = row.ItemName });

                department.GridRows = dataGroup
                    .Select(dg =>
                        new GridRow
                        {
                            AsAt = dg.Key.AsAt,
                            StatisticName = achievementDisplayMapping.GetOrDefault(dg.Key.StatisticName, dg.Key.StatisticName),
                            CountCP = dg.Sum(y => y.CountCP),
                            CountEX = dg.Sum(y => y.CountEX),
                            CountNC = dg.Sum(y => y.CountNC),
                            CountNA = dg.Sum(y => y.CountNA),
                            CountNT = dg.Sum(y => y.CountNT),
                            CountSA = dg.Sum(y => y.CountSA),
                            CountSV = dg.Sum(y => y.CountSV),
                            CountVA = dg.Sum(y => y.CountVA),
                            CountVN = dg.Sum(y => y.CountVN),
                            CountRQ = dg.Sum(y => y.CountRQ),
                            Score = dg.All(y => !y.Score.HasValue)
                                ? (double?)null
                                : Math.Floor(100.0 * dg.Average(y => (double?)y.Score ?? 0)) / 100.0,
                            Progress = dg.Sum(y => y.CountRQ) == 0
                                ? (double?)null
                                : Math.Floor(100.0 * (double)dg.Sum(y => y.CountCP + y.CountVA + y.CountVN) / dg.Sum(y => y.CountRQ)) / 100.0
                        })
                    .OrderByDescending(x => x.AsAt).ThenBy(x => x.StatisticName)
                    .ToArray();

                return department;
            }).OrderBy(x => x.DepartmentName).ToArray();
        }

        protected string GetComplianceScore(double? score)
        {
            if (score == null)
                return "NA <span class='text-body-secondary'><i class='fas fa-circle'></i></span>";

            if (score == 1d)
                return $"{score:p0} <span class='text-success'><i class='fas fa-flag-checkered'></i></span>";

            if (0.5d <= score && score < 1.0d)
                return $"{score:p0} <span class='text-danger'><i class='fas fa-flag'></i></span>";

            return $"{score:p0} <span class='text-danger'><i class='far fa-flag'></i></span>";
        }

        protected static string NumberOrEmpty(object obj)
        {
            var intVal = (int)obj;

            return intVal == 0
                ? string.Empty
                : intVal.ToString("n0");
        }

        private static int? NullIfZero(int value)
        {
            return value == 0 ? (int?)null : value;
        }

        #endregion
    }
}