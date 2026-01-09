using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Programs
{
    public partial class ModifyNotification : AdminBasePage, IHasParentLinkParameters
    {
        public const string NavigateUrl = "/ui/admin/learning/programs/modify-notification";

        public static string GetNavigateUrl(Guid programId) => NavigateUrl + "?id=" + programId;

        public static void Redirect(Guid programId) => HttpResponseHelper.Redirect(GetNavigateUrl(programId));

        private Guid? ProgramId => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        #region Methods (open)

        private void Open()
        {
            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
                Search.Redirect();

            PageHelper.AutoBindHeader(this, null, program.ProgramName);

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

            BindTasksInProgram(program);

            CancelButton.NavigateUrl = Outline.GetNavigateUrl(ProgramId.Value, tab: "notification");
        }

        private void BindTasksInProgram(TProgram program)
        {
            var organization = CurrentSessionState.Identity.Organization;

            var taskObjectData = ProgramHelper.GetTaskObjectData(organization.Identifier, organization.ParentOrganizationIdentifier);

            var filter = new TTaskFilter
            {
                ProgramIdentifier = program.ProgramIdentifier,
                ExcludeObjectTypes = new string[] { "Assessment" }
            };

            filter.OrganizationIdentifiers.Add(organization.Identifier);

            var tasks = ProgramSearch1.GetProgramTasks(filter);

            var taskInfoContainer = ProgramHelper.BindTaskInfo(taskObjectData, tasks);

            TaskInProgram.LoadItems(taskInfoContainer.OrderBy(x => x.Type), "TaskIdentifier", "DisplayTitle");
            TaskInProgram.ValueAsGuid = program.CompletionTaskIdentifier;
        }

        #endregion

        #region Methods (save)

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
                return;

            program.NotificationStalledLearnerMessageIdentifier = NotificationStalledLearnerMessageIdentifier.Value;
            program.NotificationStalledAdministratorMessageIdentifier = NotificationStalledAdministratorMessageIdentifier.Value;
            program.NotificationCompletedLearnerMessageIdentifier = NotificationCompletedLearnerMessageIdentifier.Value;
            program.NotificationCompletedAdministratorMessageIdentifier = NotificationCompletedAdministratorMessageIdentifier.Value;
            program.NotificationStalledTriggerDay = NotificationStalledTriggerDay.ValueAsInt;
            program.NotificationStalledReminderLimit = NotificationStalledReminderLimit.ValueAsInt;
            program.CompletionTaskIdentifier = TaskInProgram.ValueAsGuid;

            ProgramStore.Update(program, CurrentSessionState.Identity.User.UserIdentifier);

            Outline.Redirect(program.ProgramIdentifier, tab: "notification");
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramId}"
                : null;
        }

        #endregion
    }
}