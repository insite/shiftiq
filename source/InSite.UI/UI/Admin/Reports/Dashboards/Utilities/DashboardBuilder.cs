using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Dashboards.Utilities
{
    public class DashboardBuilder
    {
        private const string ChartIdPostfix = "_Chart";

        private Page _page;

        public List<string> Errors { get; set; } = new List<string>();

        public DashboardBuilder(Page page)
        {
            _page = page;
        }

        #region Create

        public Control CreateWidget(DashboardWidget widget)
        {
            if (widget.Type == DashboardWidgetType.Count)
                return CreateCard(widget.Title, CreateCountControl(widget));

            if (widget.Type == DashboardWidgetType.Table)
                return CreateCard(widget.Title, CreateGridControl(widget));

            if (widget.Type == DashboardWidgetType.Chart)
                return CreateCard(widget.Title, CreateChartControl(widget));

            if (widget.Type == DashboardWidgetType.ChartTable)
                return CreateCard(widget.Title, CreateChartTableControl(widget));

            throw new NotSupportedException($"Unknown widget type: {widget.Type}");
        }

        private Control CreateCountControl(DashboardWidget widget)
        {
            var literal = new Literal();
            literal.ID = widget.Id;

            return literal;
        }

        private Control CreateGridControl(DashboardWidget widget)
        {
            var grid = (DashboardGrid)_page.LoadControl("~/UI/Admin/Reports/Dashboards/Controls/DashboardGrid.ascx");
            grid.ID = widget.Id;

            return grid;
        }

        private Control CreateChartControl(DashboardWidget widget)
        {
            var chart = (DashboardChart)_page.LoadControl("~/UI/Admin/Reports/Dashboards/Controls/DashboardChart.ascx");
            chart.ID = widget.Id + ChartIdPostfix;
            chart.Settings = widget.Chart;

            return chart;
        }

        private Control CreateChartTableControl(DashboardWidget widget)
        {
            var layout = widget.Chart.Layout ?? new DashboardChartLayout();

            var row = new Panel();
            row.CssClass = "row";

            var chartColumn = new Panel();
            var tableColumn = new Panel();

            chartColumn.Controls.Add(CreateChartControl(widget));
            tableColumn.Controls.Add(CreateGridControl(widget));

            if (layout.Position == ChartPosition.Left || layout.Position == ChartPosition.Right)
            {
                chartColumn.CssClass = $"col-lg-{layout.ChartColumns}";
                tableColumn.CssClass = $"col-lg-{layout.TableColumns}";
            }
            else
            {
                chartColumn.CssClass = "col-12";
                tableColumn.CssClass = "col-12";
            }

            var position = layout.Position;
            var chartFirst = position == ChartPosition.Left || position == ChartPosition.Top;

            row.Controls.Add(chartFirst ? chartColumn : tableColumn);
            row.Controls.Add(chartFirst ? tableColumn : chartColumn);

            return row;
        }

        private Control CreateCard(string cardTitle, Control cardBody)
        {
            var card = new Panel();
            card.CssClass = "card border-0 shadow mb-4";

            var body = new Panel();
            body.CssClass = "card-body";

            var title = new Literal();
            title.Text = $"<h5 class='card-title'>{cardTitle}</h5>";

            body.Controls.Add(title);
            body.Controls.Add(cardBody);
            card.Controls.Add(body);

            return card;
        }

        #endregion

        #region Bind

        public void BindWidget(DashboardWidget widget, Control container)
        {
            if (widget.Type == DashboardWidgetType.Count)
                BindCount(widget, (Literal)container.FindControl(widget.Id));

            if (widget.Type == DashboardWidgetType.Table || widget.Type == DashboardWidgetType.ChartTable)
                BindTable(widget, (DashboardGrid)container.FindControl(widget.Id));

            if (widget.Type == DashboardWidgetType.Chart || widget.Type == DashboardWidgetType.ChartTable)
                BindChart(widget, (DashboardChart)container.FindControl(widget.Id + ChartIdPostfix));
        }

        private void BindCount(DashboardWidget widget, Literal literal)
        {
            try
            {
                var sqlParameters = BuildQueryParameters(widget.QueryParameters, widget.Query.Parameters);

                literal.Text = $"<h2>{Persistence.DatabaseHelper.ExecuteCount(widget.Query.Sql, sqlParameters):n0}</h2>";
            }
            catch (Exception ex)
            {
                AddError(widget, ex);
                literal.Text = $"<div class='text-danger'><i class='fas fa-bomb fa-2x'></i></div>";
            }
        }

        private void BindTable(DashboardWidget widget, DashboardGrid grid)
        {
            try
            {
                grid.BindModel(widget);
            }
            catch (Exception ex)
            {
                AddError(widget, ex);
            }
        }

        private void BindChart(DashboardWidget widget, DashboardChart chart)
        {
            try
            {
                chart.BindModel(widget);
            }
            catch (Exception ex)
            {
                AddError(widget, ex);
            }
        }

        private void AddError(DashboardWidget widget, Exception ex)
        {
            Errors.Add($"The query for the widget <strong>{widget.Title}</strong> contains an error. {ex.Message}");
        }

        internal static SqlParameter[] BuildQueryParameters(IDictionary<string, string> widgetParameters, IDictionary<string, string> queryParameters)
        {
            return widgetParameters.EmptyIfNull()
                .Concat(queryParameters.EmptyIfNull().Where(x => !widgetParameters.ContainsKey(x.Key)))
                .Select(x => new SqlParameter(x.Key, x.Value))
                .ToArray();
        }

        #endregion
    }
}
