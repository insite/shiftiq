using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Events.Classes.Controls
{
    public partial class ClassNotificationTab : BaseUserControl
    {
        private Guid EventId
        {
            get => (Guid)ViewState[nameof(EventId)];
            set => ViewState[nameof(EventId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TriggerButton.Click += TriggerButton_Click;
        }

        private void TriggerButton_Click(object sender, EventArgs e)
        {
            if (!Identity.IsOperator)
                return;

            var classReminder = new ClassReminder(
                ServiceLocator.EventSearch,
                ServiceLocator.RegistrationSearch,
                ServiceLocator.RecordSearch,
                ServiceLocator.GroupSearch,
                new Commander(),
                ServiceLocator.AlertMailer,
                (Guid organizationId) => OrganizationSearch.Select(organizationId),
                ServiceLocator.AppSettings
            );

            var count = classReminder.CreateNotifications(EventId, IgnoreScheduleStart.SelectedValue == "Yes");
            var now = TimeZones.Format(DateTimeOffset.UtcNow, User.TimeZone);

            var @event = ServiceLocator.EventSearch.GetEvent(EventId);

            LoadData(@event, ToLearnerLink.Visible);

            TriggerAlert.AddMessage(AlertType.Information, $"Notifications successfully triggered at {now}.<br><b>{count}</b> notification(s) have been scheduled.");
        }

        public void LoadData(QEvent @event, bool canEdit)
        {
            EventId = @event.EventIdentifier;

            ReminderLearnerMessage.Text = GetMessageName(@event.WhenEventReminderRequestedNotifyLearnerMessageIdentifier);
            ReminderInstructorMessage.Text = GetMessageName(@event.WhenEventReminderRequestedNotifyInstructorMessageIdentifier);
            SendReminderBeforeDays.Text = @event.SendReminderBeforeDays.HasValue ? $"{@event.SendReminderBeforeDays:n0}" : "None";

            ReminderMessageSent.Text = @event.ReminderMessageSent.HasValue ? TimeZones.Format(@event.ReminderMessageSent.Value, User.TimeZone) : "Never";

            var changeUrl = $"/ui/admin/events/classes/modify-notification?event={@event.EventIdentifier}";

            ToLearnerLink.NavigateUrl = changeUrl;
            ToInstructorsLink.NavigateUrl = changeUrl;
            SendReminderBeforeDaysLink.NavigateUrl = changeUrl;

            ToLearnerLink.Visible = canEdit;
            ToInstructorsLink.Visible = canEdit;
            SendReminderBeforeDaysLink.Visible = canEdit;

            TestPanel.Visible = Identity.IsOperator;
        }

        private static string GetMessageName(Guid? messageId)
        {
            var message = messageId.HasValue
                ? ServiceLocator.MessageSearch.GetMessage(messageId.Value)
                : null;

            return message != null
                ? $"<a href='/ui/admin/messages/outline?message={messageId}'>{message.MessageTitle}</a>"
                : "None";
        }
    }
}