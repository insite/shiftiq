using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Common.Web.UI.Chart;
using InSite.Persistence;
using InSite.UI.Admin.Reports.Dashboards.Utilities;

using Shift.Common;
using Shift.Common.Colors;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.UI.Admin.Reports.Dashboards
{
    public partial class DashboardChart : BaseUserControl
    {
        #region Properties

        public string DownloadName
        {
            get => (string)ViewState[nameof(DownloadName)];
            set => ViewState[nameof(DownloadName)] = value;
        }

        public DashboardChartSettings Settings
        {
            get => (DashboardChartSettings)ViewState[nameof(Settings)];
            set => ViewState[nameof(Settings)] = value;
        }

        private Dictionary<string, string> WidgetQueryParameters
        {
            get => (Dictionary<string, string>)ViewState[nameof(WidgetQueryParameters)];
            set => ViewState[nameof(WidgetQueryParameters)] = value;
        }

        public bool HasData
        {
            get => (bool?)ViewState[nameof(HasData)] == true;
            set => ViewState[nameof(HasData)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadXlsx.Click += (sender, args) => DownloadXlsxFile();
        }

        #endregion

        #region Binding

        public void BindModel(DashboardWidget widget)
        {
            DownloadName = widget.Title;
            Settings = widget.Chart.Clone();
            WidgetQueryParameters = widget.QueryParameters.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
            
            var chart = CreateChart();
            var table = CreateDataSource();

            HasData = table.Rows.Count > 0;

            NoDataMessage.Visible = !HasData;
            chart.Visible = HasData;
            DownloadXlsx.Visible = widget.AllowDownload && HasData;
            DownloadCount.Visible = widget.AllowDownload && HasData;
            DownloadCount.InnerText = "result".ToQuantity(table.Rows.Count, "N0");

            if (HasData)
                LoadData(chart, table);
        }

        private DataTable CreateDataSource()
        {
            var query = Settings.Query;

            var sqlParameters = DashboardBuilder.BuildQueryParameters(WidgetQueryParameters, query.Parameters);

            return DatabaseHelper.CreateDataTable(query.Sql, sqlParameters);
        }

        private void LoadData(BaseChart chart, DataTable table)
        {
            chart.Data.Clear();

            var palette = Palette.GenerateColorPalette(table.Rows.Count);

            for (var i = 0; i < Settings.Datasets.Length; i++)
            {
                var config = Settings.Datasets[i];
                var values = GetValues(table, config);
                var dataset = chart.Data.CreateDataset(config.Name);

                dataset.Label = config.Label ?? config.Name;

                for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    var item = (IChartDatasetColoredItem)dataset.NewItem();

                    item.Label = GetLabel(table.Rows[rowIndex]);
                    item.Value = values[rowIndex];
                    item.BackgroundColor = GetColor(table, config, palette, rowIndex);
                }
            }
        }

        #endregion

        #region Download

        private void DownloadXlsxFile()
        {
            try
            {
                var table = CreateDataSource();

                LocalizeDates(table);

                var helper = new XlsxExportHelper();

                MapColumns(helper, table);

                var bytes = helper.GetXlsxBytes(table, DownloadName);

                var fileName = StringHelper.Sanitize(DownloadName, '-', false);

                Page.Response.SendFile(fileName, "xlsx", bytes, null, false);
            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith("Thread was being aborted"))
                    ControlStatus.AddMessage(AlertType.Error, ex.Message);
            }
        }

        private void MapColumns(XlsxExportHelper helper, DataTable table)
        {
            var settings = Settings;
            var mapped = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (settings.Label.IsNotEmpty() && table.Columns.Contains(settings.Label))
            {
                helper.Map(settings.Label, settings.Label);
                mapped.Add(settings.Label);
            }

            if (settings.Datasets != null)
            {
                foreach (var config in settings.Datasets)
                {
                    if (config.Name.IsEmpty() || !table.Columns.Contains(config.Name))
                        continue;

                    if (!mapped.Add(config.Name))
                        continue;

                    helper.Map(config.Name, config.Label ?? config.Name);
                }
            }

            if (mapped.Count > 0)
                return;

            foreach (DataColumn column in table.Columns)
                helper.Map(column.ColumnName, column.ColumnName);
        }

        private static void LocalizeDates(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    var value = row[column];
                    if (value is DateTimeOffset d)
                        row[column] = TimeZones.ConvertTime(d, User.TimeZone);
                }
            }
        }

        #endregion

        #region Chart Initialization

        private BaseChart CreateChart()
        {
            BaseChart chart;

            Container.UnloadControl();

            var settings = Settings;

            if (settings.Type == DashboardChartType.Bar)
            {
                var bar = Container.LoadControl<BarChart>();
                bar.MaintainApectRatio = false;

                bar.Options.Scales = new ChartOptionsScales();
                bar.Options.Scales.X.Stacked = settings.Stacked;
                bar.Options.Scales.Y.Stacked = settings.Stacked;
                bar.Options.Scales.Y.BeginAtZero = true;

                SetLegend(settings, x => bar.LegendVisible = x, x => bar.LegendPosition = x);

                chart = bar;
            }
            else if (settings.Type == DashboardChartType.Pie)
            {
                var pie = Container.LoadControl<PieChart>();
                pie.DataType = settings.DataType == DashboardChartDataType.Percent ? ChartDataType.Percent : ChartDataType.Number;
                pie.MaintainAspectRatio = false;

                SetLegend(settings, x => pie.LegendVisible = x, x => pie.LegendPosition = x);

                chart = pie;
            }
            else
                throw ApplicationError.Create("Unknown chart type: {0}", settings.Type.GetName());

            chart.ID = "Chart";
            chart.CssClass = "w-100";

            if (settings.Height.IsNotEmpty())
                chart.Height = Unit.Parse(settings.Height);

            return chart;
        }

        private static void SetLegend(DashboardChartSettings settings, Action<bool> applyVisibility, Action<ChartPosition> applyPosition)
        {
            applyVisibility(settings.Legend == null || settings.Legend.Visible == true);

            if (settings.Legend != null)
                applyPosition(settings.Legend.Position);
        }

        #endregion

        #region Helper Methods

        private string GetLabel(DataRow row)
        {
            var value = row[Settings.Label];

            if (value == DBNull.Value)
                return string.Empty;

            if (value is DateTimeOffset offset)
                return TimeZones.ConvertTime(offset, User.TimeZone).ToString("MMM d, yyyy");

            if (value is DateTime date)
                return date.ToString("MMM d, yyyy");

            return value.ToString();
        }

        private double[] GetValues(DataTable table, DashboardChartDataset config)
        {
            var values = new double[table.Rows.Count];

            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var value = row[config.Name];
                values[i] = value == DBNull.Value ? 0 : Convert.ToDouble(value);
            }

            return values;
        }

        private static Color GetColor(
            DataTable table,
            DashboardChartDataset config,
            IReadOnlyList<Color> palette,
            int rowIndex)
        {
            if (config.Color.IsNotEmpty() && table.Columns.Contains(config.Color))
            {
                var row = table.Rows[rowIndex];
                var value = row[config.Color] as string;
                if (value.IsNotEmpty())
                    return ColorTranslator.FromHtml(value);
            }

            if (config.BackgroundColor.IsNotEmpty())
                return ColorTranslator.FromHtml(config.BackgroundColor);

            if (config.AutoColors)
                return palette[rowIndex % palette.Count];

            return Color.FromArgb(238, 238, 238);
        }

        #endregion
    }
}
