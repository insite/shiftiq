using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.UI.Admin.Records.Programs.Controls
{
    public partial class ProgramTaskRepeater : BaseUserControl
    {
        #region Properties

        private bool AutoPostBack
        {
            get => ((bool?)ViewState[nameof(AutoPostBack)]) ?? false;
            set => ViewState[nameof(AutoPostBack)] = value;
        }

        public bool Disabled
        {
            get => ((bool?)ViewState[nameof(Disabled)]) ?? false;
            set => ViewState[nameof(Disabled)] = value;
        }

        public string ObjectType
        {
            get => (string)ViewState[nameof(ObjectType)];
            set => ViewState[nameof(ObjectType)] = value;
        }

        public string HelperMsg
        {
            get => (string)ViewState[nameof(HelperMsg)];
            set => ViewState[nameof(HelperMsg)] = value;
        }

        public List<TTask> ProgramTasks
        {
            get => (List<TTask>)ViewState[nameof(ProgramTasks)];
            set => ViewState[nameof(ProgramTasks)] = value;
        }

        private Guid? ProgramIdentifier
        {
            get => (Guid?)ViewState[nameof(ProgramIdentifier)];
            set => ViewState[nameof(ProgramIdentifier)] = value;
        }
        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            TaskRepeater.ItemCreated += TaskRepeater_ItemCreated;
            TaskRepeater.ItemDataBound += TaskRepeater_ItemDataBound;
        }

        private void TaskRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!AutoPostBack)
                return;

            if (!IsContentItem(e))
                return;

            var name = (CheckBox)e.Item.FindControl("TaskName");
            name.AutoPostBack = true;
            name.CheckedChanged += TaskName_CheckedChanged;

            var childTaskRepeater = (Repeater)e.Item.FindControl("ChildTaskRepeater");
            childTaskRepeater.ItemDataBound += ChildTaskRepeater_ItemDataBound;
        }

        private void TaskName_CheckedChanged(object sender, EventArgs e)
        {
            var item = (CheckBox)sender;
            var isChecked = item.Checked;
            var guid = Guid.Parse(((HiddenField)item.Parent.FindControl("TaskIdentifier")).Value);
            var type = ((HiddenField)item.Parent.FindControl("ObjectType")).Value;

            if (type.Equals("Assessment") && isChecked)
            {
                var childTaskRepeater = (Repeater)item.FindControl("ChildTaskRepeater");

                List<ProgramTaskItem> childItems = GetChildTaskItems(guid);

                childTaskRepeater.DataSource = childItems;
                childTaskRepeater.DataBind();
            }
        }

        private void TaskRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (ProgramTaskItem)e.Item.DataItem;

            var name = (CheckBox)e.Item.FindControl("TaskName");
            name.Checked = item.IsSelected;
            name.Text = item.TaskName;

            var identifier = (HiddenField)e.Item.FindControl("TaskIdentifier");

            if (ProgramIdentifier.HasValue)
                identifier.Value = item.ObjectIdentifier.ToString();
            else
                identifier.Value = item.ProgramIdentifier.ToString();

            var objectType = (HiddenField)e.Item.FindControl("ObjectType");
            objectType.Value = item.ObjectType.ToString();

            if (item.ObjectType == "Assessment" && item.IsSelected)
                BindChildTaskItemRepeater(e.Item, (ProgramTaskItem)e.Item.DataItem);
        }

        private void ChildTaskRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (ProgramTaskItem)e.Item.DataItem;

            var name = (CheckBox)e.Item.FindControl("ChildTaskName");
            name.Checked = item.IsSelected;
            name.Text = item.TaskName;

            var identifier = (HiddenField)e.Item.FindControl("ChildTaskIdentifier");

            if (ProgramIdentifier.HasValue)
                identifier.Value = item.ObjectIdentifier.ToString();
            else
                identifier.Value = item.ProgramIdentifier.ToString();
        }

        private void BindChildTaskItemRepeater(RepeaterItem item, ProgramTaskItem parentTask)
        {
            var childTaskRepeater = (Repeater)item.FindControl("ChildTaskRepeater");

            List<ProgramTaskItem> childItems = GetChildTaskItems(parentTask.ObjectIdentifier);

            childTaskRepeater.DataSource = childItems;
            childTaskRepeater.DataBind();
        }

        public void BindModelToControls(Guid? programId)
        {
            ProgramIdentifier = programId;
            AutoPostBack = true;

            var (programTasks, items) = ProgramHelper.GetTasksAndItems(programId, ObjectType, Organization.OrganizationIdentifier, null);

            ProgramTasks = programTasks;

            TaskRepeater.DataSource = items.OrderBy(x => x.TaskName);
            TaskRepeater.DataBind();

            TaskSearchLink.NavigateUrl = "/ui/admin/records/achievements/search";
            TaskSearchLink.Visible = false;

            TaskLabel.InnerText = ProgramHelper.GenerateTitle(ObjectType);

            Visible = items.Count > 0;
            HelperMsgPlaceholder.Visible = HelperMsg.HasValue();
            HelperMsgText.Text = HelperMsg;
        }

        public void UpdateTasks(Guid? programId)
        {
            if (programId.HasValue)
            {
                foreach (RepeaterItem item in TaskRepeater.Items)
                {
                    var checkBox = (CheckBox)item.FindControl("TaskName");
                    var guid = Guid.Parse(((HiddenField)item.FindControl("TaskIdentifier")).Value);
                    var type = ((HiddenField)item.FindControl("ObjectType")).Value;

                    UpdateTask(programId.Value, guid, type, GetTaskCompletionRequirement(type), checkBox.Checked, checkBox.Text);

                    if (type == "Assessment")
                    {
                        var childRepeater = (Repeater)item.FindControl("ChildTaskRepeater");
                        foreach (RepeaterItem childItem in childRepeater.Items)
                        {
                            var childCheckBox = (CheckBox)childItem.FindControl("ChildTaskName");
                            var childGuid = Guid.Parse(((HiddenField)childItem.FindControl("ChildTaskIdentifier")).Value);

                            if (checkBox.Checked)
                                UpdateTask(programId.Value, childGuid, "AssessmentForm", GetTaskCompletionRequirement("AssessmentForm"), childCheckBox.Checked, childCheckBox.Text);
                            else
                                UpdateTask(programId.Value, childGuid, "AssessmentForm", GetTaskCompletionRequirement("AssessmentForm"), false, childCheckBox.Text);
                        }
                    }
                }
            }
        }

        private string GetTaskCompletionRequirement(string type)
        {
            if (type == "Survey")
                return "Submission Completed";

            return "Credential Granted";
        }

        public void UpdateTask(Guid? programId, Guid objectIdentifier, string objectType, string taskCompletionRequirement, bool isChecked, string taskTitle)
        {
            var programUsers = ProgramSearch1.GetProgramUsers(new VProgramEnrollmentFilter() { ProgramIdentifier = programId, OrganizationIdentifier = Organization.Identifier });

            if (isChecked)
                EnsureEnrollment(programId, objectIdentifier, objectType, taskCompletionRequirement, taskTitle, programUsers);
            else
                RemoveEnrollment(programId, objectIdentifier, objectType, programUsers);

            var filter = new TTaskFilter { ProgramIdentifier = programId };

            filter.OrganizationIdentifiers.Add(Organization.OrganizationIdentifier);

            ProgramTasks = ProgramSearch1.GetProgramTasks(filter);
        }

        #region Helper Methods

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

        private List<ProgramTaskItem> GetChildTaskItems(Guid guid)
        {
            return ServiceLocator.BankSearch.GetForms(new QBankFormFilter()
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                IncludeFormStatus = "Published",
                BankIdentifier = guid
            }).Select(x => new ProgramTaskItem()
            {
                ProgramIdentifier = ProgramIdentifier.Value,
                TaskIdentifier = ProgramTasks.FirstOrDefault(y => y.ObjectIdentifier == x.FormIdentifier) != null ?
                    ProgramTasks.FirstOrDefault(y => y.ObjectIdentifier == x.FormIdentifier).TaskIdentifier : Guid.Empty,
                ObjectIdentifier = x.FormIdentifier,
                ObjectType = "AssessmentForm",
                TaskName = x.FormName,
                IsSelected = ProgramTasks.Any(y => y.ObjectIdentifier == x.FormIdentifier)
            }).ToList();
        }

        private void InsertContent(string title, Guid taskId)
        {
            var content = new ContentContainer();
            content.Title.Text.Default = title;
            ServiceLocator.ContentStore.SaveContainer(Organization.OrganizationIdentifier, ContentContainerType.Task, taskId, content);
        }

        #endregion
    }
}