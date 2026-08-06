using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public class DashboardParser
    {
        private string _folder;
        private string[] _dashboards;
        private List<DashboardModel> _models;

        public string Folder => _folder;
        public string[] Dashboards => _dashboards;
        public List<DashboardModel> Models => _models;

        public List<string> Errors { get; set; } = new List<string>();

        public DashboardParser(string folder)
        {
            _folder = folder;
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);
            _dashboards = Directory.GetFiles(_folder, "*.json");

            _models = new List<DashboardModel>();

            foreach (var dashboard in Dashboards)
            {
                var json = File.ReadAllText(dashboard);

                try
                {
                    var model = JsonConvert.DeserializeObject<DashboardModel>(json);

                    model.File = dashboard;

                    foreach (var widget in model.Panels.SelectMany(x => x.Widgets))
                    {
                        ResolveQueries(model, widget);
                        ValidateWidget(model, widget);
                    }

                    _models.Add(model);
                }
                catch (Exception ex)
                {
                    var title = Path.GetFileNameWithoutExtension(dashboard);
                    var error = ex.Message.Replace(Path.Combine(_folder, title), string.Empty);

                    _models.Add(new DashboardModel
                    {
                        File = dashboard,
                        Title = title,
                        Error = error,
                        Panels = new List<DashboardPanel>()
                    });

                    Errors.Add($"Error in {title}: {error}");
                }
            }

            if (_models.Count == 0)
                Errors.Add("There are no dashboards defined for your organization.");
        }

        #region Queries

        private void ResolveQueries(DashboardModel model, DashboardWidget widget)
        {
            ResolveQuery(model, widget.Query);

            if (widget.Chart == null)
                return;

            if ((widget.Chart.Query?.File).IsEmpty())
            {
                widget.Chart.Query = widget.Query;
                return;
            }

            ResolveQuery(model, widget.Chart.Query);
        }

        private void ResolveQuery(DashboardModel model, DashboardQuery query)
        {
            if ((query?.File).IsEmpty())
                return;

            query.File = Path.Combine(_folder, model.Title, query.File);
            query.Sql = File.ReadAllText(query.File);

            if (query.FileRaw.IsEmpty())
                return;

            query.FileRaw = Path.Combine(_folder, model.Title, query.FileRaw);
            query.SqlRaw = File.ReadAllText(query.FileRaw);
        }

        #endregion

        #region Validation

        private void ValidateWidget(DashboardModel model, DashboardWidget widget)
        {
            var name = widget.Title.IfNullOrEmpty(widget.Code);

            if (widget.Type == DashboardWidgetType.None)
            {
                AddError(model, name, $"the widget type is not supported.");
                return;
            }

            if (widget.Type != DashboardWidgetType.Chart && (widget.Query?.Sql).IsEmpty())
                AddError(model, name, "the widget requires a Query section with the correct filename");

            if (widget.Type != DashboardWidgetType.Chart && widget.Type != DashboardWidgetType.ChartTable)
                return;

            if (widget.Chart == null)
            {
                AddError(model, name, "the widget requires a Chart section");
                return;
            }

            if (widget.Chart.Type != DashboardChartType.Bar && widget.Chart.Type != DashboardChartType.Pie)
                AddError(model, name, $"the chart type '{widget.Chart.Type}' is not supported.");

            if ((widget.Chart.Query?.Sql).IsEmpty())
                AddError(model, name, "the chart requires a Query section with the correct filename or a Query section on the widget");

            if (widget.Chart.Label.IsEmpty())
                AddError(model, name, "the chart requires a Label column");

            if (widget.Chart.Datasets.IsEmpty())
                AddError(model, name, "the chart requires at least one dataset");
            else if (widget.Chart.Datasets.Any(x => x.Name.IsEmpty()))
                AddError(model, name, "every chart dataset requires the Name of a value column");
        }

        private void AddError(DashboardModel model, string widgetName, string message)
        {
            var error = $"Error in {model.Title}, widget {widgetName}: {message}.";

            model.Error = model.Error.IsEmpty() ? error : model.Error + " " + error;

            Errors.Add(error);
        }

        #endregion
    }
}
