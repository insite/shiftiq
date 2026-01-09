using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

using DocumentFormat.OpenXml;

using Humanizer;

using InSite.Application.Messages.Read;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Messages.Mailouts.Forms
{
    public partial class Schedule : AdminBasePage, IHasParentLinkParameters
    {
        const int DeliveryLimit = 20000;

        #region Classes

        [Serializable]
        private class MessageInfo
        {
            public Guid OrganizationIdentifier { get; }
            public Guid MessageIdentifier { get; }
            public string MessageType { get; }
            public string MessageTitle { get; }
            public Guid SenderIdentifier { get; }
            public Guid? SurveyFormIdentifier { get; }
            public string ContentText { get; }
            public bool AutoBccSubscribers { get; }

            public Guid? RecipientIdentifier { get; }
            public IReadOnlyCollection<RecipientInfo> Recipients { get; set; }

            public MessageInfo(VMessage message, Guid? recipientId)
            {
                OrganizationIdentifier = message.OrganizationIdentifier;
                MessageIdentifier = message.MessageIdentifier;
                MessageType = message.MessageType;
                MessageTitle = message.MessageTitle;
                SenderIdentifier = message.SenderIdentifier;
                SurveyFormIdentifier = message.SurveyFormIdentifier;
                ContentText = message.ContentText;
                AutoBccSubscribers = message.AutoBccSubscribers;

                RecipientIdentifier = recipientId;
            }
        }

        [Serializable]
        private class RecipientInfo
        {
            public Guid UserIdentifier { get; }
            public string UserFullName { get; }
            public string UserFirstName { get; }
            public string UserLastName { get; }
            public string UserEmail { get; }
            public bool UserEmailEnabled { get; }
            public bool UserMarketingEmailEnabled { get; set; }
            public string UserEmailAlternate { get; }
            public bool UserEmailAlternateEnabled { get; }
            public string PersonLanguage { get; }
            public string PersonCode { get; }

            public RecipientInfo(ISubscriberPerson user)
            {
                UserIdentifier = user.UserIdentifier;
                UserFullName = user.UserFullName;
                UserFirstName = user.UserFirstName;
                UserLastName = user.UserLastName;
                UserEmail = user.UserEmail;
                UserEmailEnabled = user.UserEmailEnabled;
                UserMarketingEmailEnabled = user.UserMarketingEmailEnabled;
                UserEmailAlternate = user.UserEmailAlternate;
                UserEmailAlternateEnabled = user.UserEmailAlternateEnabled;
                PersonLanguage = user.PersonLanguage;
                PersonCode = user.PersonCode;
            }
        }

        #endregion

        #region Properties

        private const string SearchUrl = "/ui/admin/messages/messages/search";

        private string OutlineUrl =>
            $"/ui/admin/messages/outline?message={Message.MessageIdentifier}";

        private MessageInfo Message
        {
            get => (MessageInfo)ViewState[nameof(Message)];
            set => ViewState[nameof(Message)] = value;
        }

        private bool IsNewsletter => string.Equals(Message.MessageType, MessageTypeName.Newsletter, StringComparison.OrdinalIgnoreCase);

        protected bool IsScheduleActive { get; set; }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/outline"))
                return $"message={Message.MessageIdentifier}";

            if (parent.Name.EndsWith("/search"))
                return $"type={Server.UrlEncode(Message.MessageType)}";

            return null;
        }

        #endregion

        #region Fields

        private static readonly ConcurrentDictionary<Guid, DateTime> _activeMessages = new ConcurrentDictionary<Guid, DateTime>();

        private static readonly Regex RegexLinkPattern = new Regex("\\s+href=(?:(?:'(?<URL>[^']+)')|(?:\"(?<URL>[^\"]+)\"))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexEmailPattern = new Regex("\\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\\Z", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            if (HandleAjaxRequest())
                return;

            base.OnInit(e);

            ScheduleButton.Click += (s, a) => { if (Page.IsValid) ScheduleMessage(); };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();

                CloseButton.NavigateUrl = OutlineUrl + "&tab=message";
            }

            ValidateScreen();
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Schedule), "vars", $"var ___messageRecipientsCount = {Message.Recipients.Count};", true);

            IsScheduleActive = _activeMessages.TryGetValue(Message.MessageIdentifier, out var startDate);

            if (ScheduleButton.Visible)
                ScheduleButton.Visible = !IsScheduleActive;

            base.OnPreRender(e);
        }

        #endregion

        #region Methods (open)

        private void Open()
        {
            var message = Guid.TryParse(Request.QueryString["message"], out var messageId)
                ? ServiceLocator.MessageSearch.GetMessage(messageId)
                : null;
            if (message == null)
                HttpResponseHelper.Redirect(SearchUrl);

            Message = new MessageInfo(message, ValueConverter.ToGuidNullable(Page.Request.QueryString["recipient"]));
            Message.Recipients = GetRecipients(message, Message.RecipientIdentifier);

            if (message.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                var organization = OrganizationSearch.Select(message.OrganizationIdentifier);

                ScreenStatus.AddMessage(
                    AlertType.Error,
                    $"You cannot schedule a mailout for this message (<i>{HttpUtility.HtmlEncode(message.MessageTitle)}</i>) " +
                    $"because it belongs to another organization account (<i>{HttpUtility.HtmlEncode(organization?.CompanyName ?? "??")}</i>).");
                MessageSection.Visible = false;
                ProblemSection.Visible = false;
                return;
            }

            PageHelper.AutoBindHeader(
                Page,
                qualifier: message.MessageTitle);

            var sender = TSenderSearch.Select(message.SenderIdentifier);

            if (sender != null)
                MessageSenderOutput.InnerText = sender.SenderName != null
                    ? $"{sender.SenderName} <{sender.SenderEmail}>"
                    : sender.SystemMailbox;

            MessageSubjectOutput.InnerText = message.MessageTitle;

            ScheduleDate.DefaultTimeZone = User.TimeZone.Id;
            ScheduleDate.Value = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, User.TimeZone);

            LoadRecipients(Message.Recipients);
        }

        private RecipientInfo[] GetRecipients(VMessage message, Guid? recipientId)
        {
            var contacts = ServiceLocator.MessageSearch.GetSubscribers(message.OrganizationIdentifier, message.MessageIdentifier, recipientId);

            if (message.MessageType != MessageTypeName.Invitation || !message.SurveyFormIdentifier.HasValue)
                return contacts.Select(x => new RecipientInfo(x)).ToArray();

            // If this is a form invitation then existing respondents should be excluded automatically from
            // the recipient list.

            var filter = new QResponseSessionFilter
            {
                SurveyFormIdentifier = message.SurveyFormIdentifier,
                IsLocked = true
            };

            var respondents = ServiceLocator.SurveySearch
                .GetResponseSessions(filter)
                .Select(x => x.RespondentUserIdentifier)
                .Distinct()
                .ToList();

            return contacts
                .Where(contact => !respondents.Any(respondent => respondent == contact.UserIdentifier))
                .Select(x => new RecipientInfo(x))
                .ToArray();
        }

        private void LoadRecipients(IReadOnlyCollection<RecipientInfo> recipients)
        {
            var html = new StringBuilder();

            if (!BuildRecipientsShortlist(html, recipients))
                html.Append("recipient".ToQuantity(recipients.Count, "n0"));

            LargeMailoutNotice.Visible = 1000 < recipients.Count && recipients.Count <= DeliveryLimit;
            OversizeMailoutNotice.Visible = DeliveryLimit < recipients.Count;

            MailoutDeliveryLimit.Text = $"{DeliveryLimit:n0}";

            var (disabled, marketingDisabled) = GetUnsubscribers(recipients);
            if (disabled.Length > 0)
            {
                html.Append($@"
<div class='alert alert-warning' role='alert'>
<strong><i class='fas fa-exclamation-triangle'></i> Warning: </strong>
These subscribers will not get this message because they do not have at least one enabled email address:
<ul class='fst-italic'>");
                foreach (var item in disabled)
                    html.Append($"<li>{item}</li>");
                html.Append("</ul></div>");
            }

            if (marketingDisabled.Length > 0)
            {
                html.Append($@"
<div class='alert alert-warning' role='alert'>
<strong><i class='fas fa-exclamation-triangle'></i> Warning: </strong>
These subscribers will not get this message because they have unsubscribed from marketing emails (newsletters):
<ul class='fst-italic'>");
                foreach (var item in marketingDisabled)
                    html.Append($"<li>{item}</li>");
                html.Append("</ul></div>");
            }

            MessageRecipientsOutput.InnerHtml = html.ToString();
        }

        private bool BuildRecipientsShortlist(StringBuilder html, IReadOnlyCollection<RecipientInfo> recipients)
        {
            var hasItem = false;

            if (recipients.Count > 10)
                return hasItem;

            foreach (var recipient in recipients)
            {
                if (IsNewsletter && !recipient.UserMarketingEmailEnabled)
                    continue;

                if (html.Length > 0)
                    html.Append("<br />");

                if (EmailAddress.IsValidAddress(recipient.UserEmail, recipient.UserEmailEnabled))
                {
                    hasItem = true;
                    html.AppendFormat("{0} &lt;{1}&gt;", recipient.UserFullName, recipient.UserEmail);
                }
                else if (EmailAddress.IsValidAddress(recipient.UserEmailAlternate, recipient.UserEmailAlternateEnabled))
                {
                    hasItem = true;
                    html.AppendFormat("{0} &lt;{1}&gt;", recipient.UserFullName, recipient.UserEmailAlternate);
                }
            }

            return hasItem;
        }

        private (string[] disabled, string[] marketingDisabled) GetUnsubscribers(IReadOnlyCollection<RecipientInfo> recipients)
        {
            var disabled = recipients
                .Where(r =>
                    (!r.UserEmailEnabled || string.IsNullOrEmpty(r.UserEmail)) &&
                    (!r.UserEmailAlternateEnabled || string.IsNullOrEmpty(r.UserEmailAlternate)))
                .SelectMany(r => new[] { r.UserEmail, r.UserEmailAlternate }.Where(e => !string.IsNullOrEmpty(e)))
                .ToArray();

            var marketingDisabled = IsNewsletter
                ? recipients
                    .Where(r => !r.UserMarketingEmailEnabled)
                    .Select(r => string.IsNullOrEmpty(r.UserEmail) ? r.UserEmailAlternate : r.UserEmail)
                    .ToArray()
                : Array.Empty<string>();

            return (disabled, marketingDisabled);
        }

        #endregion

        #region Methods (AJAX)

        private bool HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest || !bool.TryParse(Page.Request.Form["IsPageAjax"], out bool isAjax) || !isAjax)
                return false;

            Response.Clear();

            var action = Page.Request.Form["action"];

            if (action == "check-status")
            {
                Response.ContentType = "text/plain";

                var messageId = Guid.Parse(Request.QueryString["message"]);

                if (_activeMessages.TryGetValue(messageId, out var startDate))
                    Response.Write($"{startDate:yyyyMMdd}T{startDate:HHmmss}Z");
                else
                    Response.Write("RELOAD");
            }
            else
            {
                throw new NotImplementedException($"Unexpected action name: {action}");
            }

            Response.End();

            return true;
        }

        #endregion

        #region Methods (schedule)

        private void ScheduleMessage()
        {
            var utcQueued = ScheduleDate.Value.Value;
            if (utcQueued > DateTimeOffset.UtcNow.AddDays(3))
            {
                ScreenStatus.AddMessage(AlertType.Error, "Messages can be scheduled for delivery a maximum of 3 days (72 hours) in the future.");
                return;
            }

            if (!_activeMessages.TryAdd(Message.MessageIdentifier, DateTime.UtcNow))
            {
                ScreenStatus.AddMessage(AlertType.Error, "The message is currently sending. Please try again later.");
                return;
            }

            // We need to avoid race conditions between the web application and the Windows service. If both
            // applications send commands and/or publish events on the same aggregate at the same time then
            // we can get collisions on aggregate event version numbers. 

            // We need a more robust solution than the one I have impleted here, but it should help to avoid
            // collisions in the short term if we force the scheduled delivery date to be at least 3 minutes
            // in the future.

            if (ServiceLocator.AppSettings.Environment.Name == EnvironmentName.Production)
            {
                var minUtcQueued = DateTimeOffset.UtcNow.AddMinutes(2);
                if (utcQueued < minUtcQueued)
                    utcQueued = minUtcQueued;
            }

            var content = ServiceLocator.ContentSearch.GetBlock(Message.MessageIdentifier);

            try
            {
                var email = EmailDraft.Create(
                    Message.OrganizationIdentifier,
                    Message.MessageIdentifier,
                    Message.SenderIdentifier,
                    Message.AutoBccSubscribers
                    );

                email.ContentSubject = content.Title.Text;
                email.ContentBody = content.Body.Text;
                email.ContentVariables = new MessageVariableList().ToDictionary();

                if (Message.SurveyFormIdentifier != null)
                {
                    var survey = ServiceLocator.SurveySearch.GetSurveyForm(Message.SurveyFormIdentifier.Value);
                    if (survey != null)
                    {
                        email.SurveyIdentifier = survey.SurveyFormIdentifier;
                        email.SurveyNumber = survey.AssetNumber;
                    }
                }

                var subscribers = Message.Recipients.Select(x => x.UserIdentifier).ToList();

                foreach (var recipient in Message.Recipients)
                {
                    var userEmail = EmailAddress.GetEnabledEmail(recipient.UserEmail, recipient.UserEmailEnabled, recipient.UserEmailAlternate, recipient.UserEmailAlternateEnabled);
                    if (userEmail.IsEmpty() || IsNewsletter && !recipient.UserMarketingEmailEnabled)
                        continue;

                    var address = new EmailAddress(recipient.UserIdentifier, userEmail, recipient.UserFullName, recipient.PersonCode, recipient.PersonLanguage)
                    {
                        Variables =
                        {
                            ["FirstName"] = recipient.UserFirstName,
                            ["LastName"] = recipient.UserLastName,
                            ["PersonCode"] = recipient.PersonCode
                        }
                    };

                    email.Recipients.Add(address);
                }

                var organization = Organization;

                AddToQueue(organization, email, utcQueued);

                ScreenStatus.AddMessage(AlertType.Success, $"{email.Recipients.Count:n0} email deliveries scheduled for {utcQueued.Format(User.TimeZone)}");

                ScheduleButton.Visible = false;
                CloseButton.NavigateUrl = OutlineUrl + "&tab=mailout";
            }
            catch (Exception ex)
            {
                ScreenStatus.AddMessage(AlertType.Error, GetExceptionMessages(ex, 0));
            }
            finally
            {
                _activeMessages.TryRemove(Message.MessageIdentifier, out var startDate);
            }
        }

        private string GetExceptionMessages(Exception ex, int depth)
        {
            var message = $"<div style='padding-left:{20 * depth}px;'><p style='margin-top:{10 * depth}px'>{ex.Message}</p><div class='form-text'>{ex.StackTrace}</div></div>";

            if (ex.InnerException != null)
            {
                message += GetExceptionMessages(ex.InnerException, depth + 1);
            }

            return message;
        }

        public Guid AddToQueue(OrganizationState organization, EmailDraft email, DateTimeOffset scheduled)
        {
            if (email.SenderIdentifier == Guid.Empty)
                throw new Exception("The sender identifier is missing.");

            email.MailoutIdentifier = UniqueIdentifier.Create();
            email.OrganizationIdentifier = organization.OrganizationIdentifier;
            email.MailoutScheduled = scheduled;

            ServiceLocator.EmailOutbox.Send(email);

            return email.MailoutIdentifier;
        }

        private void ValidateScreen()
        {
            var isProblem = false;
            var isValid = true;

            if (string.IsNullOrEmpty(Message.ContentText))
            {
                isValid = false;
                ScreenStatus.AddMessage(AlertType.Warning, "You cannot send this email message because the body of the message is empty.");
            }

            if (Message.Recipients.Count == 0)
            {
                isValid = false;
                ScreenStatus.AddMessage(AlertType.Warning, "You cannot send this email message because it has no recipients.");
            }

            if (Message.Recipients.Count > DeliveryLimit)
            {
                isValid = false;
                ScreenStatus.AddMessage(AlertType.Error, $"You cannot send this email message because it contains more than {DeliveryLimit:n0} recipients.");
            }

            var invalidUrl = new DataTable();

            invalidUrl.Columns.Add("Url", typeof(string));
            invalidUrl.Columns.Add("Reason", typeof(string));

            if (Message.ContentText.IsNotEmpty())
            {
                var htmlContent = MessageHelper.BuildPreviewHtml(Organization.OrganizationIdentifier, Message.SenderIdentifier, Outlines.Forms.Outline.GetSurveyFormAsset(Message.SurveyFormIdentifier), Message.ContentText);

                var matches = RegexLinkPattern.Matches(htmlContent);

                foreach (Match match in matches)
                {
                    var url = HttpUtility.HtmlDecode(match.Groups["URL"].Value);

                    if (url.StartsWith("mailto:"))
                    {
                        var mailTo = url.Substring(7);
                        var mailToEmail = mailTo;
                        NameValueCollection parameters = null;

                        try
                        {
                            var paramSeparatorIndex = mailTo.IndexOf("?", StringComparison.Ordinal);
                            if (paramSeparatorIndex > 0)
                            {
                                mailToEmail = mailTo.Substring(0, paramSeparatorIndex);
                                parameters = HttpUtility.ParseQueryString(mailTo.Substring(paramSeparatorIndex + 1));
                            }

                            if (!IsEmailValid(mailToEmail))
                            {
                                AddRow(invalidUrl, url, "Invalid email address: " + mailToEmail);
                            }

                            if (parameters != null)
                            {
                                foreach (string pKey in parameters)
                                {
                                    var pValue = parameters[pKey];
                                    if (string.Compare(pKey, "cc", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(pKey, "bcc", StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        continue;
                                    }

                                    var ccEmails = pValue.Split(',');
                                    foreach (var ccEmail in ccEmails)
                                    {
                                        if (!IsEmailValid(ccEmail))
                                        {
                                            AddRow(invalidUrl, url, "Invalid email address: " + ccEmail);
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            AddRow(invalidUrl, url, "Invalid mailto syntax");
                        }
                    }
                    else
                    {
                        if (!IsUrlValid(url))
                        {
                            AddRow(invalidUrl, url, "Invalid URL");
                        }
                    }
                }
            }

            if (invalidUrl.Rows.Count > 0)
            {
                isProblem = true;

                InvalidLinksProblem.Visible = true;

                InvalidLinksRepeater.DataSource = invalidUrl;
                InvalidLinksRepeater.DataBind();
            }

            ScheduleDateField.Visible = isValid;
            ScheduleButton.Visible = isValid;
            ProblemSection.Visible = isProblem;
        }

        private static bool IsUrlValid(string url)
        {
            // Allow a special-case pattern for form links. The URL here is resolved in the Send method.
            if (StringHelper.Equals(url, $"${MessageVariable.AppUrl}/{{Survey-Path}}"))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (!string.IsNullOrEmpty(uriResult.Query) || uriResult.LocalPath.IndexOf("&", StringComparison.Ordinal) < 0);
        }

        private static bool IsEmailValid(string email)
        {
            return email.IsNotEmpty() && RegexEmailPattern.IsMatch(email.Trim());
        }

        private static void AddRow(DataTable t, params object[] values)
        {
            var row = t.NewRow();

            for (var i = 0; i < values.Length; i++)
                row[i] = values[i];

            t.Rows.Add(row);
        }

        #endregion
    }
}