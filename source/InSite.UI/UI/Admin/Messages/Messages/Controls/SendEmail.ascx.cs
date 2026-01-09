using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Admin.Messages.Messages.Models;
using InSite.UI.Admin.Messages.Messages.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Messages.Messages.Controls
{
    public partial class SendEmail : BaseUserControl
    {
        [Serializable]
        private class EmailTemplate
        {
            public Guid MessageIdentifier { get; set; }
            public string ContentText { get; set; }
            public string MessageTitle { get; set; }
        }

        private SendEmailModel Model
        {
            get => (SendEmailModel)ViewState[nameof(Model)];
            set => ViewState[nameof(Model)] = value;
        }

        private List<EmailTemplate> Templates
        {
            get => (List<EmailTemplate>)ViewState[nameof(Templates)];
            set => ViewState[nameof(Templates)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MessageTemplate.AutoPostBack = true;
            MessageTemplate.ValueChanged += (s, a) => OnTemplateChanged();

            MessageSendButton.Click += (s, a) => OnSend();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            MessageScheduleDate.Value = DateTimeOffset.Now;
            MessageUpload.FolderPath = OrganizationRelativePath.EventRegistrationsBuildMessagePathTemplate
                .Format(User.Identifier, RandomStringGenerator.Create(RandomStringType.Alphanumeric, 6));

            BindMessageTemplate(Identity.Language);
        }

        public void BindModelToControls(string recipientType, IEnumerable<Guid> recipientIdentifiers)
        {
            Model = new SendEmailModel(Organization.Identifier, recipientType, recipientIdentifiers);

            var hasRecipients = Model.RecipientCount > 0;

            MessageSendButton.Enabled = hasRecipients;

            if (!hasRecipients)
                return;

            MessageRecipients.Text = Model.RecipientCount < 20
                ? string.Join("; ", Model.GetRecipients().Select(x => $"{x.FullName} <span class='text-body-secondary'>&lt;{x.Email}&gt;</span>"))
                : Model.GetRecipientCount();
            MessageSendButton.ConfirmText = Model.GetConfirmText();
        }

        private void BindMessageTemplate(string language)
        {
            var isChangeLang = language.IsNotEmpty()
                && language != MultilingualString.DefaultLanguage;

            Templates = ServiceLocator.MessageSearch
                .GetMessages(new MessageFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    Type = MessageTypeName.Newsletter,
                    IsDisabled = false
                })
                .Select(x => new EmailTemplate
                {
                    MessageIdentifier = x.MessageIdentifier,
                    ContentText = x.ContentText,
                    MessageTitle = x.MessageTitle,
                })
                .ToList();

            MessageTemplate.Items.Clear();
            MessageTemplate.Items.Add(new ComboBoxOption());

            foreach (var item in Templates)
            {
                if (isChangeLang)
                {
                    var session = ContentSessionStorage.Create(item.MessageIdentifier, User.UserIdentifier, Organization.CompanyName);
                    var content = session?.MarkdownText?.Get(language);

                    if (content.IsNotEmpty())
                        item.ContentText = content;
                }

                MessageTemplate.Items.Add(new ComboBoxOption(item.MessageTitle,
                    item.MessageIdentifier.ToString()));
            }
        }

        private void OnTemplateChanged()
        {
            var templateId = MessageTemplate.ValueAsGuid;
            var template = Templates.FirstOrDefault(x => x.MessageIdentifier == templateId);
            if (template == null)
                return;

            MessageSubject.Text = template.MessageTitle;
            MessageBody.Value = template.ContentText;
        }

        private void OnSend()
        {
            if (!ValidateMessage())
                return;

            var sender = TSenderSearch.Select(MessageSender.ValueAsGuid.Value);
            var recipients = Model.GetRecipients();

            foreach (var recipient in recipients)
                DraftAndSendEmail(sender, recipient);

            MessageStatus.AddMessage(AlertType.Success, Model.GetSuccessText());
        }

        private void DraftAndSendEmail(TSender sender, SendEmailModel.RecipientInfo recipient)
        {
            var email = EmailDraft.Create(
                Organization.OrganizationIdentifier,
                null,
                sender.SenderIdentifier,
                false
                );

            email.MailoutIdentifier = UniqueIdentifier.Create();
            email.UserIdentifier = User.Identifier;

            email.SenderEmail = sender.SenderEmail;
            email.SenderName = sender.SenderName;
            email.SenderEnabled = sender.SenderEnabled;
            email.SenderType = sender.SenderType;
            email.SystemMailbox = sender.SystemMailbox;

            email.MailoutScheduled = MessageScheduleDate.Value;

            email.RecipientListTo.Add(recipient.UserIdentifier, recipient.Email);
            email.Recipients.Add(new EmailAddress(recipient.UserIdentifier, recipient.Email, recipient.FullName, recipient.PersonCode, recipient.Language)
            {
                Variables =
                {
                    ["FirstName"] = recipient.FirstName,
                    ["LastName"] = recipient.LastName
                }
            });

            email.ContentSubject.Default = MessageSubject.Text;
            email.ContentBody.Default = MessageBody.Value;
            email.ContentVariables["FirstName"] = recipient.FirstName;
            email.ContentVariables["RecipientNameFirst"] = recipient.FirstName;
            email.ContentVariables["LastName"] = recipient.LastName;
            email.ContentVariables["RecipientNameLast"] = recipient.LastName;
            email.ContentVariables["FullName"] = recipient.FullName;
            email.ContentVariables["RecipientName"] = recipient.FullName;
            email.ContentVariables["Email"] = recipient.Email;
            email.ContentVariables["RecipientEmail"] = recipient.Email;
            email.ContentVariables["RecipientCode"] = recipient.PersonCode;
            email.ContentVariables["CompanyTitle"] = Organization.CompanyName;
            email.ContentVariables["SignInUrl"] = HttpRequestHelper.CurrentRootUrl + "/ui/lobby/signin";

            var message = MessageHelper.BuildMessage(email, MultilingualString.DefaultLanguage);

            email.ContentSubject.Default = message.Subject;
            email.ContentBody.Default = message.Body;

            ServiceLocator.EmailOutbox.SendAndReplacePlaceholders(email, Model.RecipientType);
        }

        private bool ValidateMessage()
        {
            MessageStatus.Clear();

            if (MessageScheduleDate.Value == null)
                MessageStatus.AddMessage(AlertType.Error, "Select a date and time to deliver the message.");

            else if (MessageScheduleDate.Value > DateTimeOffset.UtcNow.AddDays(3))
                MessageStatus.AddMessage(AlertType.Error, "Messages can be scheduled for delivery a maximum of 3 days (72 hours) in the future.");

            if (MessageSender.ValueAsGuid == null || MessageSender.ValueAsGuid.Value == Guid.Empty)
                MessageStatus.AddMessage(AlertType.Error, "Select a sender account to deliver the message.");

            if (string.IsNullOrEmpty(MessageSubject.Text))
                MessageStatus.AddMessage(AlertType.Error, "Your message must have a Subject.");

            if (string.IsNullOrEmpty(MessageBody.Value))
                MessageStatus.AddMessage(AlertType.Error, "Your message must have a Body.");

            return !MessageStatus.HasMessage;
        }
    }
}