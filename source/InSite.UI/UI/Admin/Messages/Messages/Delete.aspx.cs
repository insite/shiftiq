using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Messages.Messages
{
    public partial class DeleteForm : AdminBasePage, IHasParentLinkParameters
    {
        private Guid MessageID => Guid.TryParse(Request["message"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                BindModelToControls();
        }

        protected void BindModelToControls()
        {
            if (!IsPostBack)
            {
                var message = ServiceLocator.MessageSearch.GetMessage(MessageID);
                if (message == null)
                    RedirectToSearch();

                PageHelper.AutoBindHeader(this, null, message.MessageTitle);

                SetMessageValues(message);

                var counters = ServiceLocator.MessageSearch.CountMessageReferences(message.MessageIdentifier);
                var list = new List<Counter>();
                foreach (var counter in counters.Where(x => x.Value > 0))
                    list.Add(new Counter { Value = counter.Value, Name = FormatCounterName(counter.Name) });

                ReferenceRepeater.DataSource = list;
                ReferenceRepeater.DataBind();

                var externalReferenceCount = list.Count(x => !x.Name.StartsWith("Messages"));
                if (externalReferenceCount > 0 && !Identity.IsOperator)
                {
                    ConfirmAlert.InnerHtml = $"<i class='fas fa-stop-circle'></i> <strong>Stop:</strong> You cannot delete this message without first removing or replacing {Shift.Common.Humanizer.ToQuantity(externalReferenceCount, "reference")} to it.";
                    DeleteButton.Visible = false;
                }

                CancelButton.NavigateUrl = GetOutlineUrl(MessageID);
            }
        }

        private void SetMessageValues(VMessage message)
        {
            MessageType.Text = message.MessageType;

            var isSurveyInvitation = message.MessageType == MessageTypeName.Invitation;
            var isSystemNotification = message.MessageType == MessageTypeName.Alert;

            var formName = "(Undefined)";
            var formId = message.SurveyFormIdentifier;
            if (formId.HasValue)
            {
                var survey = ServiceLocator.SurveySearch.GetSurveyForm(formId.Value);
                if (survey != null)
                    formName = $"<a target=_blank href=\"/ui/admin/workflow/forms/outline?form={message.SurveyFormIdentifier}\">{survey.SurveyFormName}</a>";
            }

            SurveyFieldHeader.Visible = isSurveyInvitation;
            SurveyFieldValue.Visible = isSurveyInvitation;
            SurveyName.Text = formName;

            Name.Text = message.MessageName;
            NameFieldHeader.Visible = !isSystemNotification;
            NameFieldValue.Visible = !isSystemNotification;

            NotificationFieldHeader.Visible = isSystemNotification;
            NotificationFieldValue.Visible = isSystemNotification;

            Subject.Text = $"<a target=_blank href=\"{GetOutlineUrl(message.MessageIdentifier)}\">{message.MessageTitle}</a>";
            MessageStatus.Text = message.IsDisabled ? "Disabled" : "Enabled";

            Sender.Text = message.SenderEmail.IsEmpty()
                ? "<i>Sender Not Found</i>"
                : Identity.IsGranted(PermissionIdentifiers.Admin_Accounts)
                    ? $"<a target=_blank href=\"/ui/admin/accounts/senders/edit?id={message.SenderIdentifier}\">{message.SenderName}</a> &lt;<a href =\"mailto:{message.SenderEmail.ToLower()}\">{message.SenderEmail.ToLower()}</a>&gt;"
                    : $"{message.SenderName}";
        }

        private string FormatCounterName(string name)
        {
            var parts = StringHelper.Split(name, '/');
            return parts[parts.Length - 1];
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new ArchiveMessage(MessageID));

            RedirectToSearch();
        }

        private static void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/messages/messages/search", true);

        private string GetOutlineUrl(Guid messageId) =>
            $"/ui/admin/messages/outline?message={messageId}&tab=message";

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"message={MessageID}"
                : null;
        }
    }
}