using System;

using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class OutputDetails : BaseUserControl
    {
        private Guid MessageIdentifier
        {
            get { return (Guid)ViewState[nameof(MessageIdentifier)]; }
            set { ViewState[nameof(MessageIdentifier)] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DisableMessageButton.Click += (x, y) => ChangeMessageStatus(true);
            EnableMessageButton.Click += (x, y) => ChangeMessageStatus(false);

            DisableAutoBccSubscribers.Click += (x, y) => ChangeAutoBccSubscribers(false);
            EnableAutoBccSubscribers.Click += (x, y) => ChangeAutoBccSubscribers(true);
        }

        public void SetOutputValues(VMessage message, bool showEditLinks)
        {
            MessageIdentifier = message.MessageIdentifier;

            MessageType.Text = message.MessageType;

            var isNotification = message.MessageType == MessageTypeName.Notification || message.MessageType == MessageTypeName.Alert;
            var isSurveyInvitation = message.MessageType == MessageTypeName.Invitation;
            var isSystemNotification = message.MessageType == MessageTypeName.Alert;

            SurveyField.Visible = isSurveyInvitation;

            if (isSurveyInvitation)
            {
                var survey = message.SurveyFormIdentifier.HasValue
                    ? ServiceLocator.SurveySearch.GetSurveyForm(message.SurveyFormIdentifier.Value)
                    : null;

                SurveyName.Text = survey != null
                    ? $"<a href=\"/ui/admin/workflow/forms/outline?form={survey.SurveyFormIdentifier}\">{survey.SurveyFormName}</a>"
                    : "(Undefined)";
            }

            Name.Text = message.MessageName;
            NameField.Visible = !isSystemNotification;
            NameEditLink.Visible = showEditLinks && !isNotification;
            NameEditLink.NavigateUrl = $"/ui/admin/messages/rename?message={message.MessageIdentifier}";

            NotificationField.Visible = isSystemNotification;

            if (isSystemNotification)
            {
                var notificationType = message.MessageName.ToEnumNullable<NotificationType>();

                NotificationName.Text = message.MessageName;
                NotificationDescription.InnerHtml = notificationType.HasValue
                    ? MessageFormHelper.GetNotificationDescription(notificationType.Value)
                    : "<strong class='text-danger'>Invalid system notification name</strong>";
            }

            Subject.Text = message.MessageTitle;
            SubjectEditLink.Visible = showEditLinks;
            SubjectEditLink.NavigateUrl = $"/ui/admin/messages/retitle?message={message.MessageIdentifier}";

            MessageIdentifierOutput.Text = message.MessageIdentifier.ToString();

            SetupStatusField(message, showEditLinks);
            SetupAutoBccSubscribers(message, showEditLinks);

            Sender.Text = message.SenderEmail.IsEmpty()
                ? "<i>Sender Not Found</i>"
                : Identity.IsGranted(PermissionIdentifiers.Admin_Accounts)
                    ? $"<a href=\"/ui/admin/accounts/senders/edit?id={message.SenderIdentifier}\">{message.SenderName}</a> &lt;<a href =\"mailto:{message.SenderEmail.ToLower()}\">{message.SenderEmail.ToLower()}</a>&gt;"
                    : $"{message.SenderName}";

            SenderEditLink.Visible = showEditLinks;
            SenderEditLink.NavigateUrl = $"/ui/admin/messages/change-sender?message={message.MessageIdentifier}";
        }

        private void SetupStatusField(VMessage message, bool showEditLinks)
        {
            if (message.IsDisabled)
            {
                MessageStatus.Text = "Disabled";
                MessageStatusDescription.InnerText = "Mailouts for this message are disabled. Scheduled deliveries are cancelled automatically.";
            }
            else
            {
                MessageStatus.Text = "Enabled";
                MessageStatusDescription.InnerText = "Mailouts for this message can be scheduled normally.";
            }

            DisableMessageButton.Visible = showEditLinks && !message.IsDisabled;
            EnableMessageButton.Visible = showEditLinks && message.IsDisabled;
        }

        private void SetupAutoBccSubscribers(VMessage message, bool showEditLinks)
        {
            if (StringHelper.EqualsAny(message.MessageType, new[] { "Invitation", "Newsletter" }))
            {
                AutoBccSubscribersPanel.Visible = false;
                return;
            }

            if (message.AutoBccSubscribers)
            {
                AutoBccSubscribers.Text = "Enabled";
            }
            else
            {
                AutoBccSubscribers.Text = "Disabled";
            }

            DisableAutoBccSubscribers.Visible = showEditLinks && message.AutoBccSubscribers;
            EnableAutoBccSubscribers.Visible = showEditLinks && !message.AutoBccSubscribers;
        }

        private void ChangeMessageStatus(bool disabled)
        {
            if (disabled)
                ServiceLocator.SendCommand(new DisableMessage(MessageIdentifier));
            else
                ServiceLocator.SendCommand(new EnableMessage(MessageIdentifier));

            var message = ServiceLocator.MessageSearch.GetMessage(MessageIdentifier);

            SetupStatusField(message, true);
        }

        private void ChangeAutoBccSubscribers(bool enable)
        {
            if (enable)
                ServiceLocator.SendCommand(new EnableAutoBccSubscribers(MessageIdentifier)); 
            else
                ServiceLocator.SendCommand(new DisableAutoBccSubscribers(MessageIdentifier));

            var message = ServiceLocator.MessageSearch.GetMessage(MessageIdentifier);

            SetupAutoBccSubscribers(message, true);
        }
    }
}