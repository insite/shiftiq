using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Courses.Courses;
using InSite.Admin.Courses.Outlines;
using InSite.Admin.Courses.Outlines.Controls;
using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Common.Web;
using InSite.Domain.Courses;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Courses
{
    public partial class Manage : AdminBasePage
    {
        #region Classes

        private class EditorInfo
        {
            public string Type { get; }
            public string ControlPath { get; }

            public EditorInfo(string type, string controlPath)
            {
                Type = type;
                ControlPath = controlPath;
            }
        }

        #endregion

        #region Constants

        private static readonly Dictionary<string, EditorInfo> EditorInfoByType = new Dictionary<string, EditorInfo>(StringComparer.OrdinalIgnoreCase)
        {
            { "lesson", new EditorInfo("lesson", "~/UI/Admin/Courses/Outlines/Controls/ActivityEditLesson.ascx") },
            { "assessment", new EditorInfo("assessment", "~/UI/Admin/Courses/Outlines/Controls/ActivityEditAssessment.ascx") },
            { "survey", new EditorInfo("survey", "~/UI/Admin/Courses/Outlines/Controls/ActivityEditForm.ascx") },
            { "document", new EditorInfo("document", "~/UI/Admin/Courses/Outlines/Controls/ActivityEditDocument.ascx") },
            { "link", new EditorInfo("link", "~/UI/Admin/Courses/Outlines/Controls/ActivityEditLink.ascx") },
            { "video", new EditorInfo("video", "~/UI/Admin/Courses/Outlines/Controls/ActivityEditVideo.ascx") },
            { "quiz", new EditorInfo("quiz", "~/UI/Admin/Courses/Outlines/Controls/ActivityEditQuiz.ascx") },
        };

        private static readonly string WarningsSessionKey = typeof(Manage).FullName + "." + nameof(Warnings);

        #endregion

        #region Properties

        public static List<string> Warnings
        {
            get => (List<string>)HttpContext.Current.Session[WarningsSessionKey];
            set => HttpContext.Current.Session[WarningsSessionKey] = value;
        }

        #endregion

        #region Fields

        private UrlParser.Url _url;
        private OutlineModel _model;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ValidateUrl();
            InitModel();

            if (_model.Module == null && _model.Course != null)
                HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={_model.Course.Identifier}");
            else if (_model.Course == null)
                HttpResponseHelper.Redirect($"/ui/admin/courses/search");

            if (_url.Exists("create-unit"))
                ActivityPlaceHolder.Controls.Add(LoadActivityCreator(null, null, _url.Get("create-unit")));
            else if (_url.Exists("create-module"))
                ActivityPlaceHolder.Controls.Add(LoadActivityCreator(_model.Unit.UnitIdentifier, null, _url.Get("create-module")));
            else if (_url.Exists("create-activity"))
                ActivityPlaceHolder.Controls.Add(LoadActivityCreator(_model.Unit.UnitIdentifier, _url.GetGuid("module"), _url.Get("create-activity")));
            else
                ActivityPlaceHolder.Controls.Add(LoadActivityEditor(_model.Activity));

            SelectPanel(_url.Get("panel"));
        }

        private void SelectPanel(string panel)
        {
            ActivityPanel.IsSelected = panel == "activity";
            ModulePanel.IsSelected = panel == "module";
            UnitPanel.IsSelected = panel == "unit";
            NotificationPanel.IsSelected = panel == "notification";
            CoursePanel.IsSelected = panel == "course";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls();

            if (Warnings != null && Warnings.Count > 0)
            {
                var warningMessage = string.Join("", Warnings.Select(x => $"<li>{x}</li>"));
                ScreenStatus.AddMessage(AlertType.Warning, "Warnings during upload:<br><ul>" + warningMessage + "</ul>");

                Warnings = null;
            }
        }

        /// <summary>
        /// A course must have at least one module that contains at least one activity, and this must be requested in the URL. If the URL is missing
        /// an activity identifier, then this method "selects" the first activity as the default. The course contains no activities, then it creates
        /// an empty lesson and "selects" it as the default.
        /// </summary>
        private void ValidateUrl()
        {
            var parser = new UrlParser();
            _url = parser.Parse(Request.RawUrl);

            var course = _url.GetGuid("course");
            if (course == null || !CourseSearch.CourseExists(x => x.CourseIdentifier == course))
                HttpResponseHelper.Redirect("/ui/admin/courses/search");

            var activity = _url.GetGuid("activity");
            if (activity != null)
                return;

            activity = CourseSearch.GetFirstCourseActivity(course.Value);
            if (activity != null)
                HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={course}&activity={activity}");

            var activityId = CreateLesson(course.Value);

            HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={course}&activity={activityId}");
        }

        private Guid CreateLesson(Guid courseId)
        {
            var commands = new List<ICommand>();

            var moduleIdentifier = CourseSearch.GetFirstCourseModule(courseId);
            if (moduleIdentifier == null)
            {
                var unitIdentifier = CourseSearch.GetFirstCourseUnit(courseId);

                if (!unitIdentifier.HasValue)
                {
                    var unit = EntityHelper.CreateUnit(courseId, "Unit 1");
                    UnitCommandCreator.Create(null, null, unit, GetContent(unit.UnitName), commands);
                    unitIdentifier = unit.UnitIdentifier;
                }

                var module = EntityHelper.CreateModule(unitIdentifier.Value, "Module 1");
                ModuleCommandCreator.Create(courseId, null, null, module, GetContent(module.ModuleName), commands);
                moduleIdentifier = module.ModuleIdentifier;
            }

            var lesson = EntityHelper.CreateActivity(moduleIdentifier.Value, ActivityType.Lesson, "Lesson 1");
            ActivityCommandCreator.Create(courseId, null, null, lesson, GetContent(lesson.ActivityName), commands);

            ServiceLocator.SendCommand(new RunCommands(courseId, commands.ToArray()));

            DomainCache.Instance.RemoveCourse(courseId);

            return lesson.ActivityIdentifier;
        }

        private static ContentContainer GetContent(string title)
        {
            var content = new ContentContainer();
            content.Title.Text.Default = title;
            return content;
        }

        private void InitModel()
        {
            var courseId = _url.GetGuid("course").Value;
            var activityId = _url.GetGuid("activity").Value;

            var course = CourseSearch.SelectCourse(ServiceLocator.RecordSearch, courseId);

            _model = new OutlineModel(course, activityId);
        }

        private void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this, null, _model.Course.Content.Title.GetText());

            CourseSetup.BindModelToControls(_model.Course);
            UnitSetup.BindModelToControls(_model.Unit);
            UnitPanel.Visible = _model.Course.AllowMultipleUnits;
            ModuleSetup.BindModelToControls(_model.Unit, _model.Module);
            ModuleTreeView.BindModelToControls(_model.Course, _model.Unit.UnitIdentifier, _model.Activity.ActivityIdentifier, CourseSetup.IsPublished, null);
            NoGradebookReminder.Visible = _model.Course.Gradebook == null;
            CreateGradebookLink.HRef = $"/ui/admin/courses/manage?course={_model.Course.Identifier}&activity={_model.Activity.ActivityIdentifier}&panel=course&tab=records";
            ActivitySetup.BindModelToControls(_model.Course, _model.Activity);
            NotificationSetup.BindModelToControls(_model.Course);
        }

        private System.Web.UI.Control LoadActivityEditor(QActivity activity)
        {
            var editor = GetEditorInfo(activity.ActivityType);
            if (editor == null)
            {
                activity.ActivityType = ActivityType.Lesson.ToString();

                Course2Store.ModifyCourseActivityType(_model.Course.Identifier, activity.ActivityIdentifier, ActivityType.Lesson);

                editor = GetEditorInfo(activity.ActivityType);
            }

            var control = LoadControl(editor.ControlPath);

            if (control is IActivityEdit edit)
            {
                edit.ActivityIdentifier = _model.Activity.ActivityIdentifier;
                edit.CourseIdentifier = _model.Course.Identifier;
            }

            return control;
        }

        private System.Web.UI.Control LoadActivityCreator(Guid? unit, Guid? module, string type)
        {
            var control = (ActivityCreate)LoadControl("~/UI/Admin/Courses/Outlines/Controls/ActivityCreate.ascx");
            control.BindModelToControls(_model.Course.Identifier, unit, module, type);
            control.SaveClicked += (x, y) => { HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={_model.Course.Identifier}&activity={y.Value}"); };
            control.CancelClicked += (x, y) => { HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={_model.Course.Identifier}&activity={_model.Activity.ActivityIdentifier}"); };
            return control;
        }

        private EditorInfo GetEditorInfo(string type)
        {
            EditorInfoByType.TryGetValue(type, out var result);
            return result;
        }
    }
}
