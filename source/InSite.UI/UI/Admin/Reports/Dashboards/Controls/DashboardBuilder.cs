using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Dashboards
{
    public class DashboardBuilder
    {
        private Page _page;

        public List<string> Errors { get; set; } = new List<string>();

        public DashboardBuilder(Page page)
        {
            _page = page;
        }

        public Control CreateWidget(DashboardWidget widget)
        {
            if (widget.Type == "Count")
                return CreateCountWidgetHtml(widget);

            if (widget.Type == "Table")
                return CreateTableWidgetHtml(widget);

            throw new Exception("Unknown widget type");
        }

        public void BindWidget(DashboardWidget widget, Control control)
        {
            if (widget.Type == "Count")
                BindCountWidgetHtml(widget, (Literal)control);
            else if (widget.Type == "Table")
                BindTableWidgetHtml(widget, (DashboardGrid)control);
        }

        private Control CreateCountWidgetHtml(DashboardWidget widget)
        {
            var literal = new Literal();
            literal.ID = widget.Id;

            return CreateCard(widget.Title, literal);
        }

        private Control CreateTableWidgetHtml(DashboardWidget widget)
        {
            var grid = (DashboardGrid)_page.LoadControl("~/UI/Admin/Reports/Dashboards/Controls/DashboardGrid.ascx");
            grid.ID = widget.Id;
            return CreateCard(widget.Title, grid);
        }

        private void BindCountWidgetHtml(DashboardWidget widget, Literal literal)
        {
            try
            {
                var sqlParameters = new List<SqlParameter>();
                foreach (var parameter in widget.Query.Parameters)
                    sqlParameters.Add(new SqlParameter(parameter.Key, parameter.Value));

                literal.Text = $"<h2>{Persistence.DatabaseHelper.ExecuteCount(widget.Query.Sql, sqlParameters.ToArray()):n0}</h2>";
            }
            catch (Exception ex)
            {
                Errors.Add($"The query for the widget <strong>{widget.Title}</strong> contains an error. {ex.Message}");
                literal.Text = $"<div class='text-danger'><i class='fas fa-bomb fa-2x'></i></div>";
            }
        }

        private void BindTableWidgetHtml(DashboardWidget widget, DashboardGrid grid)
        {
            try
            {
                grid.BindModel(widget.Query);
            }
            catch (Exception ex)
            {
                Errors.Add($"The query for the widget <strong>{widget.Title}</strong> contains an error. {ex.Message}");
            }
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
    }
}