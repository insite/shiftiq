using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

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

                    foreach (var panel in model.Panels)
                        foreach (var widget in panel.Widgets)
                        {
                            if (widget.Query.File != null)
                            {
                                widget.Query.File = Path.Combine(_folder, model.Title, widget.Query.File);
                                widget.Query.Sql = File.ReadAllText(widget.Query.File);

                                if (widget.Query.FileRaw != null)
                                {
                                    widget.Query.FileRaw = Path.Combine(_folder, model.Title, widget.Query.FileRaw);
                                    widget.Query.SqlRaw = File.ReadAllText(widget.Query.FileRaw);
                                }
                            }
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
    }
}