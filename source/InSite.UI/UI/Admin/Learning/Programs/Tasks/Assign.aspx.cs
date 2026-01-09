using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Records.Programs
{
    public partial class Edit : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? ProgramId => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        public List<TaskInfo> TaskInfoContainer
        {
            get => (List<TaskInfo>)ViewState[nameof(TaskInfoContainer)];
            set => ViewState[nameof(TaskInfoContainer)] = value;
        }

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SaveButton.Click += SaveButton_Click;
            AddTask.Click += AddTaskButton_Click;

            TaskRepeater.ItemCommand += Repeater_ItemCommand;

            TaskType.AutoPostBack = true;
            TaskType.ValueChanged += TaskType_ValueChanged;

            TaskList.AutoPostBack = true;
            TaskList.ValueChanged += TaskList_ValueChanged;

            if (!IsPostBack)
            {
                HelperText.Visible = false;
                TaskSubListDiv.Visible = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
                if (program == null)
                    Search.Redirect();

                PageHelper.AutoBindHeader(this, null, program.ProgramName);

                ProgramCode.Text = program.ProgramCode;
                ProgramName.Text = program.ProgramName;
                ProgramIdentifier.Text = program.ProgramIdentifier.ToString();
                ProgramDescription.Text = program.ProgramDescription;
                ProgramTag.Text = program.ProgramTag.HasValue() ? program.ProgramTag : "None";

                CancelButton.NavigateUrl = Outline.GetNavigateUrl(ProgramId.Value);

                BindTasks(program.ProgramIdentifier);
            }
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
                return;

            var program = ProgramSearch.GetProgram(ProgramId.Value);

            if (program == null)
                return;

            EnsureProgramCompletion(program);

            Outline.Redirect(ProgramId.Value);
        }

        private void AddTaskButton_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
                return;

            if (TaskType.Value == null)
                return;

            var program = ProgramSearch.GetProgram(ProgramId.Value);

            if (program == null)
                return;

            var programUsers = ProgramSearch1.GetProgramUsers(new VProgramEnrollmentFilter() { ProgramIdentifier = ProgramId, OrganizationIdentifier = Organization.Identifier });
            var taskType = TaskType.Value;

            if (taskType != "Assessment")
            {
                var objectIdentifier = TaskList.ValueAsGuid;
                if (objectIdentifier == null)
                    return;

                AddOtherTask(program, programUsers, taskType, objectIdentifier);

                HttpResponseHelper.Redirect($"/ui/admin/learning/programs/tasks/assign?id={ProgramId}");
            }

            var assessmentIdentifier = TaskList.ValueAsGuid;
            var assessmentFormIdentifier = TaskSubList.ValueAsGuid;
            if (assessmentIdentifier == null || assessmentFormIdentifier == null)
                return;

            AddAssessmentTask(program, programUsers, taskType, assessmentIdentifier, assessmentFormIdentifier);

            HttpResponseHelper.Redirect($"/ui/admin/learning/programs/tasks/assign?id={ProgramId}");
        }

        private void TaskType_ValueChanged(object sender, EventArgs e)
        {
            if (TaskType.Value == null)
                TaskList.Value = null;

            TaskSubList.ClearSelection();

            SetTaskList(TaskType.Value);
        }

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var taskId = Guid.Parse((string)e.CommandArgument);
                var taskInfo = TaskInfoContainer.Where(x => x.TaskIdentifier == taskId).FirstOrDefault();
                var programUsers = ProgramSearch1.GetProgramUsers(new VProgramEnrollmentFilter() { ProgramIdentifier = ProgramId, OrganizationIdentifier = Organization.Identifier });

                RemoveEnrollment(ProgramId, taskInfo.ObjectIdentifier, taskInfo.Type, programUsers);

                HttpResponseHelper.Redirect($"/ui/admin/learning/programs/tasks/assign?id={ProgramId}");
            }
        }

        private void TaskList_ValueChanged(object sender, EventArgs e)
        {
            SetSubTaskList(TaskList.Value);
        }

        #endregion

        #region Binding

        private void SetTaskList(string taskType)
        {
            var items = ProgramHelper.GetTaskObjects(taskType, Organization.OrganizationIdentifier);
            TaskList.EmptyMessage = taskType;
            TaskList.LoadItems(items, "Value", "Text");
            HelperText.Visible = false;

            if (taskType == "Achievement")
            {
                HelperText.Visible = true;
                HelperTextLabel.Text = "Adding a Standalone Achievement will not include its related activity.";
            }

            if (taskType != "Assessment")
            {
                TaskSubListDiv.Visible = false;
                return;
            }

            TaskSubListDiv.Visible = true;
            SetSubTaskList(TaskList.Value);
        }

        public void SetSubTaskList(string assessment)
        {
            if (!Guid.TryParse(assessment, out Guid guid))
                return;
            var items = ServiceLocator.BankSearch.GetForms(new QBankFormFilter()
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                IncludeFormStatus = "Published",
                BankIdentifier = guid
            }).ToList();

            TaskSubList.EmptyMessage = "Assessment Form";
            TaskSubList.LoadItems(items, "FormIdentifier", "FormName");
        }

        private void BindTasks(Guid programId)
        {
            var organization = CurrentSessionState.Identity.Organization;

            var taskObjectData = ProgramHelper.GetTaskObjectData(organization.Identifier, organization.ParentOrganizationIdentifier);

            var filter = new TTaskFilter
            {
                ProgramIdentifier = programId,
                ExcludeObjectTypes = new string[] { "Assessment" }
            };

            filter.OrganizationIdentifiers.Add(organization.OrganizationIdentifier);

            var tasks = ProgramSearch1.GetProgramTasks(filter);

            TaskInfoContainer = ProgramHelper.BindTaskInfo(taskObjectData, tasks);

            TaskRepeater.DataSource = TaskInfoContainer;
            TaskRepeater.DataBind();
        }

        #endregion

        #region ProgramEnrollment
        private static void RemoveEnrollment(Guid? programId, Guid objectIdentifier, string objectType, List<VProgramEnrollment> programUsers)
        {
            var task = TaskStore.Delete(programId.Value, objectIdentifier);
            if (task != null)
            {
                TaskStore.DeleteEnrollments(Organization.Identifier, task.TaskIdentifier);

                foreach (var programUser in programUsers)
                {
                    if (objectType == "Logbook")
                        ProgramHelper.EnsureLogbookEnrollmentDeletion(programUser.UserIdentifier, task.ObjectIdentifier, Organization.Identifier);
                    else if (objectType == "Course")
                        ProgramHelper.EnsureCourseEnrollmentDeletion(programUser.UserIdentifier, task.ObjectIdentifier, Organization.Identifier);
                }

                ProgramStore.RemoveCompletionTaskIdFromProgram(programId.Value, task.TaskIdentifier);
                ServiceLocator.ContentStore.DeleteContainer(task.TaskIdentifier);
            }
        }

        private void EnsureEnrollment(Guid? programId, Guid objectIdentifier, string objectType, string taskCompletionRequirement, string taskTitle, List<VProgramEnrollment> programUsers)
        {
            var taskId = TaskStore.Insert(Organization.Identifier, programId.Value, objectIdentifier, objectType, taskCompletionRequirement);

            foreach (var programUser in programUsers)
            {
                var completedTasks = ProgramSearch1.GetProgramTaskCompletionForUser(programId.Value, programUser.UserIdentifier);
                TaskStore.EnrollUserToProgramTask(Organization.Identifier, taskId, programUser.UserIdentifier, objectIdentifier, completedTasks);

                if (objectType == "Logbook")
                    ProgramHelper.EnsureLogbookEnrollment(programUser.UserIdentifier, objectIdentifier);
                else if (objectType == "Course")
                    ProgramHelper.EnsureCourseEnrollment(programUser.UserIdentifier, objectIdentifier);
                else if (objectType == "Achievement")
                    ProgramHelper.EnsureProgramAchievementEnrollement(programId, objectIdentifier, taskId, programUser);
            }

            InsertContent(taskTitle, taskId);
        }

        private static void EnsureProgramCompletion(TProgram program)
        {
            var enrollments = ProgramSearch1.GetProgramUsers(new VProgramEnrollmentFilter()
            { ProgramIdentifier = program.ProgramIdentifier, OrganizationIdentifier = program.OrganizationIdentifier })
                .Select(x => x.UserIdentifier).ToList();

            foreach (var userIdentifier in enrollments)
            {
                var tasks = TaskStore.EnrollUserToProgramTasks(Organization.Identifier, program.ProgramIdentifier, userIdentifier);

                if ((program.CompletionTaskIdentifier.HasValue && ServiceLocator.ProgramSearch.IsTaskCompletedByLearner(program.CompletionTaskIdentifier.Value, userIdentifier)) ||
                    ServiceLocator.ProgramSearch.IsProgramFullyCompletedByLearner(program.ProgramIdentifier, userIdentifier))
                {
                    if (program.AchievementIdentifier.HasValue)
                        ProgramHelper.SendGrantCommands(TriggerEffectCommand.Grant, CurrentSessionState.Identity.Organization.Identifier, program.AchievementIdentifier.Value, userIdentifier);

                    ProgramStore.InsertEnrollment(Organization.Identifier, program.ProgramIdentifier, userIdentifier, User.Identifier, DateTimeOffset.UtcNow);
                }
            }
        }

        private void AddOtherTask(TProgram program, List<VProgramEnrollment> programUsers, string taskType, Guid? objectIdentifier)
        {
            var taskTitle = TaskInfoContainer.Where(x => x.ObjectIdentifier == objectIdentifier).FirstOrDefault()?.TaskTitle;

            EnsureEnrollment(ProgramId, objectIdentifier.Value, taskType, GetTaskCompletionRequirement(taskType), taskTitle, programUsers);
            EnsureProgramCompletion(program);
        }

        private void AddAssessmentTask(TProgram program, List<VProgramEnrollment> programUsers, string taskType, Guid? assessmentIdentifier, Guid? assessmentFormIdentifier)
        {
            var assessmentTitle = TaskInfoContainer.Where(x => x.ObjectIdentifier == assessmentIdentifier).FirstOrDefault()?.TaskTitle;
            var assessmentFormTitle = TaskInfoContainer.Where(x => x.ObjectIdentifier == assessmentFormIdentifier).FirstOrDefault()?.TaskTitle;

            EnsureEnrollment(ProgramId, assessmentIdentifier.Value, taskType, GetTaskCompletionRequirement(taskType), assessmentTitle, programUsers);
            EnsureEnrollment(ProgramId, assessmentFormIdentifier.Value, "AssessmentForm", GetTaskCompletionRequirement("AssessmentForm"), assessmentFormTitle, programUsers);
            EnsureProgramCompletion(program);
        }

        #endregion

        #region Helper Functions

        private void InsertContent(string title, Guid taskId)
        {
            var content = new ContentContainer();
            content.Title.Text.Default = title;
            ServiceLocator.ContentStore.SaveContainer(Organization.OrganizationIdentifier, ContentContainerType.Task, taskId, content);
        }

        private string GetTaskCompletionRequirement(string type)
        {
            if (type == "Survey")
                return "Submission Completed";

            return "Credential Granted";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramId}"
                : null;
        }

        protected string GetDisplayTextType(string type)
            => !string.IsNullOrEmpty(type) ? Shift.Common.Humanizer.TitleCase(type) : null;

        #endregion
    }
}
