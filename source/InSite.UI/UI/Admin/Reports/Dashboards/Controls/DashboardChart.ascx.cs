using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Common.Web.UI.Chart;
using InSite.Persistence;
using InSite.UI.Admin.Reports.Dashboards.Utilities;

using Shift.Common;
using Shift.Common.Colors;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Dashboards
{
    public partial class DashboardChart : BaseUserControl
    {
        #region Properties

        public DashboardChartSettings Settings { get; set; }

        public bool HasData
        {
            get => (bool?)ViewState[nameof(HasData)] == true;
            set => ViewState[nameof(HasData)] = value;
        }

        #endregion

        #region Binding

        public void BindModel(DashboardWidget widget)
        {
            Settings = widget.Chart;

            var chart = CreateChart(widget.Chart);
            var table = CreateDataSource(widget);

            HasData = table.Rows.Count > 0;

            NoDataMessage.Visible = !HasData;
            chart.Visible = HasData;

            if (HasData)
                LoadData(chart, table);
        }

        private static DataTable CreateDataSource(DashboardWidget widget)
        {
            var sqlParameters = DashboardBuilder.BuildQueryParameters(widget.QueryParameters, widget.Chart.Query.Parameters);

            return DatabaseHelper.CreateDataTable(widget.Chart.Query.Sql, sqlParameters);
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

        #region Chart Initialization

        private BaseChart CreateChart(DashboardChartSettings settings)
        {
            BaseChart chart;

            Container.UnloadControl();

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
                return TimeZones.ConvertFromUtc(offset, User.TimeZone).ToString("MMM d, yyyy");

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
