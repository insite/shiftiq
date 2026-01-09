using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Courses.Courses;
using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.CourseObjects;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.UI.Portal.Learning.Models;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using ActivityField = InSite.Domain.Courses.ActivityField;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ModuleTreeView : BaseUserControl
    {
        #region Events

        public EventHandler NewModuleClicked { get; set; }

        #endregion

        #region Classes

        [Serializable]
        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class ReorderItem
        {
            #region Properties

            [JsonProperty(PropertyName = "id")]
            public string Key { get; private set; }

            public Guid Identifier
            {
                get => _identifier;
                set
                {
                    if (_identifier == Guid.Empty)
                        _identifier = value;
                }
            }

            [JsonProperty(PropertyName = "icon", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Icon { get => _icon; set => _icon = value; }

            [JsonProperty(PropertyName = "text", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Text { get => _text; set => _text = value; }

            [JsonProperty(PropertyName = "items", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public List<ReorderItem> Items { get; set; }

            #endregion

            #region Fields

            private Guid _identifier;

            [NonSerialized]
            private string _icon;

            [NonSerialized]
            private string _text;

            #endregion

            #region Construction

            public ReorderItem(string key, Guid identifier)
            {
                Key = key;
                Identifier = identifier;
            }

            #endregion
        }

        #endregion

        #region Properties

        private Course Model
        {
            get => (Course)ViewState[nameof(Model)];
            set => ViewState[nameof(Model)] = value;
        }

        protected Guid ActivityIdentifier
        {
            get => (Guid?)ViewState[nameof(ActivityIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(ActivityIdentifier)] = value;
        }

        protected Guid CourseIdentifier => Model.Identifier;

        protected bool ShowMetadataChecked => ShowMetadata.Checked;

        private bool IsPublished
        {
            get => (bool?)ViewState[nameof(IsPublished)] ?? false;
            set => ViewState[nameof(IsPublished)] = value;
        }

        protected Guid UnitIdentifier
        {
            get => (Guid?)ViewState[nameof(UnitIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(UnitIdentifier)] = value;
        }

        private List<ReorderItem> ReorderItems
        {
            get => (List<ReorderItem>)ViewState[nameof(ReorderItems)];
            set => ViewState[nameof(ReorderItems)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UnitComboBox.AutoPostBack = true;
            UnitComboBox.ValueChanged += UnitComboBox_ValueChanged;

            ModuleRepeater.ItemCreated += ModuleRepeater_ItemCreated;
            ModuleRepeater.ItemDataBound += ModuleRepeater_ItemDataBound;

            ActionCommandsDropDown.Click += ActionCommandsDropDown_Click;

            ReorderSaveButton.Click += ReorderSaveButton_Click;
            ReorderCancelButton.Click += ReorderCancelButton_Click;

            ShowMetadata.AutoPostBack = true;
            ShowMetadata.CheckedChanged += (x, y) => { CurrentSessionState.ShowMetadata = ShowMetadata.Checked; HttpResponseHelper.Redirect(Request.RawUrl); };
            ShowMetadata.Checked = CurrentSessionState.ShowMetadata;
        }

        #endregion

        #region Event handlers

        private void ActionCommandsDropDown_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "ActionPreview")
            {
                HttpResponseHelper.Redirect(ProgressState.GetPreviewUrl(CourseIdentifier));
            }
            else if (e.CommandName == "ActionReorder")
            {
                StartReorder();
            }
            else if (e.CommandName == "ActionPublish")
            {
                HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={CourseIdentifier}&activity={ActivityIdentifier}&panel=course&tab=publication");
            }
            else if (e.CommandName == "ActionDownload")
            {
                var redirectUrl = new ReturnUrl("course")
                    .GetRedirectUrl($"/ui/admin/courses/download?course={CourseIdentifier}");

                HttpResponseHelper.Redirect(redirectUrl);
            }
            else
            {
                throw new ApplicationError("Unexpected command name: " + e.CommandName);
            }
        }

        private void UnitComboBox_ValueChanged(object sender, EventArgs e)
        {
            var activity = CourseSearch.GetFirstUnitActivity(UnitComboBox.ValueAsGuid.Value);
            HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={Model.Identifier}&activity={activity}");
        }

        private void ModuleRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var repeater = (Repeater)e.Item.FindControl("ActivityRepeater");
            repeater.ItemDataBound += ActivityRepeater_ItemDataBound;
            repeater.ItemCommand += ActivityRepeater_ItemCommand;
        }

        private void ModuleRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var courseId = Model.Identifier;
            var moduleId = (Guid)DataBinder.Eval(e.Item.DataItem, "Identifier");
            var activityId = ActivityIdentifier;

            var a = (HtmlAnchor)e.Item.FindControl("ModuleDeleteLink");
            a.HRef = $"/ui/admin/courses/modules/delete?module={moduleId}";

            var link = (HyperLink)e.Item.FindControl("LessonCreateLink");
            link.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&module={moduleId}&activity={activityId}&create-activity=lesson";

            link = (HyperLink)e.Item.FindControl("AssessmentCreateLink");
            link.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&module={moduleId}&activity={activityId}&create-activity=assessment";

            link = (HyperLink)e.Item.FindControl("SurveyCreateLink");
            link.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&module={moduleId}&activity={activityId}&create-activity=survey";

            link = (HyperLink)e.Item.FindControl("DocumentCreateLink");
            link.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&module={moduleId}&activity={activityId}&create-activity=document";

            link = (HyperLink)e.Item.FindControl("LinkCreateLink");
            link.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&module={moduleId}&activity={activityId}&create-activity=link";

            link = (HyperLink)e.Item.FindControl("VideoCreateLink");
            link.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&module={moduleId}&activity={activityId}&create-activity=video";

            link = (HyperLink)e.Item.FindControl("QuizCreateLink");
            link.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&module={moduleId}&activity={activityId}&create-activity=quiz";

            BindActivities(e.Item, moduleId);
        }

        private void ActivityRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var activityId = (Guid)DataBinder.Eval(e.Item.DataItem, "Identifier");

            var link = (HtmlAnchor)e.Item.FindControl("ActivityDeleteLink");
            link.HRef = $"/ui/admin/courses/activities/delete?course={Model.Identifier}&activity={activityId}";

            link = (HtmlAnchor)e.Item.FindControl("ActivityEditLink");
            link.HRef = $"/ui/admin/courses/manage?course={Model.Identifier}&activity={activityId}";
        }

        private void ActivityRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Task-Remove")
            {
                var taskId = GetTaskId(e.Item);

                var activity = CourseSearch.SelectActivity(taskId);
                if (activity == null)
                    return;

                var parentItem = (RepeaterItem)e.Item.NamingContainer.NamingContainer;
                var moduleId = GetModuleId(parentItem);

                Course2Store.DeleteActivity(CourseIdentifier, moduleId, taskId);

                if (activity.ActivityType == "Document" && !string.IsNullOrEmpty(activity.ActivityUrl))
                    EntityHelper.RemoveFile(activity.ActivityUrl);

                BindActivities(parentItem, moduleId);
            }
        }

        private void StartReorder()
        {
            TreeWrapper.Visible = false;
            TreeReorderWrapper.Visible = true;

            var items = new List<ReorderItem>();
            var keyGenerator = new RandomStringGenerator(RandomStringType.AlphanumericCaseSensitive, 3);

            if (Model.AllowMultipleUnits)
            {
                foreach (var unit in Model.Units)
                {
                    var unitItem = new ReorderItem(keyGenerator.Next(), unit.Identifier)
                    {
                        Text = unit.Content.Title.GetText(),
                        Items = new List<ReorderItem>()
                    };

                    AddModules(unit.Modules, unitItem.Items);

                    items.Add(unitItem);
                }
            }
            else
            {
                var modules = Model.Units.FirstOrDefault(x => x.Identifier == UnitIdentifier).Modules;

                AddModules(modules, items);
            }

            ReorderState.Value = JsonConvert.SerializeObject(items);
            ReorderItems = items;

            void AddModules(List<Module> modules, List<ReorderItem> itemsList)
            {
                foreach (var module in modules)
                {
                    var moduleItem = new ReorderItem(keyGenerator.Next(), module.Identifier)
                    {
                        Text = module.Content.Title.GetText(),
                        Items = new List<ReorderItem>()
                    };

                    foreach (var activity in module.Activities)
                    {
                        moduleItem.Items.Add(new ReorderItem(keyGenerator.Next(), activity.Identifier)
                        {
                            Text = activity.Content.Title.GetText(),
                            Icon = GetTypeIconName(activity.Type)
                        });
                    }

                    itemsList.Add(moduleItem);
                }
            }
        }

        private void ReorderCancelButton_Click(object sender, EventArgs e)
        {
            TreeWrapper.Visible = true;
            TreeReorderWrapper.Visible = false;
            ReorderState.Value = null;
            ReorderItems = null;
        }

        private void ReorderSaveButton_Click(object sender, EventArgs e)
        {
            Reorder(ReorderState.Value);

            HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={Model.Identifier}&activity={ActivityIdentifier}");
        }

        #endregion

        #region Methods (binding)

        private void BindActivities(RepeaterItem item, Guid moduleId)
        {
            var repeater = (Repeater)item.FindControl("ActivityRepeater");

            var datasource = Model.FindModule(moduleId)
                .GetSupportedActivites()
                .Select(x => new
                {
                    x.Identifier,
                    Code = x.FullCode,
                    x.Type,
                    Name = x.Content.Title.GetText()
                })
                .ToList();

            repeater.Visible = datasource.Count > 0;
            repeater.DataSource = datasource;
            repeater.DataBind();
        }

        private static Guid GetModuleId(RepeaterItem item) =>
            Guid.Parse(((ITextControl)item.FindControl("ModuleID")).Text);

        private static Guid GetTaskId(RepeaterItem item) =>
            Guid.Parse(((ITextControl)item.FindControl("TaskID")).Text);

        public void BindModelToControls(Course model, Guid unit, Guid activity, bool isPublished, string previewUrl)
        {
            Model = model;
            UnitIdentifier = unit;
            ActivityIdentifier = activity;
            IsPublished = isPublished;

            var courseId = Model.Identifier;
            var activityId = ActivityIdentifier;

            UnitComboBoxWrapper.Visible = Model.AllowMultipleUnits;
            UnitComboBox.CourseID = courseId;
            UnitComboBox.ShowCodes = ShowMetadata.Checked;
            UnitComboBox.RefreshData();
            UnitCommandWrapper.Visible = Model.AllowMultipleUnits;

            ModuleLessonCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-module=Lesson";
            ModuleAssessmentCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-module=Assessment";
            ModuleSurveyCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-module=Survey";

            ModuleDocumentCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-module=Document";
            ModuleLinkCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-module=Link";
            ModuleVideoCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-module=Video";

            ModuleQuizCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-module=Quiz";

            UnitLessonCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-unit=Lesson";
            UnitAssessmentCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-unit=Assessment";
            UnitSurveyCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-unit=Survey";

            UnitDocumentCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-unit=Document";
            UnitLinkCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-unit=Link";
            UnitVideoCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-unit=Video";

            UnitQuizCreateLink.NavigateUrl = $"/ui/admin/courses/manage?course={courseId}&activity={activityId}&create-unit=Quiz";

            HistoryButton.NavigateUrl = $"/ui/admin/logs/aggregates/outline?aggregate={courseId}&returnURL="
                + HttpUtility.UrlEncode($"/ui/admin/courses/manage?course={courseId}");

            var modules = Model.Units.FirstOrDefault(x => x.Identifier == UnitIdentifier).Modules;

            ModuleRepeater.DataSource = modules.Select(x => new
            {
                x.Identifier,
                Code = x.FullCode,
                Name = x.Content.Title.GetText()
            });
            ModuleRepeater.DataBind();

            UnitComboBox.ValueAsGuid = UnitIdentifier;

            if (ShowMetadata.Checked)
            {
                var u = Model.FindUnit(UnitIdentifier);
                if (u.Prerequisites.Count > 0)
                    UnitComboBox.FindOptionByValue(UnitIdentifier.ToString()).Text =
                        $"{u.Content.Title.GetText()} (Unlocked by {Shift.Common.Humanizer.ToQuantity(u.Prerequisites.Count, "Prerequisite")})";
            }

            ActionCommandsDropDown.Items["ActionReorder"].Visible = modules.Count > 0 || Model.AllowMultipleUnits && UnitComboBox.Items.Count > 0;
        }

        #endregion

        #region Methods (render)

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

        protected string CreateMetadataHtmlForModule(object moduleIdentifier)
        {
            if (!ShowMetadata.Checked)
                return string.Empty;

            var moduleId = (Guid)moduleIdentifier;

            var html = new StringBuilder();

            var module = Model.FindModule(moduleId);

            if (module.IsAdaptive)
                html.Append(CreateLabelHtml("info", "This module is adaptive.", $"Adaptive"));

            if (module.Prerequisites.Count > 0)
                html.Append(CreateLabelHtml("warning", "", $"Unlocked by {Shift.Common.Humanizer.ToQuantity(module.Prerequisites.Count, "Prerequisite")}"));

            return html.ToString();

            string CreateLabelHtml(string css, string title, string label)
            {
                if (title != null)
                    title = title.Replace("'", "");

                return $"<span class='badge bg-{css}' title='{title}'>{label}</span> ";
            }
        }

        protected string CreateMetadataHtml(object activityIdentifier)
        {
            if (!ShowMetadata.Checked)
                return string.Empty;

            if (_labels == null)
                _labels = GetLabels(Model.Identifier);

            var activityId = (Guid)activityIdentifier;

            var html = new StringBuilder();

            var activity = Model.GetActivity(activityId);

            if (activity.GradeItem == null)
                html.Append(
                    CreateLabelHtml(
                        "danger",
                        "This activity is not connected to a grade item in your gradebook.",
                        "Not Graded"));

            if (activity.IsAdaptive)
                html.Append(
                    CreateLabelHtml(
                        "info",
                        "This activity is adaptive.",
                        "Adaptive"));

            if (activity.Prerequisites.Count > 0)
                html.Append(
                    CreateLabelHtml(
                        "warning",
                        "",
                        $"Unlocked by {Shift.Common.Humanizer.ToQuantity(activity.Prerequisites.Count, "Prerequisite")}"));

            if (activity.Requirement != RequirementType.None)
                html.Append(
                    CreateLabelHtml(
                        "primary",
                        "This is the requirement for completion.",
                        $"{Shift.Common.Humanizer.TitleCase(activity.Requirement.ToString())}"));

            if (activity.PrivacyGroups.Count > 0)
                html.Append(
                    CreateLabelHtml(
                        "custom-default",
                        string.Join(",", activity.PrivacyGroups.Select(x => x.Name)),
                        $"Access Granted to {Shift.Common.Humanizer.ToQuantity(activity.PrivacyGroups.Count, "Group")}"));

            if (_labels.ContainsKey(activityId))
            {
                var competencies = _labels[activityId];
                if (competencies != null)
                {
                    foreach (var competency in competencies)
                    {
                        var label = $"{competency.CompetencyLabel ?? competency.CompetencyType} {competency.CompetencyCode ?? competency.CompetencyAsset.ToString()}";

                        var css = StringHelper.Equals(competency.RelationshipType, "Relates") ? "primary" : "success";

                        html.Append(CreateLabelHtml(css, competency.CompetencyTitle, label));
                    }
                }
            }

            return html.ToString();

            string CreateLabelHtml(string css, string title, string label)
            {
                if (title != null)
                    title = title.Replace("'", "");

                return $"<span class='badge bg-{css}' title='{title}'>{label}</span> ";
            }
        }

        protected static string GetTypeIconName(string type)
        {
            switch (type.ToLower())
            {
                case "lesson":
                    return "chalkboard-teacher";
                case "assessment":
                    return "balance-scale";
                case "survey":
                    return "check-square";
                case "document":
                    return "file-pdf";
                case "link":
                    return "link";
                case "video":
                    return "video";
                case "quiz":
                    return "square-question";
                default:
                    return "bomb";
            }
        }

        #endregion

        #region Methods (reorder)

        private void Reorder(string stateJson)
        {
            if (stateJson.IsEmpty())
                return;

            var state = JsonConvert.DeserializeObject<List<ReorderItem>>(ReorderState.Value);
            var keyMap = new Dictionary<string, Guid>();
            var levels = new List<Dictionary<Guid, Tuple<Guid, int>>>(3);

            BindKeys(ReorderItems);

            if (!BindLevels(state, Guid.Empty, 0) || keyMap.Count != 0)
                return;

            var commands = new List<ICommand>();

            if (Model.AllowMultipleUnits)
            {
                if (levels.Count > 3)
                    return;

                var unitIds = UpdateUnits(levels.Count > 0 ? levels[0] : null, x => x.CourseIdentifier == CourseIdentifier);
                if (unitIds == null)
                    return;

                var moduleIds = UpdateModules(levels.Count > 1 ? levels[1] : null, x => unitIds.Contains(x.UnitIdentifier));
                if (moduleIds == null)
                    return;

                var activityIds = UpdateActivities(levels.Count > 2 ? levels[2] : null, x => moduleIds.Contains(x.ModuleIdentifier));
                if (activityIds == null)
                    return;
            }
            else
            {
                if (levels.Count > 2)
                    return;

                var moduleIds = UpdateModules(levels.Count > 0 ? levels[0] : null, x => x.UnitIdentifier == UnitIdentifier);
                if (moduleIds == null)
                    return;

                var activityIds = UpdateActivities(levels.Count > 1 ? levels[1] : null, x => moduleIds.Contains(x.ModuleIdentifier));
                if (activityIds == null)
                    return;
            }

            ServiceLocator.SendCommand(new RunCommands(CourseIdentifier, commands.ToArray()));

            DomainCache.Instance.RemoveCourse(CourseIdentifier);

            void BindKeys(List<ReorderItem> items)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];

                    keyMap.Add(item.Key, item.Identifier);

                    if (item.Items != null && item.Items.Count > 0)
                        BindKeys(item.Items);
                }
            }

            bool BindLevels(List<ReorderItem> items, Guid parent, int depth)
            {
                if (levels.Count == depth)
                    levels.Add(new Dictionary<Guid, Tuple<Guid, int>>());

                var dict = levels[depth];

                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];

                    if (keyMap.ContainsKey(item.Key))
                    {
                        item.Identifier = keyMap[item.Key];
                        keyMap.Remove(item.Key);
                    }
                    else
                    {
                        return false;
                    }

                    dict.Add(item.Identifier, new Tuple<Guid, int>(parent, i + 1));

                    if (item.Items != null && item.Items.Count > 0)
                        BindLevels(item.Items, item.Identifier, depth + 1);
                }

                return true;
            }

            List<Guid> UpdateUnits(Dictionary<Guid, Tuple<Guid, int>> level, Expression<Func<QUnit, bool>> filter)
            {
                var entities = CourseSearch.BindUnits(x => x, filter);
                if (entities.Length != (level == null ? 0 : level.Count))
                    return null;

                foreach (var entity in entities)
                {
                    if (level.TryGetValue(entity.UnitIdentifier, out var item))
                        commands.Add(new ModifyCourseUnitSequence(CourseIdentifier, entity.UnitIdentifier, item.Item2));
                    else
                        return null;
                }

                return entities.Select(x => x.UnitIdentifier).ToList();
            }

            List<Guid> UpdateModules(Dictionary<Guid, Tuple<Guid, int>> level, Expression<Func<QModule, bool>> filter)
            {
                var entities = CourseSearch.BindModules(x => x, filter);
                if (entities.Length != (level == null ? 0 : level.Count))
                    return null;

                foreach (var entity in entities)
                {
                    if (level.TryGetValue(entity.ModuleIdentifier, out var item))
                    {
                        if (item.Item1 != Guid.Empty)
                            commands.Add(new MoveCourseModule(CourseIdentifier, entity.ModuleIdentifier, item.Item1));

                        commands.Add(new ModifyCourseModuleSequence(CourseIdentifier, entity.ModuleIdentifier, item.Item2));
                    }
                    else
                        return null;
                }

                return entities.Select(x => x.ModuleIdentifier).ToList();
            }

            List<Guid> UpdateActivities(Dictionary<Guid, Tuple<Guid, int>> level, Expression<Func<QActivity, bool>> filter)
            {
                var entities = CourseSearch.BindActivities(x => x, filter);
                if (entities.Length != (level == null ? 0 : level.Count))
                    return null;

                foreach (var entity in entities)
                {
                    if (level.TryGetValue(entity.ActivityIdentifier, out var item))
                    {
                        if (item.Item1 != Guid.Empty)
                            commands.Add(new MoveCourseActivity(CourseIdentifier, entity.ActivityIdentifier, item.Item1));

                        commands.Add(new ModifyCourseActivityFieldInt(CourseIdentifier, entity.ActivityIdentifier, ActivityField.ActivitySequence, item.Item2));
                    }
                    else
                        return null;
                }

                return entities.Select(x => x.ActivityIdentifier).ToList();
            }
        }

        #endregion
    }
}