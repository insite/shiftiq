using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Dashboards
{
    public partial class DashboardContainer : BaseUserControl
    {
        private DashboardParser _parser;
        private DashboardModel _model;
        private DashboardBuilder _builder;

        private string _folderPath;
        private int _selectedIndex;

        public bool HasModel => _model != null;

        public bool HasErrors => _parser.Errors.Count > 0;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DashboardList.SelectedIndexChanged += DashboardList_SelectedIndexChanged;

            _folderPath = ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Organizations", Organization.Identifier.ToString(), "Reports", "Dashboards");
            _selectedIndex = int.TryParse(Request.QueryString["d"], out int n) ? n : 0;
            _parser = new DashboardParser(_folderPath);

            if (_selectedIndex >= 0 && _selectedIndex < _parser.Models.Count)
                _model = _parser.Models[_selectedIndex];

            if (!HasErrors && _model != null)
                CreateDashboardPanels();

            ShowErrorMessages();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls();
        }

        private void DashboardList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect("/ui/admin/reports/dashboards?d=" + DashboardList.SelectedIndex);
        }

        private void CreateDashboardPanels()
        {
            _builder = new DashboardBuilder(Page);

            foreach (var panel in _model.Panels)
            {
                var container = new Panel();
                container.CssClass = $"col-lg-{panel.Size}";
                foreach (var widget in panel.Widgets)
                    container.Controls.Add(_builder.CreateWidget(widget));
                DashboardPanels.Controls.Add(container);
            }
        }

        private void ShowErrorMessages()
        {
            foreach (var error in _parser.Errors)
            {
                DashboardStatus.AddMessage(AlertType.Error, error);
                DashboardPanels.Visible = false;
            }

            if (_model == null)
                DashboardStatus.AddMessage(AlertType.Error, $"The dashboard panel d={_selectedIndex} does not exist");
        }

        private void BindModelToControls()
        {
            DashboardList.SelectedIndex = _selectedIndex;

            DashboardList.DataValueField = "File";
            DashboardList.DataTextField = "Title";
            DashboardList.DataSource = _parser.Models;
            DashboardList.DataBind();

            if (HasErrors)
                return;

            if (_model != null)
                foreach (var panel in _model.Panels)
                    foreach (var widget in panel.Widgets)
                    {
                        widget.Query.Parameters.Add("@OrganizationIdentifier", Organization.Identifier.ToString());
                        widget.Query.Parameters.Add("@UserIdentifier", User.Identifier.ToString());
                        _builder.BindWidget(widget, DashboardPanels.FindControl(widget.Id));
                    }

            if (_builder != null)
                foreach (var error in _builder.Errors)
                    DashboardStatus.AddMessage(AlertType.Error, error);
        }

        public void DownloadModel()
        {
            var fileName = FileHelper.AdjustFileName(_model.Title);

            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    archive.CreateEntryFromFile(_model.File, Path.GetFileName(_model.File), CompressionLevel.Optimal);

                    foreach (var panel in _model.Panels)
                    {
                        foreach (var widget in panel.Widgets)
                        {
                            archive.CreateEntryFromFile(widget.Query.File, Path.GetFileName(widget.Query.File), CompressionLevel.Optimal);
                        }
                    }
                }

                Page.Response.SendFile(fileName, "zip", archiveStream.ToArray());
            }
        }

        public int UploadModel(Stream fileStream)
        {
            string title;

            using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry jsonFile;
                {
                    var jsonFiles = archive.Entries.Where(x => x.Name.EndsWith(".json")).ToArray();
                    if (jsonFiles.Length == 0)
                        throw ApplicationError.Create("The archive is missing a JSON file");

                    if (jsonFiles.Length > 1)
                        throw ApplicationError.Create("The archive should contain only one JSON file");

                    jsonFile = jsonFiles[0];
                }

                var sqlFiles = archive.Entries.Where(x => x.Name.EndsWith(".sql")).ToArray();
                if (sqlFiles.Length == 0)
                    throw ApplicationError.Create("The archive is missing the SQL files");

                var jsonFilePath = Path.Combine(_folderPath, jsonFile.Name);
                if (File.Exists(jsonFilePath))
                    throw ApplicationError.Create($"The file with the name '{jsonFile.Name}' already exist.");

                title = Path.GetFileNameWithoutExtension(jsonFile.Name);

                var sqlFolderPath = Path.Combine(_folderPath, title);
                if (Directory.Exists(sqlFolderPath))
                    Directory.Delete(sqlFolderPath, true);

                Directory.CreateDirectory(sqlFolderPath);

                jsonFile.ExtractToFile(jsonFilePath);

                foreach (var sqlFile in sqlFiles)
                    sqlFile.ExtractToFile(sqlFolderPath + sqlFile.Name);
            }

            var parser = new DashboardParser(_folderPath);

            if (parser.Errors.Count == 0)
            {
                for (var i = 0; i < parser.Models.Count; i++)
                {
                    var model = parser.Models[i];
                    if (title.Equals(model.Title, StringComparison.OrdinalIgnoreCase))
                        return i;
                }
            }

            return -1;
        }

        public void DeleteModel()
        {
            if (File.Exists(_model.File))
                File.Delete(_model.File);

            var folderPath = Path.Combine(_folderPath, Path.GetFileNameWithoutExtension(_model.File));
            if (Directory.Exists(folderPath))
                Directory.Delete(folderPath, true);
        }
    }
}