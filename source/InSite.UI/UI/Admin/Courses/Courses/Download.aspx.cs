using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Courses.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.CourseObjects;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json.Serialization;

using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using IHasParentLinkParameters = Shift.Common.IHasParentLinkParameters;
using IWebRoute = Shift.Common.IWebRoute;

namespace InSite.Admin.Courses.Courses.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private class HierarchyItem
        {
            public Guid Identifier { get; set; }
            public int Asset { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
            public int Depth { get; set; }
            public string AssetPath { get; set; }
            public string Competencies { get; set; }
        }

        private static class FileType
        {
            public const string Outline = "Outline";
            public const string NumberHierarchy = "NumberHierarchy";
        }

        private static class FileFormat
        {
            public static class Json
            {
                public const string Text = "JSON (*.json)";
                public const string Value = "JSON";
            }

            public static class Markdown
            {
                public const string Text = "Markdown (*.txt)";
                public const string Value = "MD";
            }

            public static class Excel
            {
                public const string Text = "Microsoft Excel (*.xlsx)";
                public const string Value = "XLSX";
            }
        }

        private Guid CourseID => Guid.TryParse(Request.QueryString["course"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FileTypeSelector.AutoPostBack = true;
            FileTypeSelector.SelectedIndexChanged += FileTypeSelector_SelectedIndexChanged;

            FileFormatSelector.AutoPostBack = true;
            FileFormatSelector.SelectedIndexChanged += FileFormatSelector_SelectedIndexChanged;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (CourseID == Guid.Empty)
                HttpResponseHelper.Redirect("/ui/admin/courses/search", true);

            var course = CourseSearch.SelectCourse(CourseID);
            if (course == null || course.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect("/ui/admin/courses/search", true);

            PageHelper.AutoBindHeader(this, null, $"{course.CourseName ?? "Untitled"} <span class='form-text'>Course Asset #{course.CourseAsset}</span>");

            CourseDetail.BindCourse(course);

            // FileName.Text = StringHelper.Sanitize(string.Format("{1}-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow, "Course"), '-', true);
            FileName.Text = course.CourseName;

            var resolver = new DefaultContractResolver();
            var contract = (JsonObjectContract)resolver.ResolveContract(typeof(CourseSerialized));

            foreach (var p in contract.Properties.Where(x => !x.Ignored).OrderBy(x => x.PropertyName))
            {
                var item = new ListItem(
                    p.PropertyName.Humanize(LetterCasing.Title),
                    p.PropertyName)
                {
                    Selected = true
                };
                ObjectPropertiesSelector.Items.Add(item);
            }

            SetupFileTypeSelector();

            CancelLink.NavigateUrl = new ReturnUrl().GetReturnUrl()
                ?? $"/ui/admin/courses/manage?course={CourseID}";
        }

        private void SetupFileTypeSelector()
        {
            FileTypeSelector.Items.Clear();
            FileTypeSelector.Items.Add(new ListItem("Course Outline", FileType.Outline));
            FileTypeSelector.Items.Add(new ListItem("Asset Number Hierarchy", FileType.NumberHierarchy));

            FileTypeSelector.SelectedValue = FileType.Outline;
            OnFileTypeChanged();
        }

        private void FileTypeSelector_SelectedIndexChanged(object sender, EventArgs e) => OnFileTypeChanged();

        private void OnFileTypeChanged()
        {
            FileFormatSelector.Items.Clear();

            if (FileTypeSelector.SelectedValue == FileType.Outline)
            {
                FileFormatField.Visible = true;
                FileFormatSelector.Items.Add(new ListItem(FileFormat.Json.Text, FileFormat.Json.Value) { Selected = true });
                FileFormatSelector.Items.Add(new ListItem(FileFormat.Markdown.Text, FileFormat.Markdown.Value));
            }
            else if (FileTypeSelector.SelectedValue == FileType.NumberHierarchy)
            {
                FileFormatField.Visible = true;
                FileFormatSelector.Items.Add(new ListItem(FileFormat.Excel.Text, FileFormat.Excel.Value) { Selected = true });
            }
            else
            {
                FileFormatField.Visible = false;
            }

            OnFileFormatChanged();
        }

        private void FileFormatSelector_SelectedIndexChanged(object sender, EventArgs e) => OnFileFormatChanged();

        private void OnFileFormatChanged()
        {
            JsonPropertiesField.Visible = FileFormatSelector.SelectedValue == FileFormat.Json.Value;
            MarkdownPropertiesField.Visible = FileFormatSelector.SelectedValue == FileFormat.Markdown.Value;
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var course = CourseSearch.SelectCourse(ServiceLocator.RecordSearch, CourseID);

            var fileType = FileTypeSelector.SelectedValue;
            var fileFormat = FileFormatSelector.SelectedValue;

            if (fileType == FileType.Outline)
            {
                if (fileFormat == FileFormat.Json.Value)
                    SendOutlineJson();
                else if (fileFormat == FileFormat.Markdown.Value)
                    SendOutlineMarkdown(course);
            }
            else if (fileType == FileType.NumberHierarchy)
            {
                if (FileFormatSelector.SelectedValue == FileFormat.Excel.Value)
                    SendHierarchyXlsx(course);
            }
        }

        private void SendOutlineJson()
        {
            var props = ObjectPropertiesSelector.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).Distinct().ToArray();
            if (props.Length == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected properties to import.");
                return;
            }

            var resolver = new DownloadContractResolver();
            resolver.AddProperties(typeof(CourseSerialized), props);

            var json = Course2Serializer.Serialize(CourseID, resolver);

            SendFile("json", json);
        }

        private void SendOutlineMarkdown(Course course)
        {
            var publication = new Course2PublicationModel();
            publication.Load(course, User.FullName, "Download", ServiceLocator.ContentSearch);

            var markdown = MarkdownMode.SelectedValue == "Shell"
                ? publication.SerializeAsMarkdownShell()
                : publication.SerializeAsMarkdownOutline();

            var data = Encoding.UTF8.GetBytes(markdown);

            SendFile("txt", data);
        }

        private void SendHierarchyXlsx(Course course)
        {
            var list = CreateHierarchyItem(course);

            var helper = new XlsxExportHelper();

            helper.Map(nameof(HierarchyItem.Asset), "Asset #", 10, HorizontalAlignment.Left);
            helper.Map(nameof(HierarchyItem.AssetPath), "Asset # Path", 30, HorizontalAlignment.Left);
            helper.Map(nameof(HierarchyItem.Identifier), "Identifier", 40, HorizontalAlignment.Left);
            helper.Map(nameof(HierarchyItem.Type), "Type", 15, HorizontalAlignment.Left);
            helper.Map(nameof(HierarchyItem.Name), "Name", 40, HorizontalAlignment.Left);
            helper.Map(nameof(HierarchyItem.Competencies), "Competencies", 80, HorizontalAlignment.Left);

            var data = helper.GetXlsxBytes(list, "Sheet 1");

            SendFile("xlsx", data);
        }

        private List<HierarchyItem> CreateHierarchyItem(Course course)
        {
            var list = new List<HierarchyItem>();

            list.Add(new HierarchyItem
            {
                Identifier = course.Identifier,
                Asset = course.Asset,
                Type = "Course",
                Name = course.Content.Title.GetText(),
                Depth = 0,
                AssetPath = course.Asset.ToString()
            });

            foreach (var unit in course.Units)
            {
                list.Add(new HierarchyItem
                {
                    Identifier = unit.Identifier,
                    Asset = unit.Asset,
                    Type = "Unit",
                    Name = new string(' ', 4) + unit.Content.Title.GetText(),
                    Depth = 1,
                    AssetPath = $"{course.Asset}.{unit.Asset}"
                });

                foreach (var module in unit.Modules)
                {
                    list.Add(new HierarchyItem
                    {
                        Identifier = module.Identifier,
                        Asset = module.Asset,
                        Type = "Module",
                        Name = new string(' ', 4) + module.Content.Title.GetText(),
                        Depth = 1,
                        AssetPath = $"{course.Asset}.{unit.Asset}.{module.Asset}"
                    });

                    foreach (var activity in module.Activities)
                    {
                        list.Add(new HierarchyItem
                        {
                            Identifier = activity.Identifier,
                            Asset = activity.Asset,
                            Type = activity.Type,
                            Name = new string(' ', 8) + activity.Content.Title.GetText(),
                            Depth = 2,
                            AssetPath = $"{course.Asset}.{unit.Asset}.{module.Asset}.{activity.Asset}",
                            Competencies = GetCompetencies(activity.Identifier)
                        });
                    }
                }
            }

            return list;
        }

        Dictionary<Guid, List<VActivityCompetency>> _labels = null;

        Dictionary<Guid, List<VActivityCompetency>> GetLabels(Guid course)
        {
            _labels = new Dictionary<Guid, List<VActivityCompetency>>();

            var competencies = CourseSearch.GetCourseCompetencies(course);

            foreach (var competency in competencies)
            {
                if (!_labels.ContainsKey(competency.ActivityIdentifier))
                    _labels.Add(competency.ActivityIdentifier, new List<VActivityCompetency>());
                _labels[competency.ActivityIdentifier].Add(competency);
            }

            return _labels;
        }

        protected string GetCompetencies(Guid activity)
        {
            if (_labels == null)
                _labels = GetLabels(CourseID);

            if (!_labels.ContainsKey(activity))
                return string.Empty;

            var competencies = _labels[activity];
            if (competencies == null)
                return string.Empty;

            var html = new StringBuilder();
            for (var i = 0; i < competencies.Count; i++)
            {
                var competency = competencies[i];
                html.Append($"{competency.CompetencyLabel ?? competency.CompetencyType} {competency.CompetencyCode ?? competency.CompetencyAsset.ToString()}");
                if (i < competencies.Count - 1)
                    html.Append("; ");
            }
            return html.ToString();
        }

        private void SendFile(string ext, byte[] data)
        {
            if (CompressionMode.Value == "ZIP")
                SendZipFile(data, FileName.Text, ext);
            else
                Response.SendFile(FileName.Text, ext, data);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/manage")
                ? $"course={CourseID}"
                : null;
        }

    }
}
