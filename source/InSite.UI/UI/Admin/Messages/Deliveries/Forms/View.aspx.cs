using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Messages.Deliveries.Forms
{
    public partial class View : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? MailoutIdentifier => Guid.TryParse(Request.QueryString["mailout"], out var value) ? value : (Guid?)null;

        private const string HomeUrl = "/ui/admin/messages/home";

        private string ReturnUrl
        {
            get => (string)ViewState[nameof(ReturnUrl)];
            set => ViewState[nameof(ReturnUrl)] = value;
        }

        private Guid? MessageIdentifier
        {
            get => (Guid?)ViewState[nameof(MessageIdentifier)];
            set => ViewState[nameof(MessageIdentifier)] = value;
        }

        protected string DeliveryContent
        {
            get => (string)ViewState[nameof(DeliveryContent)];
            set => ViewState[nameof(DeliveryContent)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            ReturnUrl = GetReturnUrl().IfNullOrEmpty("/ui/admin/messages/reports/mailout-summary?mailout=" + MailoutIdentifier);

            Open();

            CancelButton.NavigateUrl = ReturnUrl;
        }

        private void Open()
        {
            if (!MailoutIdentifier.HasValue)
                HttpResponseHelper.Redirect(HomeUrl);

            var recipient = Request.QueryString["recipient"];
            if (recipient.IsEmpty())
                HttpResponseHelper.Redirect(ReturnUrl);

            var mailout = ServiceLocator.MessageSearch.FindMailout(MailoutIdentifier.Value);
            if (mailout == null /*|| mailout.TenantIdentifier != Tenant.Identifier*/)
                HttpResponseHelper.Redirect(HomeUrl);

            var delivery = Guid.TryParse(recipient, out var recipientId)
                ? ServiceLocator.MessageSearch.GetDelivery(mailout.MailoutIdentifier, recipientId)
                : ServiceLocator.MessageSearch.GetDelivery(mailout.MailoutIdentifier, recipient);
            if (delivery == null)
                HttpResponseHelper.Redirect(ReturnUrl);

            MessageIdentifier = mailout.MessageIdentifier;

            var job = DeliveryAdapter.CreateEmailJobs(mailout, new[] { delivery }).First();

            if (job.Email.ContentSubject.IsEmpty)
                job.Email.ContentSubject.Default = mailout.ContentSubject;

            if (job.Email.ContentBody.IsEmpty)
                job.Email.ContentBody.Default = mailout.ContentBodyHtml;

            var compiledMessage = MessageHelper.BuildMessage(job.Email, job.Language);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: compiledMessage.Subject);

            if (mailout.MessageIdentifier.HasValue)
            {
                var message = ServiceLocator.MessageSearch.GetMessage(mailout.MessageIdentifier.Value);
                if (message != null)
                {
                    var sender = TSenderSearch.Select(message.SenderIdentifier);
                    var senderEmail = sender.SenderEmail.ToLower();

                    MessageSenderOutput.InnerHtml = $"<a href=\"/ui/admin/contacts/people/edit?contact={message.SenderIdentifier}\">{sender.SenderName}</a> <<a href =\"mailto:{HttpUtility.HtmlEncode(senderEmail)}\">{HttpUtility.HtmlEncode(senderEmail)}</a>>";
                    MessageSubjectOutput.InnerText = compiledMessage.Subject;
                    MessageNameOutput.InnerHtml = $"<a href=\"/ui/admin/messages/outline?message={message.MessageIdentifier}\">{HttpUtility.HtmlEncode(message.MessageName)}</a>";
                }
            }

            var timezone = User.TimeZone;

            MailoutIdentifierOutput.InnerText = mailout.MailoutIdentifier.ToString();
            MailoutScheduledOutput.InnerText = mailout.MailoutScheduled.Format(timezone);
            MailoutCompletedOutput.InnerText = mailout.MailoutCompleted.Format(timezone);
            MailoutCompletedField.Visible = mailout.MailoutCompleted.HasValue;
            MailoutCancelledOutput.InnerText = mailout.MailoutCancelled.Format(timezone);
            MailoutCancelledField.Visible = mailout.MailoutCancelled.HasValue;

            DeliveryRecipientAddressOutput.InnerHtml = $"<a href=\"mailto:{HttpUtility.HtmlEncode(delivery.UserEmail.ToLower())}\">{HttpUtility.HtmlEncode(delivery.UserEmail.ToLower())}</a>";
            DeliveryRecipientNameOutput.InnerHtml = $"<a href=\"/ui/admin/contacts/people/edit?contact={delivery.RecipientIdentifier}\">{HttpUtility.HtmlEncode(delivery.PersonName)}</a>";
            DeliveryStatusOutput.InnerText = delivery.DeliveryStatus;
            DeliveryStartedOutput.InnerText = delivery.DeliveryStarted.Format(timezone);
            DeliveryCompletedOutput.InnerText = delivery.DeliveryCompleted.Format(timezone);

            SetEmailOutput(job.Email);
        }

        private void SetEmailOutput(EmailDraft email)
        {
            var recipient = email.Recipients.FirstOrDefault();

            var variables = new Dictionary<string, string>(email.ContentVariables);
            foreach (var kv in recipient.Variables)
                variables[kv.Key] = kv.Value;

            var envelope = new EmailVariables(recipient.Identifier.Value, recipient.Address, email.OrganizationIdentifier, variables);

            var subject = email.ContentSubject.Default;
            subject = MessageHelper.ReplacePlaceholdersForMailgun(email.OrganizationIdentifier, email.SenderIdentifier, email.SurveyNumber, subject, envelope);

            var body = email.ContentBody.Default;
            body = MessageHelper.ReplacePlaceholdersForMailgun(email.OrganizationIdentifier, email.SenderIdentifier, email.SurveyNumber, body, envelope);
            body = MessageHelper.CreateHtmlBody(subject, body, false);

            ContentSubjectOutput.InnerText = subject;
            DeliveryContent = body;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/mailout-summary"))
            {
                return $"mailout={MailoutIdentifier}";
            }
            else if (parent.Name.EndsWith("/outline"))
            {
                if (MessageIdentifier.HasValue)
                    return $"message={MessageIdentifier.Value}";
            }

            return null;
        }
    }
}
