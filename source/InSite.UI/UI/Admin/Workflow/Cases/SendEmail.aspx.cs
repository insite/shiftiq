using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Admin.Messages.Messages.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Issues.Outlines.Forms
{
    public partial class SendEmail : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        [Serializable]
        private class EmailInfo
        {
            public RecipientInfo Recipient { get; set; }
            public SenderInfo Sender { get; set; }

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

        private string IssueUrl => $"/ui/admin/workflow/cases/outline?case={IssueId}";

        private Guid UserId => Guid.TryParse(Request.QueryString["contact"], out var value) ? value : Guid.Empty;

        private Guid IssueId => Guid.TryParse(Request.QueryString["issueId"], out var value) ? value : Guid.Empty;

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
            MessageTemplateCombobox.ValueChanged += MessageTemplateCombobox_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect(IssueUrl);

            Open();

            ComposeCancelButton.NavigateUrl = IssueUrl;
            ConfirmCancelButton.NavigateUrl = IssueUrl;
        }

        #endregion

        #region Event handlers

        private void MessageTemplateCombobox_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            var template = Templates.Where(x => x.MessageIdentifier.ToString() == MessageTemplateCombobox.Value).FirstOrDefault();
            if (template != null)
            {
                ComposeEmailSubject.Text = template.MessageTitle;
                ComposeEmailBody.Value = template.ContentText;
            }
        }

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

        #endregion

        #region Methods (open)

        private void Open()
        {
            var person = PersonSearch.Select(Organization.Identifier, UserId, x => x.User);
            if (person == null || !person.UserAccessGranted.HasValue || person.User.UserPasswordHash == null || person.User.IsCloaked && !User.IsCloaked)
                HttpResponseHelper.Redirect(IssueUrl);

            PageHelper.AutoBindHeader(Page, qualifier: person.User.FullName);

            ConfirmTab.Visible = false;

            var email = Email = new EmailInfo
            {
                Recipient = new RecipientInfo
                {
                    UserIdentifier = person.UserIdentifier,
                    FirstName = person.User.FirstName,
                    LastName = person.User.LastName,
                    FullName = person.User.FullName,
                    Email = GetRecipientEmail(person),
                    PersonCode = person.PersonCode,
                    Language = person.Language ?? "en"
                }
            };

            ComposeRecipientOutput.InnerText = email.Recipient.Email.IsNotEmpty()
                ? $"{email.Recipient.FullName} <{email.Recipient.Email}>"
                : email.Recipient.FullName;

            SetupSenderSelector();
            SetupEmailVariables(email);

            if (!SetupEmailType(Email, Request.QueryString["type"]))
                HttpResponseHelper.Redirect(IssueUrl);

            OnSenderChanged();
            SetupCorrespondenceEmail(email, Request.QueryString["type"], person.Language);

            var emailFound = email.Recipient.Email.IsNotEmpty();

            ComposeNextButton.Visible = emailFound;
            ConfirmSendButton.Visible = emailFound;
            ShowMessageTemplate.Visible = emailFound;

            var enterpriseEmail = ServiceLocator.Partition.Email;
            SenderInstruction.InnerHtml = $"Contact {enterpriseEmail} to configure the list of Senders available in your account.";
        }

        private void SetupCorrespondenceEmail(EmailInfo email, string type, string language)
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

            ShowMessageTemplate.Visible = true;
            ComposeEmailSubject.Text = null;
            ComposeEmailBody.Value = null;
        }

        private string GetRecipientEmail(Person person)
        {
            if (!person.EmailEnabled && !person.EmailAlternateEnabled)
                ScreenStatus.AddMessage(AlertType.Warning, "Email sending is disabled for this recipient.");
            else if (EmailAddress.IsValidAddress(person.User.Email, person.EmailEnabled))
                return person.User.Email;
            else if (EmailAddress.IsValidAddress(person.User.EmailAlternate, person.EmailAlternateEnabled))
                return person.User.Email;

            return null;
        }

        private void SetupEmailVariables(EmailInfo email)
        {
            var recipient = email.Recipient;

            email.Variables.Clear();
            email.Variables.Add("FirstName", recipient.FirstName);
            email.Variables.Add("RecipientNameFirst", recipient.FirstName);
            email.Variables.Add("LastName", recipient.LastName);
            email.Variables.Add("RecipientNameLast", recipient.LastName);
            email.Variables.Add("FullName", recipient.FullName);
            email.Variables.Add("RecipientName", recipient.FullName);
            email.Variables.Add("Email", recipient.Email);
            email.Variables.Add("RecipientEmail", recipient.Email);
            email.Variables.Add("RecipientCode", recipient.PersonCode);
            email.Variables.Add("CompanyTitle", Organization.CompanyName);
            email.Variables.Add("SignInUrl", HttpRequestHelper.CurrentRootUrl + "/ui/lobby/signin");
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

        private bool SetupEmailType(EmailInfo email, string type)
        {
            if (type == "welcome")
            {
                var welcomed = PersonHelper.CreateWelcomeMessage(Organization.Identifier, UserId, false);
                var message = ServiceLocator.CoreProcessManager.CreateEmail(welcomed, Identity.Organization.Identifier);

                var welcomeVars = ServiceLocator.CoreProcessManager
                    .GetWelcomeMsgPasswordInfo(welcomed.UserPassword, welcomed.UserPasswordHash);

                foreach (var property in welcomeVars)
                    email.Variables.Add(property.Key, property.Value);

                ComposeSenderIdentifier.ValueAsGuid = message.SenderIdentifier;
                ComposeEmailSubject.Text = message.ContentSubject.Default;
                ComposeEmailBody.Value = message.ContentBody.Default;

                return true;
            }

            if (type == "correspondence")
            {
                ComposeEmailSubject.Text = null;
                ComposeEmailBody.Value = null;

                return true;
            }

            return false;
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
            draft.ContentSubject.Default = ComposeEmailSubject.Text;
            draft.ContentBody.Default = ComposeEmailBody.Value;

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
                ServiceLocator.EmailOutbox.SendAndReplacePlaceholders(email, "Case");

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

        private static EmailDraft CreateEmailDraft(EmailInfo info)
        {
            var recipient = info.Recipient;
            var draft = EmailDraft.Create(
                Organization.OrganizationIdentifier,
                null,
                info.Sender.Identifier,
                false);

            draft.UserIdentifier = User.Identifier;

            draft.ContentSubject = new MultilingualString(info.EmailSubject);
            draft.ContentBody = new MultilingualString(info.EmailBody);
            draft.ContentVariables = new Dictionary<string, string>(info.Variables);

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

            return draft;
        }

        #endregion
    }
}