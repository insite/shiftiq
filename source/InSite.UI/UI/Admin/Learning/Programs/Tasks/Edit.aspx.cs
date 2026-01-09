using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using static InSite.Persistence.CourseSearch;

namespace InSite.Admin.Records.Programs.Tasks
{
    public partial class Edit : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid ProgramId => Guid.TryParse(Request.QueryString["program"], out var value) ? value : Guid.Empty;
        private Guid TaskId => Guid.TryParse(Request.QueryString["task"], out var value) ? value : Guid.Empty;

        public TaskInfo CurrentTaskInfo
        {
            get => (TaskInfo)ViewState[nameof(CurrentTaskInfo)];
            set => ViewState[nameof(CurrentTaskInfo)] = value;
        }

        public List<TaskInfo> TaskInfoContainer
        {
            get => (List<TaskInfo>)ViewState[nameof(TaskInfoContainer)];
            set => ViewState[nameof(TaskInfoContainer)] = value;
        }

        private Dictionary<string, int> ContentIdentifiers
        {
            get => (Dictionary<string, int>)ViewState[nameof(ContentIdentifiers)];
            set => ViewState[nameof(ContentIdentifiers)] = value;
        }

        #endregion

        #region Fields

        private ComboBox _language;
        private Repeater _repeater;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TriggerChange.AutoPostBack = true;
            TriggerChange.ValueChanged += TriggerChange_ValueChanged;

            PrerequisiteRepeater.ItemCommand += PrerequisiteRepeater_ItemCommand;
            PrerequisiteRepeater.DataBinding += PrerequisiteRepeater_DataBinding;

            SaveButton.Click += (x, y) => Save();

