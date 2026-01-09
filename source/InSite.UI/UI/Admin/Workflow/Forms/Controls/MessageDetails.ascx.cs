using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Messages.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Messages.Messages.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class MessageDetails : UserControl
    {
        private Guid MessageIdentifier
        {
            get => (Guid)ViewState[nameof(MessageIdentifier)];
            set => ViewState[nameof(MessageIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EditContentButton.Click += EditContentButton_Click;
        }

        public void SetInputValues(VMessage message, bool isInvitation, Guid surveyFormIdentifier, bool canEdit, bool isNotification = false, SurveyMessageType surveyType = SurveyMessageType.Undefined) // add type of response
        {
            BindButtons(message, isInvitation, surveyFormIdentifier, canEdit, isNotification, surveyType);
            BindMessageDetails(message);
            BindMessageDetailsPencilLinks(message, surveyFormIdentifier, surveyType, isInvitation);
        }

        private void BindButtons(VMessage message, bool isInvitation, Guid surveyFormIdentifier, bool canEdit, bool isNotification, SurveyMessageType surveyType)
        {
            ChangeMessageButton.NavigateUrl = $"/ui/admin/workflow/forms/configure-workflow?form={surveyFormIdentifier}";

            if (this.Parent != null)
            {
                var parentTab = this.Parent as NavItem;

                if (parentTab != null)
                    ChangeMessageButton.NavigateUrl = $"/ui/admin/workflow/forms/configure-workflow?form={surveyFormIdentifier}&returnpanel=messages&returntab={GetMessagePanelTabValue(surveyType, isInvitation)}";
            }

            ChangeMessageButton.Visible = canEdit;

            AddInvitationButton.Visible = NoInvitationMessage.Visible = isInvitation && message == null && canEdit;
            if (AddInvitationButton.Visible)
            {
                AddInvitationButton.NavigateUrl = new ReturnUrl($"form={surveyFormIdentifier}&panel=messages&Tab=Invitation")
                    .GetRedirectUrl($"/ui/admin/messages/create?form={surveyFormIdentifier}&invitation=1");
            }

            AddNotificationButton.Visible = isNotification && message == null && canEdit && surveyType != SurveyMessageType.Undefined;
            if (AddNotificationButton.Visible)
            {

                if (surveyType == SurveyMessageType.ResponseStarted)
                    AddNotificationButton.NavigateUrl = new ReturnUrl($"form={surveyFormIdentifier}&panel=messages&Tab=ResponseStartedAdministrator")
                        .GetRedirectUrl($"/ui/admin/messages/create?form={surveyFormIdentifier}&type=Notification&surveyType={surveyType}&notification=1");
                else if (surveyType == SurveyMessageType.ResponseCompleted)
                    AddNotificationButton.NavigateUrl = new ReturnUrl($"form={surveyFormIdentifier}&panel=messages&Tab=ResponseCompletedAdministrator")
                        .GetRedirectUrl($"/ui/admin/messages/create?form={surveyFormIdentifier}&type=Notification&surveyType={surveyType}&notification=1");
                else if (surveyType == SurveyMessageType.ResponseConfirmed)
                    AddNotificationButton.NavigateUrl = new ReturnUrl($"form={surveyFormIdentifier}&panel=messages&Tab=ResponseCompletedRespondent")
                        .GetRedirectUrl($"/ui/admin/messages/create?form={surveyFormIdentifier}&type=Notification&surveyType={surveyType}&notification=1");
            }
        }

        private void BindMessageDetails(VMessage message)
        {
            Field.Visible = message != null;
            if (!Field.Visible)
                return;

            MessageIdentifier = message.MessageIdentifier;

            var qMessage = ServiceLocator.MessageSearch.GetQMessage(message.MessageIdentifier);
            if (qMessage != null)
                MessageModified.Text = qMessage.LastChangeTime.Format(CurrentSessionState.Identity.User.TimeZone, true);

            MessageSubject.Text = $"{message.MessageTitle}";

            MessageSender.Text = !string.IsNullOrEmpty(message.SenderEmail)
                ? $"{message.SenderName} &lt;<a href=\"mailto:{message.SenderEmail.ToLower()}\">{message.SenderEmail.ToLower()}</a>&gt;"
                : "None";

            MessageRecipients.Text = GetRecipientsString(message);
            MessageMailoutCount.Text = message.MailoutCount?.ToString("n0");
            MessageContent.InnerText = MessageHelper.BuildPreviewHtml(CurrentSessionState.Identity.Organization.OrganizationIdentifier, message.SenderIdentifier, GetSurveyFormAsset(message.SurveyFormIdentifier), message.ContentText);
            LabelMessageContent.Visible = true;

            if (message.ContentText.IsEmpty())
            {
                ContentEmpty.AddMessage(AlertType.Warning, "Please click <b>Edit</b> to create content for this message.");
                EditContentButton.Visible = true;
            }
        }

        private void BindMessageDetailsPencilLinks(VMessage message, Guid surveyFormIdentifier, SurveyMessageType surveyType, bool isInvitation)
        { 
            if (message == null) 
                return;

            MessageLink.NavigateUrl = new ReturnUrl($"form={surveyFormIdentifier}&panel=messages&Tab={GetMessagePanelTabValue(surveyType, isInvitation)}")
                        .GetRedirectUrl($"/ui/admin/messages/retitle?message={message.MessageIdentifier}");
            MessageSubjectLink.NavigateUrl = new ReturnUrl($"form={surveyFormIdentifier}&panel=messages&Tab={GetMessagePanelTabValue(surveyType, isInvitation)}")
                        .GetRedirectUrl($"/ui/admin/messages/retitle?message={message.MessageIdentifier}");
            MessageSenderLink.NavigateUrl = new ReturnUrl($"form={surveyFormIdentifier}&panel=messages&Tab={GetMessagePanelTabValue(surveyType, isInvitation)}")
                        .GetRedirectUrl($"/ui/admin/messages/change-sender?message={message.MessageIdentifier}");
            MessageRecipientsLink.NavigateUrl = new ReturnUrl($"form={surveyFormIdentifier}&panel=messages&Tab={GetMessagePanelTabValue(surveyType, isInvitation)}")
                        .GetRedirectUrl($"/ui/admin/messages/subscribers/add?message={message.MessageIdentifier}");
            MessageContentLink.NavigateUrl = new ReturnUrl($"form={surveyFormIdentifier}&panel=messages&Tab={GetMessagePanelTabValue(surveyType, isInvitation)}")
                        .GetRedirectUrl($"{GetEditCOntenetUrl()}");
        }

        private static string GetMessagePanelTabValue(SurveyMessageType surveyType, bool isInvitation)
        {
            switch (surveyType)
            {
                case SurveyMessageType.Invitation:
                    return "Invitation";
                case SurveyMessageType.ResponseStarted:
                    return "ResponseStartedAdministrator";
                case SurveyMessageType.ResponseCompleted:
                    return "ResponseCompletedAdministrator";
                case SurveyMessageType.ResponseConfirmed:
                    return "ResponseCompletedRespondent";
            }

            return isInvitation ? "Invitation" : null;
        }

        private void EditContentButton_Click(object sender, EventArgs e)
            => ScriptManager.RegisterStartupScript(Page, typeof(MessageDetails), "open_editor", $"window.location.href = '{GetEditCOntenetUrl()}';", true);
        

        private string GetEditCOntenetUrl()
        {
            var session = ContentSessionStorage.Create(MessageIdentifier, CurrentSessionState.Identity.User.UserIdentifier, CurrentSessionState.Identity.Organization.CompanyName);
            return $"/ui/admin/messages/content?id={session.SessionIdentifier}";
        }

        private string GetRecipientsString(VMessage message)
        {
            if (message != null && message.SubscriberUserCount.HasValue && message.SubscriberUserCount <= 3)
            {
                var subscribers = ServiceLocator.MessageSearch.GetSubscriberUsers(message.MessageIdentifier);

                if (subscribers.Count == 0) return "None";

                return string.Join(", ", subscribers.Select(x => $"<a href=\"/ui/admin/contacts/people/edit?contact={x.UserIdentifier}\">{x.UserFullName}</a>"));
            }

            return message.SubscriberUserCount?.ToString("n0");
        }

        public static int? GetSurveyFormAsset(Guid? id)
        {
            var survey2 = id.HasValue ? ServiceLocator.SurveySearch.GetSurveyForm(id.Value) : null;

            return survey2?.AssetNumber;
        }
    }
}