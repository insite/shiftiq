using System;
using System.Collections.Generic;
using System.IO;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Courses;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.File;
using Shift.Constant;

namespace InSite.Admin.Courses.Courses
{
    public partial class Create : AdminBasePage
    {
        private class MarkdownLine
        {
            public int Level { get; }
            public string Name { get; }
            public string Type { get; }

            public MarkdownLine(int level, string name, string type)
            {
                Level = level;
                Name = name;
                Type = type;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += (x, y) => CreationTypeSelected();

            UploadFileType.AutoPostBack = true;
            UploadFileType.ValueChanged += (x, y) => UploadFileTypeSelected();

            CreateButton.Click += (x, y) => CreateCourse(CourseName.Text);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Upload);

            CancelButton.NavigateUrl = "/ui/admin/courses/search";
        }

        private void CreationTypeSelected()
        {
            if (CreationType.ValueAsEnum == CreationTypeEnum.Upload)
                CreateMultiView.ActiveViewIndex = 1;
            else
                CreateMultiView.ActiveViewIndex = 0;
        }

        private void UploadFileTypeSelected()
        {
            CreateUploadFile.LabelText = UploadFileType.Value == "Markdown"
                ? "Select and Upload JSON File"
                : "Select and Upload MD File";
        }

        private void CreateCourse(string name)
        {
            try
            {
                var id = CreationType.ValueAsEnum == CreationTypeEnum.Upload
                    ? UploadCourseShell(GetFilePath(CreateUploadFile.Metadata))
                    : CreateCourseShell(name);

                string GetFilePath(UploadMetadata metadata)
                {
                    if (metadata == null)
                        throw new ArgumentNullException(nameof(metadata));

                    if (string.IsNullOrEmpty(metadata.FilePath))
                        throw new ArgumentNullException(nameof(metadata));

                    return metadata.FilePath;
                }

                RedirectToOutline(id);
            }
            catch (ArgumentNullException)
            {
                EditorStatus.AddMessage(AlertType.Error, "Please click the upload button prior to saving.");
            }
            catch (Exception ex)
            {
                EditorStatus.AddMessage(AlertType.Error, ex.Message);
            }
        }

        private Guid CreateCourseShell(string name)
        {
            var commands = new List<ICommand>();

            var course = EntityHelper.CreateCourse(name);
            CourseCommandCreator.Create(null, null, course, GetContent(course.CourseName), commands);

            var unit = EntityHelper.CreateUnit(course.CourseIdentifier, "Unit 1");
            UnitCommandCreator.Create(null, null, unit, GetContent(unit.UnitName), commands);

            var module = EntityHelper.CreateModule(unit.UnitIdentifier, "Module 1");
            ModuleCommandCreator.Create(course.CourseIdentifier, null, null, module, GetContent(module.ModuleName), commands);

            var lesson = EntityHelper.CreateActivity(module.ModuleIdentifier, ActivityType.Lesson, "Lesson 1");
            ActivityCommandCreator.Create(course.CourseIdentifier, null, null, lesson, GetContent(lesson.ActivityName), commands);

            ServiceLocator.SendCommand(new RunCommands(course.CourseIdentifier, commands.ToArray()));

            return course.CourseIdentifier;
        }

        private static ContentContainer GetContent(string title)
        {
            var content = new ContentContainer();
            content.Title.Text.Default = title;
            return content;
        }

        private Guid UploadCourseShell(string path)
        {
            return UploadFileType.Value == "JSON"
                ? UploadJson(path)
                : UploadMarkdown(path);
        }

        private Guid UploadJson(string path)
        {
            var result = new Course2Deserializer().Deserialize(path);

            Course2Store.InsertCourse(
                result.Course,
                result.Units,
                result.Modules,
                result.Activities,
                result.Competencies,
                result.Prerequisites,
                result.Contents
            );

            ServiceLocator.ContentStore.InsertPrivacyGroups(result.PrivacyGroups);

            if (result.Warnings != null && result.Warnings.Count > 0)
                InSite.UI.Admin.Courses.Manage.Warnings = result.Warnings;

            return result.Course.CourseIdentifier;
        }

        private Guid UploadMarkdown(string path)
        {
            QCourse course = null;
            QUnit unit = null;
            QModule module = null;
            var lineNumber = 1;

            var units = new List<QUnit>();
            var modules = new List<QModule>();
            var activities = new List<QActivity>();
            var contents = new List<TContent>();

            using (var stream = new StreamReader(path))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    var markdownLine = ParseMarkdownLine(line, lineNumber);
                    if (markdownLine != null)
                    {
                        switch (markdownLine.Level)
                        {
                            case 1:
                                if (course != null)
                                    throw new ArgumentException($"Multiple courses are not supported. The error in line {lineNumber}");
                                course = AddMarkdownCourse(markdownLine, contents);
                                break;
                            case 2:
                                unit = AddMarkdownUnit(lineNumber, markdownLine, course?.CourseIdentifier, units, contents);
                                module = null;
                                break;
                            case 3:
                                module = AddMarkdownModule(lineNumber, markdownLine, unit?.UnitIdentifier, modules, contents);
                                break;
                            case 4:
                                AddMarkdownActivity(lineNumber, markdownLine, module?.ModuleIdentifier, activities, contents);
                                break;
                        }
                    }

                    lineNumber++;
                }
            }