            BindControlsToHandlers(Language, ContentRepeater);
            InitTab();
        }

        private void InitTab()
        {
            var parser = new UrlParser();
            var _url = parser.Parse(Request.RawUrl);
            if (_url.Parameters != null && _url.Parameters.Count > 0)
                SelectTab(_url.Get("tab"));
        }

        private void SelectTab(string tab)
        {
            PresentationTab.IsSelected = tab == "presentation";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SaveButton.Enabled = CanEdit;

            if (ProgramId == Guid.Empty)
                Search.Redirect();

            if (TaskId == Guid.Empty)
                Outline.Redirect(ProgramId);

            if (!IsPostBack)
            {
                Open();
                BindModelToControls(TaskId);
            }
        }

        #endregion

        #region Methods (binding)

        protected void BindModelToControls(Guid taskId)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(taskId);
            if (content == null || content.IsEmpty)
                content = InsertContent(CurrentTaskInfo);

            BindModelToControls(taskId, content);
            TaskPresentationSetup.LoadData(taskId);
        }

        protected void BindControlsToHandlers(ComboBox language, Repeater repeater)
        {
            _language = language;
            _repeater = repeater;

            _language.AutoPostBack = true;
            _language.ValueChanged += (x, y) => OnLanguageChanged();

            _repeater.DataBinding += ContentRepeater_DataBinding;
            _repeater.ItemDataBound += ContentRepeater_ItemDataBound;

        }

        protected void BindModelToControls(Guid taskId, ContentContainer content)
        {
            _language.LoadItems(CurrentSessionState.Identity.Organization.Languages, "TwoLetterISOLanguageName", "EnglishName");

            var sections = GetContentSections().ToList();
            _repeater.DataSource = sections;
            _repeater.DataBind();

            OnLanguageChanged();

            IEnumerable<AssetContentSection> GetContentSections()
            {
                {
                    var section = (AssetContentSection.SingleLine)AssetContentSection.Create(ContentSectionDefault.Title, content.Title.Text);
                    section.Title = "Title";
                    section.Label = "Title";
                    section.IsRequired = true;
                    yield return section;
                }

                {
                    var section = (AssetContentSection.Markdown)AssetContentSection.Create(ContentSectionDefault.Summary, content.Summary.Text);
                    section.Title = "Summary";
                    section.Label = "Summary";
                    section.AllowUpload = true;
                    section.UploadFolderPath = $"/record/{ProgramId}/{taskId}";
                    yield return section;
                }
            }
        }

        public virtual void BindControlsToModel(ContentContainer content)
        {
            content.Title.Text = GetContentValue(ContentSectionDefault.Title, null);
            content.Summary.Text = GetContentValue(ContentSectionDefault.Summary, null);

            MultilingualString GetContentValue(ContentSectionDefault id1, ContentSectionDefault? id2)
            {
                var index = ContentIdentifiers[id1.GetName()];
                var item = _repeater.Items[index];
                var container = (DynamicControl)item.FindControl("Container");
                var section = (SectionBase)container.GetControl();

                return id2.HasValue ? section.GetValue(id2.Value.GetName()) : section.GetValue();
            }
        }

        private void BindTasksInProgram()
        {
            TaskInProgram.LoadItems(TaskInfoContainer.OrderBy(x => x.Type), "TaskIdentifier", "DisplayTitle");
        }

        #endregion

        #region Event handlers

        private void ContentRepeater_DataBinding(object sender, EventArgs e)
        {
            ContentIdentifiers = new Dictionary<string, int>();
        }

        private void ContentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem))
                return;

            var dataItem = (AssetContentSection)e.Item.DataItem;
            if (ContentIdentifiers.ContainsKey(dataItem.Id))
                throw new Exception("Invalid section ID: " + dataItem.Id);

            var container = (DynamicControl)e.Item.FindControl("Container");
            var section = (SectionBase)container.LoadControl(dataItem.ControlPath);
            section.SetOptions(dataItem);

            ContentIdentifiers.Add(dataItem.Id, e.Item.ItemIndex);
        }

        protected void OnLanguageChanged()
        {
            var lang = _language.Value.IfNullOrEmpty("en");

            foreach (RepeaterItem item in _repeater.Items)
            {
                var container = (DynamicControl)item.FindControl("Container");
                var section = (SectionBase)container.GetControl();
                if (section != null)
                    section.SetLanguage(lang);
            }
        }

        private void PrerequisiteRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "PrerequisiteDelete")
                return;

            var id = Guid.Parse((string)e.CommandArgument);
            Course2Store.DeleteProgramPrequisite(id);
            ResetControls();
            PrerequisiteRepeater.DataBind();

        }

        private void PrerequisiteRepeater_DataBinding(object sender, EventArgs e)
        {
            var list = GetPrerequisites(TaskId);
            PrerequisiteRepeater.DataSource = list;
            PrerequisiteRepeaterField.Visible = list.Length > 0;
        }

        private void TriggerChange_ValueChanged(object sender, EventArgs e)
        {
            if (TriggerChange.Value != null)
            {
                TriggerTaskInProgram.Visible = true;
                BindTasksInProgram();
            }
            else
                TriggerTaskInProgram.Visible = false;
        }

        private void ResetControls()
        {
            TriggerChange.ClearSelection();
            TaskInProgram.ClearSelection();
            TriggerTaskInProgram.Visible = false;
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var taskInfoContainer = new List<TaskInfo>();
            var taskObjectData = ProgramHelper.GetTaskObjectData(Organization.OrganizationIdentifier, Organization.ParentOrganizationIdentifier);

            var filter = new TTaskFilter
            {
                ProgramIdentifier = ProgramId,
                ExcludedTask = TaskId,
                ExcludeObjectTypes = new string[] { "Assessment" }
            };

            filter.OrganizationIdentifiers.Add(Organization.OrganizationIdentifier);

            var tasks = ProgramSearch1.GetProgramTasks(filter);

            foreach (var task in tasks)
                taskInfoContainer.Add(ProgramHelper.GetTaskInfo(taskObjectData, task));

            var currentTask = ProgramSearch1.GetProgramTask(TaskId);
            if (currentTask == null)
                Outline.Redirect(ProgramId);

            CurrentTaskInfo = ProgramHelper.GetTaskInfo(taskObjectData, currentTask);

            PageHelper.AutoBindHeader(this, null, CurrentTaskInfo.TaskTitle);

            CancelButton.NavigateUrl = Outline.GetNavigateUrl(ProgramId);

            TaskInfoContainer = taskInfoContainer;

            PrerequisiteRepeater.DataBind();
        }

        private ContentContainer InsertContent(TaskInfo task)
        {
            var content = new ContentContainer();
            content.Title.Text.Default = task.TaskTitle;
            ServiceLocator.ContentStore.SaveContainer(Organization.OrganizationIdentifier, ContentContainerType.Task, TaskId, content);

            return ServiceLocator.ContentSearch.GetBlock(TaskId);
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            try
            {
                if (TriggerChange.Value != null)
                {

                    var p = new TPrerequisite
                    {
                        PrerequisiteIdentifier = UuidFactory.CreateV7(),
                        TriggerChange = TriggerChange.Value,
                        OrganizationIdentifier = Organization.Identifier
                    };

                    if (TaskInProgram.ValueAsGuid.HasValue)
                    {
                        p.TriggerIdentifier = TaskInProgram.ValueAsGuid.Value;
                        p.TriggerType = GetTriggerType(TaskInProgram.ValueAsGuid.Value);
                    }

                    string GetTriggerType(Guid id)
                    {

                        if (TaskInfoContainer != null && TaskInfoContainer.Count > 0)
                        {
                            var taskInfo = TaskInfoContainer.FirstOrDefault(x => x.TaskIdentifier == id);
                            if (taskInfo != null)
                                return taskInfo.Type;
                        }

                        return "Unknown";
                    }

                    if (p.TriggerIdentifier != Guid.Empty)
                    {

                        p.ObjectIdentifier = TaskId;
                        p.ObjectType = "Task";

                        TaskStore.InsertPrerequisite(p);

                        PrerequisiteRepeater.DataBind();
                    }
                }

                var content = ServiceLocator.ContentSearch.GetBlock(TaskId);

                BindControlsToModel(content);

                ServiceLocator.ContentStore.SaveContainer(
                    CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                    ContentContainerType.Task,
                    TaskId,
                    content);

                ResetControls();

                if (TaskPresentationSetup.SaveTaskImageV2())
                    HttpResponseHelper.Redirect(UrlParser.BuildRelativeUrl(Request.RawUrl, "tab", "presentation"));

                Outline.Redirect(ProgramId);
            }
            catch (ApplicationError apperr)
            {
                AlertStatus.AddMessage(AlertType.Error, apperr.Message);
            }
        }

        private TPrerequisiteSearchResult[] GetPrerequisites(Guid taskId)
        {
            var list = CourseSearch.SelectProgramPrerequisites(taskId).ToList();

            var results = new List<TPrerequisiteSearchResult>();

            foreach (var prerequisite in list)
            {
                var item = new TPrerequisiteSearchResult
                {
                    PrerequisiteIdentifier = prerequisite.PrerequisiteIdentifier,
                    TriggerType = prerequisite.TriggerType,
                    TriggerIdentifier = prerequisite.TriggerIdentifier,
                    TriggerChange = Shift.Common.Humanizer.TitleCase(prerequisite.TriggerChange),
                    TriggerDescription = GetTriggerDescription(prerequisite.TriggerType, prerequisite.TriggerIdentifier)
                };

                results.Add(item);
            }

            return results.OrderBy(x => x.TriggerChange)
                .ThenBy(x => x.TriggerDescription)
                .ToArray();

            string GetTriggerDescription(string type, Guid id)
            {
                var taskInfo = TaskInfoContainer.FirstOrDefault(x => x.TaskIdentifier == id);
                if (taskInfo != null)
                    return taskInfo.DisplayTitle;

                return "Unknown";
            }
        }

        #endregion

        #region Redirect

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramId}"
                : null;
        }

        #endregion
    }
}