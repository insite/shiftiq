using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Admin.Messages.Messages.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Contacts.People.Forms
{
    public partial class SendEmail : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private enum EmailType
        {
            Unknown, Welcome, Correspondence
        }

        [Serializable]
        private class EmailInfo
        {
            public Guid? MessageIdentifier { get; set; }

            public bool AutoBccSubscribers { get; set; }

            public RecipientInfo Recipient { get; set; }
            public SenderInfo Sender { get; set; }

            public EmailType EmailType { get; set; }
            public string EmailSubject { get; set; }
            public string EmailBody { get; set; }

            public Dictionary<string, string> Variables { get; } = new Dictionary<string, string>();
        }

        [Serializable]
        private class RecipientInfo
        {
            public Guid UserIdentifier { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string PersonCode { get; set; }
            public string Language { get; set; }
        }

        [Serializable]
        private class SenderInfo
        {
            public Guid Identifier { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Type { get; set; }
            public string SystemMailbox { get; set; }
            public bool Enabled { get; set; }

            internal SenderInfo Clone()
            {
                var copy = new SenderInfo();

                this.ShallowCopyTo(copy);

                return copy;
            }
        }

        [Serializable]
        private class EmailTemplate
        {
            public Guid MessageIdentifier { get; set; }
            public string MessageName { get; set; }
            public string ContentText { get; set; }
            public string MessageTitle { get; set; }
        }

        #endregion

        #region Properties

        private const string SearchUrl = "/ui/admin/contacts/people/search";

        private string EditUrl => $"/ui/admin/contacts/people/edit?contact={UserId}";

        private Guid UserId => Guid.TryParse(Request.QueryString["contact"], out var value) ? value : Guid.Empty;

        private EmailInfo Email
        {
            get => (EmailInfo)ViewState[nameof(Email)];
            set => ViewState[nameof(Email)] = value;
        }

        private SenderInfo Sender
        {
            get => (SenderInfo)ViewState[nameof(Sender)];
            set => ViewState[nameof(Sender)] = value;
        }

        private List<EmailTemplate> Templates
        {
            get => (List<EmailTemplate>)ViewState[nameof(Templates)];
            set => ViewState[nameof(Templates)] = value;
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            parent.Name.EndsWith("/edit") ? $"contact={UserId}" : null;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ComposeSenderIdentifier.AutoPostBack = true;
            ComposeSenderIdentifier.ValueChanged += (s, a) => OnSenderChanged();

            ComposeNextButton.Click += (s, a) => { if (Page.IsValid) Compose(); };

            ConfirmSendButton.Click += (s, a) => { if (Page.IsValid) Send(); };

            MessageTemplateCombobox.AutoPostBack = true;
            MessageTemplateCombobox.ValueChanged += (s, a) => OnTemplateChanged();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            Open();

            ComposeCancelButton.NavigateUrl = EditUrl;
            ConfirmCancelButton.NavigateUrl = EditUrl;
        }

        #endregion

        #region Event handlers

        private void OnSenderChanged()
        {
            var value = ComposeSenderIdentifier.ValueAsGuid;

            ComposeSenderDescription.InnerText = string.Empty;

            if (!value.HasValue)
                return;

            var sender = TSenderSearch.Select(value.Value);
            if (sender == null)
                return;

            Sender = new SenderInfo
            {
                Identifier = sender.SenderIdentifier,
                Type = sender.SenderType,
                Name = sender.SenderName,
                Email = sender.SenderEmail,
                Enabled = sender.SenderEnabled,
                SystemMailbox = sender.SystemMailbox,
            };

            ComposeSenderDescription.InnerHtml = $"Send using {Sender.Type} from {Sender.Name} &lt;{Sender.Email}&gt;"
                + $" with bouncebacks delivered to {Sender.SystemMailbox}.";
        }

        private void OnTemplateChanged()
        {
            var templateId = MessageTemplateCombobox.ValueAsGuid;
            var template = Templates.FirstOrDefault(x => x.MessageIdentifier == templateId);
            if (template == null)
                return;

            ComposeEmailSubject.Text = template.MessageTitle;
            ComposeEmailBody.Value = template.ContentText;
        }

        #endregion

        #region Methods (open)

        private void Open()
        {
            var user = UserSearch.Select(UserId);
            if (user == null || user.UserPasswordHash == null || user.IsCloaked && !User.IsCloaked)
                HttpResponseHelper.Redirect(SearchUrl);

            var person = ServiceLocator.PersonSearch.GetPerson(UserId, Organization.Identifier, x => x.User);
            if (person == null || !person.UserAccessGranted.HasValue)
                HttpResponseHelper.Redirect(SearchUrl);

            var email = Email = CreateEmailInfo(user, person, Request.QueryString["type"]);
            if (email == null)
                HttpResponseHelper.Redirect(EditUrl);

            SetInputValues(email);
        }

        private void SetInputValues(EmailInfo email)
        {
            PageHelper.AutoBindHeader(Page, qualifier: email.Recipient.FullName);

            if (email.Recipient.Email.IsEmpty())
                ScreenStatus.AddMessage(AlertType.Warning, "Email sending is disabled for this recipient.");

            SetupSenderSelector();

            if (email.EmailType == EmailType.Correspondence)
                InitMessageTemplateField(email.Recipient.Language);

            ComposeRecipientOutput.InnerText = email.Recipient.Email.IsNotEmpty()
                ? $"{email.Recipient.FullName} <{email.Recipient.Email}>"
                : email.Recipient.FullName;
            ComposeEmailSubject.Text = email.EmailSubject;
            ComposeEmailBody.Value = email.EmailBody;

            if (email.Sender != null)
                ComposeSenderIdentifier.ValueAsGuid = email.Sender.Identifier;

            OnSenderChanged();

            var emailFound = email.Recipient.Email.IsNotEmpty();

            ComposeNextButton.Visible = emailFound;
            ConfirmSendButton.Visible = emailFound;

            var enterpriseEmail = ServiceLocator.Partition.Email;
            SenderInstruction.InnerHtml = $"Contact {enterpriseEmail} to configure the list of Senders available in your account.";
        }

        private void SetupSenderSelector()
        {
            ComposeSenderIdentifier.SenderEnabled = true;
            ComposeSenderIdentifier.SenderType = "Mailgun";
            ComposeSenderIdentifier.RefreshData();

            var senderOptions = ComposeSenderIdentifier.FlattenOptions().Where(x => x.Value.IsNotEmpty());
            if (senderOptions.Count() == 1)
                senderOptions.First().Selected = true;
        }

        private void InitMessageTemplateField(string language)
        {
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
                    MessageName = x.MessageName,
                    ContentText = x.ContentText,
                    MessageTitle = x.MessageTitle,
                })
                .ToList();

            MessageTemplateCombobox.Items.Clear();
            MessageTemplateCombobox.Items.Add(new ComboBoxOption());

            foreach (var item in Templates)
            {
                if (!string.IsNullOrWhiteSpace(language) && language != "en")
                {
                    var session = ContentSessionStorage.Create(item.MessageIdentifier, User.UserIdentifier, Organization.CompanyName);
                    var noneEnContent = session?.MarkdownText?.Get(language) ?? "";
                    if (!string.IsNullOrWhiteSpace(noneEnContent))
                        item.ContentText = noneEnContent;
                }

                MessageTemplateCombobox.Items.Add(new ComboBoxOption(item.MessageTitle,
                    item.MessageIdentifier.ToString()));
            }

            MessageTemplateField.Visible = true;
        }

        #endregion

        #region Methods (email)

        private static EmailInfo CreateEmailInfo(User user, QPerson person, string emailType)
        {
            var email = new EmailInfo
            {
                EmailType = emailType.ToEnum(EmailType.Unknown),

                Recipient = new RecipientInfo
                {
                    UserIdentifier = user.UserIdentifier,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Email = GetRecipientEmail(person),
                    PersonCode = person.PersonCode,
                    Language = person.Language ?? "en"
                }
            };

            SetupEmailVariables(email);

            if (email.EmailType == EmailType.Welcome)
            {
                var welcomed = PersonHelper.CreateWelcomeMessage(Organization.Identifier, user.UserIdentifier, false);
                var message = ServiceLocator.CoreProcessManager.CreateEmail(welcomed, Identity.Organization.Identifier);

                var welcomeVars = ServiceLocator.CoreProcessManager
                    .GetWelcomeMsgPasswordInfo(welcomed.UserPassword, welcomed.UserPasswordHash);
                foreach (var property in welcomeVars)
                    email.Variables.Add(property.Key, property.Value);

                email.MessageIdentifier = message.MessageIdentifier;
                email.AutoBccSubscribers = message.AutoBccSubscribers;
                email.Sender = new SenderInfo { Identifier = message.SenderIdentifier };

                email.EmailSubject = message.ContentSubject[email.Recipient.Language];
                email.EmailBody = message.ContentBody[email.Recipient.Language];
            }
            else if (email.EmailType != EmailType.Correspondence)
                return null;

            return email;
        }

        private static string GetRecipientEmail(QPerson user)
        {
            if (EmailAddress.IsValidAddress(user.User.Email, user.EmailEnabled))
                return user.User.Email;
            else if (EmailAddress.IsValidAddress(user.User.EmailAlternate, user.EmailAlternateEnabled))
                return user.User.EmailAlternate;

            return null;
        }

        private static void SetupEmailVariables(EmailInfo email)
        {
            email.Variables.Clear();
            email.Variables.Add("FirstName", email.Recipient.FirstName);
            email.Variables.Add("RecipientNameFirst", email.Recipient.FirstName);
            email.Variables.Add("LastName", email.Recipient.LastName);
            email.Variables.Add("RecipientNameLast", email.Recipient.LastName);
            email.Variables.Add("FullName", email.Recipient.FullName);
            email.Variables.Add("RecipientName", email.Recipient.FullName);
            email.Variables.Add("Email", email.Recipient.Email);
            email.Variables.Add("RecipientEmail", email.Recipient.Email);
            email.Variables.Add("RecipientCode", email.Recipient.PersonCode);
            email.Variables.Add("CompanyTitle", Organization.CompanyName);
            email.Variables.Add("SignInUrl", HttpRequestHelper.CurrentRootUrl + "/ui/lobby/signin");
        }

        private static EmailDraft CreateEmailDraft(EmailInfo info)
        {
            var recipient = info.Recipient;
            var draft = EmailDraft.Create(
                Organization.OrganizationIdentifier,
                info.MessageIdentifier,
                info.Sender.Identifier,
                info.AutoBccSubscribers);

            draft.UserIdentifier = User.Identifier;

            draft.ContentSubject = new MultilingualString(info.EmailSubject);
            draft.ContentBody = new MultilingualString(info.EmailBody);
            draft.ContentVariables = info.Variables;

            draft.SenderEmail = info.Sender.Email;
            draft.SenderName = info.Sender.Name;
            draft.SenderEnabled = info.Sender.Enabled;
            draft.SenderType = info.Sender.Type;
            draft.SystemMailbox = info.Sender.SystemMailbox;

            draft.MailoutScheduled = DateTimeOffset.UtcNow;

            draft.RecipientListTo.Add(recipient.UserIdentifier, recipient.Email);
            draft.Recipients.Add(new EmailAddress(recipient.UserIdentifier, recipient.Email, recipient.FullName, recipient.PersonCode, recipient.Language)
            {
                Variables =
                {
                    ["FirstName"] = recipient.FirstName,
                    ["LastName"] = recipient.LastName
                }
            });

            if (draft.AutoBccSubscribers && !draft.IgnoreSubscribers)
            {
                var subscribers = MessageRepository.GetSubscribers(draft.OrganizationIdentifier, draft.MessageIdentifier.Value);

                foreach (var subscriber in subscribers)
                {
                    if (subscriber.UserEmailEnabled)
                        draft.RecipientListBcc.Add(subscriber.UserIdentifier, subscriber.UserEmail);

                    else if (subscriber.UserEmailAlternateEnabled)
                        draft.RecipientListBcc.Add(subscriber.UserIdentifier, subscriber.UserEmailAlternate);
                }
            }

            return draft;
        }

        #endregion

        #region Methods (actions)

        private void Compose()
        {
            ConfirmTab.Visible = true;
            ConfirmTab.IsSelected = true;
            ConfirmSendButton.Visible = true;

            var email = Email;
            email.Sender = Sender.Clone();

            var draft = CreateEmailDraft(email);
            draft.ContentSubject[email.Recipient.Language] = ComposeEmailSubject.Text;
            draft.ContentBody[email.Recipient.Language] = ComposeEmailBody.Value;

            var message = MessageHelper.BuildMessage(draft, email.Recipient.Language);
            var recipientData = DeliveryAdapter.ToDataTable(Organization.Identifier, draft.Recipients);
            var (subject, body) = EmailOutbox.ReplaceSmarterMailVariables(recipientData, 0, message.Subject, message.Body);

            email.EmailSubject = subject;
            email.EmailBody = body;

            ConfirmSenderOutput.InnerText = $"{email.Sender.Name} <{email.Sender.Email}>";
            ConfirmRecipientOutput.InnerText = $"{email.Recipient.FullName} <{email.Recipient.Email}>";
            ConfirmSubjectOutput.InnerText = email.EmailSubject;
            ConfirmEmailBody.Value = email.EmailBody;
        }

        private void Send()
        {
            var email = CreateEmailDraft(Email);

            email.MailoutIdentifier = UniqueIdentifier.Create();
            email.ContentSubject[Email.Recipient.Language] = ComposeEmailSubject.Text;
            email.ContentBody[Email.Recipient.Language] = ComposeEmailBody.Value;

            try
            {
                ServiceLocator.EmailOutbox.Send(email, "Person");

                if (email.MailoutSucceeded)
                    ScreenStatus.AddMessage(AlertType.Success, $"The email message has been sent to <strong>{Email.Recipient.Email}</strong>.");
                else
                    ScreenStatus.AddMessage(AlertType.Warning, $"No email message has been sent.");
            }
            catch (Exception ex)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"An error occurred on the server side.");

                AppSentry.SentryError(ex);
            }

            ConfirmSendButton.Visible = false;
        }

        #endregion
    }
}