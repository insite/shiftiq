using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Courses.Courses;
using InSite.Application.Courses.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Courses;
using InSite.Domain.Foundations;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.File;
using Shift.Common.Integration.ImageMagick;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.UI.Admin.Assets.Documents.Controls
{
    public partial class Detail : BaseUserControl
    {
        #region Properties

        private bool IsReportTypeVisible
        {
            get => (bool)(ViewState[nameof(IsReportTypeVisible)] ?? false);
            set => ViewState[nameof(IsReportTypeVisible)] = value;
        }

        private string ConvertedMarkdown
        {
            get => (string)(ViewState[nameof(ConvertedMarkdown)] ?? String.Empty);
            set => ViewState[nameof(ConvertedMarkdown)] = value;
        }

        private string WebCacheDirectory
        {
            get => (string)(ViewState[nameof(WebCacheDirectory)] ?? String.Empty);
            set => ViewState[nameof(WebCacheDirectory)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CoursesComboBox.ListFilter.OrganizationIdentifier = Organization.OrganizationIdentifier;

            CoursesComboBox.AutoPostBack = true;
            CoursesComboBox.ValueChanged += CoursesComboBox_ValueChanged;

            UnitsComboBox.AutoPostBack = true;
            UnitsComboBox.ValueChanged += UnitsComboBox_ValueChanged;

            ActivitySaveButton.Click += ActivitySaveButton_Click;

            AddLessonButton.Click += AddLessonButton_Click;

            RadioExtend.AutoPostBack = true;
            RadioExtend.CheckedChanged += CourseSelector_SelectedIndexChanged;

            RadioAdd.AutoPostBack = true;
            RadioAdd.CheckedChanged += CourseSelector_SelectedIndexChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                CourseSelectorChanged();
            }
        }

        #endregion

        #region UI Event Handling

        private void CourseSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            CourseSelectorChanged();
        }

        private void CourseSelectorChanged()
        {
            CoursePanel.Visible = RadioExtend.Checked;
            NewCoursePanel.Visible = RadioAdd.Checked;
        }

        private void FileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null || e.Value.EndsWith(".docx", StringComparison.OrdinalIgnoreCase);
        }

        private void CoursesComboBox_ValueChanged(object sender, EventArgs e)
        {
            if (CoursesComboBox.ValueAsGuid.HasValue)
            {
                UnitsComboBox.CourseID = CoursesComboBox.ValueAsGuid.Value;
                UnitsComboBox.RefreshData();
            }
            else
            {
                UnitsComboBox.CourseID = Guid.Empty;
                UnitsComboBox.RefreshData();

                ModulesComboBox.UnitID = Guid.Empty;
                ModulesComboBox.RefreshData();
            }

            MarkdownText.LoadData(ConvertedMarkdown);
        }

        private void UnitsComboBox_ValueChanged(object sender, EventArgs e)
        {
            if (UnitsComboBox.ValueAsGuid.HasValue)
            {
                ModulesComboBox.UnitID = UnitsComboBox.ValueAsGuid.Value;
                ModulesComboBox.RefreshData();
            }
            else
            {
                ModulesComboBox.UnitID = Guid.Empty;
                ModulesComboBox.RefreshData();
            }

            MarkdownText.LoadData(ConvertedMarkdown);
        }

        private void ActivitySaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (DocumentUpload.HasFile)
                    SaveUploadedFiles(DocumentUpload.Metadata);
            }
            catch (PandocConverterException pex)
            {
                StatusAlert.AddMessage(AlertType.Error, $"Error during conversion: {pex.Message}");
            }
            catch (Exception ex)
            {
                StatusAlert.AddMessage(AlertType.Error, "Error during conversion: " + ex.Message);
            }
        }

        private void AddLessonButton_Click(object sender, EventArgs e)
        {
            if (RadioExtend.Checked)
            {
                ExtendCourseShell();
            }
            else if (RadioAdd.Checked)
            {
                CreateCourseShell(CourseName.Text);
            }

        }

        #endregion

        #region Save (Data operations)

        private void CreateCourseShell(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                StatusAlert.AddMessage(AlertType.Error, "Name of the new course should not be empty: ");
                return;
            }

            var courseId = UniqueIdentifier.Create();

            InsertMarkdownData(courseId);

            var activityId = InsertCourse(courseId, name);

            HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={courseId}&activity={activityId}", true);
        }

        private void ExtendCourseShell()
        {
            try
            {
                if (CoursesComboBox.ValueAsGuid.HasValue &&
                    UnitsComboBox.ValueAsGuid.HasValue &&
                    ModulesComboBox.ValueAsGuid.HasValue &&
                    !string.IsNullOrEmpty(ConvertedMarkdown))
                {
                    var unit = CourseSearch.SelectUnit(UnitsComboBox.ValueAsGuid.Value);
                    var module = CourseSearch.SelectModule(ModulesComboBox.ValueAsGuid.Value);
                    var course = CourseSearch.SelectCourse(CoursesComboBox.ValueAsGuid.Value);

                    InsertMarkdownData(course.CourseIdentifier);

                    var commands = new List<ICommand>();
                    var activityId = CreateActivity(course.CourseIdentifier, module.ModuleIdentifier, commands);

                    ServiceLocator.SendCommand(new RunCommands(course.CourseIdentifier, commands.ToArray()));

                    DomainCache.Instance.RemoveCourse(course.CourseIdentifier);

                    HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={course.CourseIdentifier}&activity={activityId}", true);
                }
            }
            catch (Exception ex)
            {
                StatusAlert.AddMessage(AlertType.Error, "Error during adding lesson: " + ex.Message);
            }
        }

        private Guid InsertCourse(Guid courseId, string name)
        {
            var commands = new List<ICommand>();

            var course = EntityHelper.CreateCourse(name);
            course.CourseIdentifier = courseId;

            CourseCommandCreator.Create(null, null, course, GetContent(course.CourseName), commands);

            var unit = EntityHelper.CreateUnit(course.CourseIdentifier, "Unit 1");
            UnitCommandCreator.Create(null, null, unit, GetContent(unit.UnitName), commands);

            var module = EntityHelper.CreateModule(unit.UnitIdentifier, "Module 1");
            ModuleCommandCreator.Create(course.CourseIdentifier, null, null, module, GetContent(module.ModuleName), commands);

            var activityId = CreateActivity(course.CourseIdentifier, module.ModuleIdentifier, commands);

            ServiceLocator.SendCommand(new RunCommands(course.CourseIdentifier, commands.ToArray()));

            DomainCache.Instance.RemoveCourse(course.CourseIdentifier);

            return activityId;
        }

        private static ContentContainer GetContent(string title)
        {
            var content = new ContentContainer();
            content.Title.Text.Default = title;
            return content;
        }

        public Guid CreateActivity(Guid courseId, Guid moduleId, List<ICommand> commands)
        {
            var activity = EntityHelper.CreateActivity(moduleId, ActivityType.Lesson, "Lesson from WORD document");

            var content = GetContent(activity.ActivityName);

            using (StreamReader sr = new StreamReader(ConvertedMarkdown))
                content.Body.Text = new MultilingualString() { Default = sr.ReadToEnd() };

            ActivityCommandCreator.Create(courseId, null, null, activity, GetContent(activity.ActivityName), commands);

            return activity.ActivityIdentifier;
        }

        private string SaveUploadedFiles(UploadMetadata upload)
        {
            if (upload == null)
                return null;

            var relativeUrl = UploadHelper.SaveUploadedFiles(ServiceLocator.FilePaths, upload, "assets");
            var settings = new PandocConverterSettings(
                ServiceLocator.AppSettings.Application.PandocExePath,
                GetFilePhysicalPath(upload.FileName),
                PandocConverterSettings.PanDocType.Markdown,
                true);

            ConvertedMarkdown = PandocFileConverter.Convert(settings);
            WebCacheDirectory = settings.WebCacheDirectory;
            MarkdownText.LoadData(ConvertedMarkdown);

            if (MarkdownText.Text.Length > 0)
                CoursesPanel.Visible = true;

            MarkdownPanel.IsSelected = true;

            return relativeUrl;
        }

        #endregion

        #region Helper Functions

        private void InsertMarkdownData(Guid courseId)
        {
            UpdateImageURL(ConvertedMarkdown, courseId.ToString(), WebCacheDirectory);
            PreprocessImages(ConvertedMarkdown, courseId);
        }

        private void PreprocessImages(string uploadMarkdownFilePath, Guid course)
        {
            var storageFolderPath = GetFilePhysicalFolderPath(course);

            string uploadFolderPath =
                Path.Combine(
                    Path.GetDirectoryName(uploadMarkdownFilePath),
                    Path.GetFileNameWithoutExtension(uploadMarkdownFilePath),
                    "media");

            if (Directory.Exists(uploadFolderPath))
            {
                var imgPhysicalList = System.IO.Directory.GetFiles(uploadFolderPath);

                foreach (var img in imgPhysicalList)
                {
                    PreprocessImage(img, storageFolderPath);
                }
            }
        }

        private bool PreprocessImage(string uploadPath, string storagePath)
        {
            try
            {
                var messages = new List<string>();
                using (var input = File.Open(uploadPath, FileMode.Open, FileAccess.Read))
                using (var output = File.Open(Path.Combine(storagePath, Path.GetFileName(uploadPath)), FileMode.Create, FileAccess.Write))
                    ImageHelper.AdjustImage(input, output, ImageType.Jpeg, false, messages);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static string GetFilePhysicalFolderPath(Guid course)
        {
            var folder = ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Tenants", Organization.Code, "Courses", course.ToString());
            return folder;
        }

        private static string GetFilePhysicalPath(string fileName)
        {
            var folder = ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Tenants", Organization.Code, "Assets");
            return Path.Combine(folder, fileName);
        }

        private void UpdateImageURL(string filePath, string courseId, string pattern)
        {
            var input = String.Empty;
            using (StreamReader sr = new StreamReader(filePath))
            {
                input = sr.ReadToEnd();
            }
            var output = ConvertImageURL(input, courseId, pattern);
            if (!string.IsNullOrEmpty(output))
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    writer.Write(output);
                }
            }
        }

        private string ConvertImageURL(string input, string courseId, string pattern)
        {
            pattern = pattern + "/media";
            return input.Replace(pattern, $"/in-content/courses/{courseId}");
        }

        #endregion
    }
}