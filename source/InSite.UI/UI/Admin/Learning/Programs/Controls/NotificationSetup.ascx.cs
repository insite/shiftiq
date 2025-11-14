using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Application.Records.Read;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class NotificationSetup : UserControl
    {
        private Guid ProgramIdentifier
        {
            get => (Guid)ViewState[nameof(ProgramIdentifier)];
            set => ViewState[nameof(ProgramIdentifier)] = value;
        }

        public List<TaskInfo> TaskInfoContainer
        {
            get => (List<TaskInfo>)ViewState[nameof(TaskInfoContainer)];
            set => ViewState[nameof(TaskInfoContainer)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var program = ProgramSearch.GetProgram(ProgramIdentifier);

            program.NotificationStalledLearnerMessageIdentifier = NotificationStalledLearnerMessageIdentifier.Value;
            program.NotificationStalledAdministratorMessageIdentifier = NotificationStalledAdministratorMessageIdentifier.Value;
            program.NotificationCompletedLearnerMessageIdentifier = NotificationCompletedLearnerMessageIdentifier.Value;
            program.NotificationCompletedAdministratorMessageIdentifier = NotificationCompletedAdministratorMessageIdentifier.Value;
            program.NotificationStalledTriggerDay = NotificationStalledTriggerDay.ValueAsInt;
            program.NotificationStalledReminderLimit = NotificationStalledReminderLimit.ValueAsInt;
            program.CompletionTaskIdentifier = TaskInProgram.ValueAsGuid;

            ProgramStore.Update(program, CurrentSessionState.Identity.User.UserIdentifier);
        }

        public void LoadData(TProgram program)
        {
            ProgramIdentifier = program.ProgramIdentifier;

            NotificationStalledLearnerMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            NotificationStalledLearnerMessageIdentifier.Value = program.NotificationStalledLearnerMessageIdentifier;

            NotificationStalledAdministratorMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            NotificationStalledAdministratorMessageIdentifier.Value = program.NotificationStalledAdministratorMessageIdentifier;

            NotificationCompletedLearnerMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            NotificationCompletedLearnerMessageIdentifier.Value = program.NotificationCompletedLearnerMessageIdentifier;

            NotificationCompletedAdministratorMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            NotificationCompletedAdministratorMessageIdentifier.Value = program.NotificationCompletedAdministratorMessageIdentifier;

            NotificationStalledTriggerDay.ValueAsInt = program.NotificationStalledTriggerDay;
            NotificationStalledReminderLimit.ValueAsInt = program.NotificationStalledReminderLimit;

            BindTasksInProgram(program.CompletionTaskIdentifier);
        }

        private void BindTasksInProgram(Guid? completionTaskIdentifier)
        {
            var taskObjectData = ProgramHelper.GetTaskObjectData(CurrentSessionState.Identity.Organization.Identifier, CurrentSessionState.Identity.Organization.ParentOrganizationIdentifier);
            var tasks = ProgramSearch1.GetProgramTasks(new TTaskFilter
            {
                ProgramIdentifier = ProgramIdentifier,
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                ExcludeObjectTypes = new string[] { "Assessment" }
            });

            var taskInfoContainer = ProgramHelper.BindTaskInfo(taskObjectData, tasks);

            TaskInProgram.LoadItems(taskInfoContainer.OrderBy(x => x.Type), "TaskIdentifier", "DisplayTitle");
            TaskInProgram.ValueAsGuid = completionTaskIdentifier;
        }
    }
}