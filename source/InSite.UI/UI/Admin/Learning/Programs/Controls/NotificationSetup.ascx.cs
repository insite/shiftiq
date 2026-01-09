using System;
using System.Web;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;

using Shift.Common;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class NotificationSetup : BaseUserControl
    {
        public void LoadData(TProgram program)
        {
            NotificationStalledLearnerMessageName.Text = GetMessageName(program.NotificationStalledLearnerMessageIdentifier);
            NotificationStalledAdministratorMessageName.Text = GetMessageName(program.NotificationStalledAdministratorMessageIdentifier);
            NotificationCompletedLearnerMessageName.Text = GetMessageName(program.NotificationCompletedLearnerMessageIdentifier);
            NotificationCompletedAdministratorMessageName.Text = GetMessageName(program.NotificationCompletedAdministratorMessageIdentifier);

            NotificationStalledTriggerDay.Text = GetIntValue(program.NotificationStalledTriggerDay);
            NotificationStalledReminderLimit.Text = GetIntValue(program.NotificationStalledReminderLimit);

            var task = program.CompletionTaskIdentifier.HasValue
                ? ProgramSearch1.GetProgramTask(program.CompletionTaskIdentifier.Value)
                : null;

            if (task != null)
            {
                var taskObjectData = ProgramHelper.GetTaskObjectData(
                    Organization.OrganizationIdentifier,
                    Organization.ParentOrganizationIdentifier);
                var taskInfo = ProgramHelper.GetTaskInfo(taskObjectData, task);

                TaskInProgram.Text = GetEntityName(taskInfo.DisplayTitle);
            }
            else
            {
                TaskInProgram.Text = GetEntityName(null);
            }

            var editUrl = ModifyNotification.GetNavigateUrl(program.ProgramIdentifier);
            EditLink1.NavigateUrl = editUrl;
            EditLink2.NavigateUrl = editUrl;

            string GetMessageName(Guid? id)
            {
                var result = id.HasValue
                    ? ServiceLocator.MessageSearch.GetMessage(id.Value)?.MessageTitle
                    : null;

                return GetEntityName(result);
            }

            string GetEntityName(string value)
            {
                return value.IsNotEmpty() ? HttpUtility.HtmlEncode(value) : "<i>None</i>";
            }

            string GetIntValue(int? value)
            {
                return value.HasValue ? value.Value.ToString("n0") : "<i>None</i>";
            }
        }
    }
}