            Course2Store.InsertCourse(course, units, modules, activities, null, null, contents);

            return course.CourseIdentifier;
        }

        private void AddMarkdownActivity(int lineNumber, MarkdownLine markdownLine, Guid? moduleIdentifier, List<QActivity> activities, List<TContent> contents)
        {
            if (moduleIdentifier == null)
                throw new ArgumentException($"The module should be specified first. The error in line {lineNumber}");

            var activity = new QActivity
            {
                ActivityAsset = Sequence.Increment(Organization.Identifier, SequenceType.Asset),
                ActivityIdentifier = UniqueIdentifier.Create(),
                ActivityName = markdownLine.Name,
                ActivityType = markdownLine.Type ?? "Lesson",
                ModuleIdentifier = moduleIdentifier.Value,
                ActivityAuthorDate = DateTime.Today,
                ActivityAuthorName = User.FullName,
            };

            activities.Add(activity);

            contents.Add(CreateMarkdownContent(activity.ActivityIdentifier, "Activity", activity.ActivityName));
        }

        private QModule AddMarkdownModule(int lineNumber, MarkdownLine markdownLine, Guid? unitIdentifier, List<QModule> modules, List<TContent> contents)
        {
            if (unitIdentifier == null)
                throw new ArgumentException($"The unit should be specified first. The error in line {lineNumber}");

            var module = new QModule
            {
                UnitIdentifier = unitIdentifier.Value,
                ModuleAsset = Sequence.Increment(Organization.Identifier, SequenceType.Asset),
                ModuleIdentifier = UniqueIdentifier.Create(),
                ModuleName = markdownLine.Name,
            };

            modules.Add(module);
            contents.Add(CreateMarkdownContent(module.ModuleIdentifier, "Module", module.ModuleName));

            return module;
        }

        private QUnit AddMarkdownUnit(int lineNumber, MarkdownLine markdownLine, Guid? courseIdentifier, List<QUnit> units, List<TContent> contents)
        {
            if (courseIdentifier == null)
                throw new ArgumentException($"The course should be specified first. The error in line {lineNumber}");

            var unit = new QUnit
            {
                CourseIdentifier = courseIdentifier.Value,
                UnitAsset = Sequence.Increment(Organization.Identifier, SequenceType.Asset),
                UnitIdentifier = UniqueIdentifier.Create(),
                UnitName = markdownLine.Name,
            };

            units.Add(unit);

            contents.Add(CreateMarkdownContent(unit.UnitIdentifier, "Unit", unit.UnitName));

            return unit;
        }

        private QCourse AddMarkdownCourse(MarkdownLine markdownLine, List<TContent> contents)
        {
            var course = new QCourse
            {
                CourseAsset = Sequence.Increment(Organization.Identifier, SequenceType.Asset),
                CourseIdentifier = UniqueIdentifier.Create(),
                CourseName = markdownLine.Name,
                OrganizationIdentifier = Organization.Identifier,
                IsMultipleUnitsEnabled = true,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = User.UserIdentifier,
                Modified = DateTimeOffset.UtcNow,
                ModifiedBy = User.UserIdentifier
            };

            contents.Add(CreateMarkdownContent(course.CourseIdentifier, "Course", course.CourseName));

            return course;
        }

        private TContent CreateMarkdownContent(Guid containerIdentifier, string containerType, string text)
        {
            return new TContent
            {
                OrganizationIdentifier = Organization.Identifier,
                ContainerIdentifier = containerIdentifier,
                ContentIdentifier = UniqueIdentifier.Create(),
                ContentLabel = "Title",
                ContentLanguage = "en",
                ContainerType = containerType,
                ContentText = text
            };
        }

        private static MarkdownLine ParseMarkdownLine(string line, int lineNumber)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            line = line.Trim();

            if (lineNumber == 1 && line.StartsWith("Tiers:", StringComparison.OrdinalIgnoreCase))
                return null;

            if (!line.StartsWith("#"))
                throw new ArgumentException($"Expected starting symbol '#' in line {lineNumber}");

            int spaceIndex = line.IndexOf(' ');
            if (spaceIndex > 4)
                throw new ArgumentException($"The number of symbols '#' is incorrect in line {lineNumber}");

            var text = spaceIndex > 0 ? line.Substring(spaceIndex + 1).Trim() : null;
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException($"The name is empty in line {lineNumber}");

            string type = null;
            if (spaceIndex == 4 && text.EndsWith("]"))
            {
                var typeIndex = text.LastIndexOf('[');
                if (typeIndex > 0)
                {
                    type = text.Substring(typeIndex + 1, text.Length - typeIndex - 2).Trim().ToLower();
                    if (type.Length > 0)
                    {
                        type = type.Substring(0, 1).ToUpper() + type.Substring(1);
                        ValidateActivityType(type, lineNumber);
                    }
                    else
                        type = null;

                    text = text.Substring(0, typeIndex).Trim();
                }
            }

            return new MarkdownLine(spaceIndex, text, type);
        }

        private static void ValidateActivityType(string type, int lineNumber)
        {
            switch (type)
            {
                case "Lesson":
                case "Assessment":
                case "Video":
                case "Link":
                case "Survey":
                    return;
            }

            throw new ArgumentException($"The activity type '{type}' is not supported. The error in line {lineNumber}");
        }

        private void RedirectToOutline(Guid course)
            => HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={course}", true);
    }
}
