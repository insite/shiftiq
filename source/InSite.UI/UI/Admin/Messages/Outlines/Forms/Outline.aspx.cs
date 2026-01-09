using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;

using Humanizer;

using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Admin.Messages.Messages.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;

namespace InSite.Admin.Messages.Outlines.Forms
{
    public partial class Outline : AdminBasePage
    {
        #region Classes

        [Serializable]
        private class EntityInfo
        {
            public Guid MessageIdentifier { get; }
            public Guid? SurveyFormIdentifier { get; }
            public string MessageType { get; }

            public bool IsAlert => MessageType == MessageTypeName.Alert;
            public bool IsNotification => MessageType == MessageTypeName.Notification;
            public bool IsInvitation => MessageType == MessageTypeName.Invitation;

            public EntityInfo(VMessage message)
            {
                MessageIdentifier = message.MessageIdentifier;
                SurveyFormIdentifier = message.SurveyFormIdentifier;
                MessageType = message.MessageType;
            }
        }

        #endregion

        #region Properties

        private const string SearchUrl = "/ui/admin/messages/messages/search";

        private EntityInfo Entity
        {
            get => (EntityInfo)ViewState[nameof(Entity)];
            set => ViewState[nameof(Entity)] = value;
        }

        public const string ErrorSenderNotFound = "The system could not find the sender for this message.";

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EditContentButton.Click += EditContentButton_Click;

            SubscriberGrid.Refreshed += SubscriberGrid_Refreshed;

            DownloadRespondentsCsv.Click += (s, a) => SendRespondentsCsv();
            DeleteRespondents.Click += DeleteRespondents_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var activeTab = Request.QueryString["tab"];
            if (activeTab == "message")
                MessageTab.IsSelected = true;
            else if (activeTab == "content")
                ContentTab.IsSelected = true;
            else if (activeTab == "mailout")
                MailoutTab.IsSelected = true;

            Open();
        }

        #endregion

        #region Event handlers

        private void EditContentButton_Click(object sender, EventArgs e)
        {
            var session = ContentSessionStorage.Create(Entity.MessageIdentifier, User.UserIdentifier, Organization.CompanyName);
            var url = $"/ui/admin/messages/content?id={session.SessionIdentifier}";

            ScriptManager.RegisterStartupScript(Page, typeof(Outline), "open_editor", $"window.location.href = '{url}';", true);
        }

        private void SubscriberGrid_Refreshed(object sender, EventArgs e)
        {
            var message = ServiceLocator.MessageSearch.GetMessage(Entity.MessageIdentifier);

            RenderProgressGuide(message);
            BindDomainGrid();
            SetupCommandButtons();
        }

        private void DeleteRespondents_Click(object sender, EventArgs e)
        {
            if (!Entity.SurveyFormIdentifier.HasValue)
                return;

            var recipients = ServiceLocator.MessageSearch
                .GetSubscriberUsers(Entity.MessageIdentifier)
                .Select(x => x.UserIdentifier)
                .Distinct()
                .ToArray();

            var respondents = ServiceLocator.SurveySearch.GetResponseSessions(Entity.SurveyFormIdentifier.Value, recipients);

            foreach (var respondent in respondents)
                ServiceLocator.SendCommand(new RemoveMessageSubscriber(Entity.MessageIdentifier, respondent.RespondentUserIdentifier, false));

            InvitationRespondents.Visible = false;

            SubscriberGrid.Refresh();
        }

        #endregion

        #region Methods (open)

        private void Open()
        {
            var message = Guid.TryParse(Request["message"], out var messageId)
                ? ServiceLocator.MessageSearch.GetMessage(messageId)
                : null;

            if (message == null)
                HttpResponseHelper.Redirect(SearchUrl);

            if (message.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                NavPanel.Visible = false;
                ScreenStatus.AddMessage(
                    AlertType.Error,
                    $"Your account (<i>{HttpUtility.HtmlEncode(Organization.CompanyName)}</i>) does not have access to this information.");
                return;
            }

            Entity = new EntityInfo(message);

            PageHelper.AutoBindHeader(Page, null, $"{message.MessageName} <span class='form-text'>{message.MessageType}</span>");

            if (message.SenderEmail.IsEmpty())
                ScreenStatus.AddMessage(AlertType.Error, ErrorSenderNotFound);


            // SubscriberTab

            SubscriberGrid.LoadData(message.MessageIdentifier, message.MessageType);

            if (SubscriberGrid.PersonCount == 0 && SubscriberGrid.GroupCount == 0)
                ScreenStatus.AddMessage(AlertType.Warning, "There are no recipients for this message.");

            SetupContactSection();

            // MailoutTab

            ScheduledGrid.LoadData(message.MessageIdentifier);
            CompletedGrid.LoadData(message.MessageIdentifier);

            // ContentTab

            MessageContent.Visible = message.ContentText.IsNotEmpty();
            MessageContent.Value = MessageHelper.BuildPreviewHtml(Organization.OrganizationIdentifier, message.SenderIdentifier, GetSurveyFormAsset(message.SurveyFormIdentifier), message.ContentText);

            if (message.ContentText.IsEmpty())
                ContentEmpty.AddMessage(AlertType.Warning, "Please click <b>Edit</b> to create content for this message.");

            ValidPlaceHolderNames.LoadData(message);


            // LinkTab

            MailLinks.LoadData(message.MessageIdentifier, true);


            // MessageTab

            SetupCommandButtons();

            Details.SetOutputValues(message, CanEdit);

            RenderProgressGuide(message);
        }

        private void SetupContactSection()
        {
            if (Entity.SurveyFormIdentifier.HasValue)
            {
                var recipients = ServiceLocator.MessageSearch
                    .GetSubscriberUsers(Entity.MessageIdentifier)
                    .Select(x => x.UserIdentifier)
                    .Distinct()
                    .ToList();

                var filter = new QResponseSessionFilter
                {
                    SurveyFormIdentifier = Entity.SurveyFormIdentifier.Value,
                    IsLocked = true
                };

                var respondents = ServiceLocator.SurveySearch.GetResponseSessions(filter)
                    .Select(x => x.RespondentUserIdentifier)
                    .Distinct()
                    .ToList();

                var junction = respondents.Count(respondent => recipients.Any(recipient => recipient == respondent));

                InvitationRespondents.Visible = Entity.IsInvitation && junction > 0;
                RespondentsCount.Text = "form invitation recipient".ToQuantity(junction, "n0") + " submitted";
                DeleteRespondents.OnClientClick = $"return confirm('Are you sure you want to remove from this form invitation the {"respondent".ToQuantity(junction)} who answered the form?');";
            }

            BindDomainGrid();
        }

        private void BindDomainGrid()
        {
            const int MaxDomainsCount = 5;

            var recipients = ServiceLocator.MessageSearch
                .GetSubscriberUsers(Entity.MessageIdentifier)
                .ToList();

            var domains = new List<Counter>();

            foreach (var respondent in recipients)
            {
                if (!EmailAddress.IsValidAddress(respondent.UserEmail))
                    continue;

                var email = new EmailAddress(respondent.UserEmail);
                var domain = domains.FirstOrDefault(x => x.Name == email.Domain);

                if (domain == null)
                {
                    domain = new Counter { Name = email.Domain, Value = 0 };
                    domains.Add(domain);
                }

                domain.Value++;
            }

            var list = domains.OrderByDescending(x => x.Value).ThenBy(x => x.Name).ToList();

            if (list.Count > MaxDomainsCount)
            {
                var others = list.Skip(MaxDomainsCount - 1);

                var other = new Counter { Name = "(other domains)" };
                other.Value = others.Sum(x => x.Value);

                list = list.Take(MaxDomainsCount - 1).ToList();
                list.Add(other);
            }

            DomainGridPanel.Visible = list.Count > 0;

            DomainGrid.DataSource = list;
            DomainGrid.DataBind();
        }

        private void SetupCommandButtons()
        {
            DuplicateButton.Visible =
            DuplicateButton2.Visible = !Entity.IsInvitation && !Entity.IsAlert;

            DuplicateButton.NavigateUrl =
            DuplicateButton2.NavigateUrl = $"/ui/admin/messages/create?action=duplicate&message={Entity.MessageIdentifier}";

            ScheduleMailoutButton1.NavigateUrl = $"/ui/admin/messages/mailouts/schedule?message={Entity.MessageIdentifier}";
            ScheduleMailoutButton1.Visible = SubscriberGrid.GroupCount + SubscriberGrid.PersonCount > 0 && !Entity.IsAlert;

            ScheduleMailoutButton2.NavigateUrl = ScheduleMailoutButton1.NavigateUrl;
            ScheduleMailoutButton2.Visible = ScheduleMailoutButton1.Visible;

            DownloadButton.NavigateUrl = $"/ui/admin/messages/download?message={Entity.MessageIdentifier}";

            ViewHistoryButton.NavigateUrl = AggregateOutline.GetUrl(Entity.MessageIdentifier, $"/ui/admin/messages/outline?message={Entity.MessageIdentifier}&tab=message");

            DeleteButton.NavigateUrl = $"/ui/admin/messages/messages/delete?message={Entity.MessageIdentifier}";

            DuplicateButtonSpacer.Visible = DuplicateButton.Visible || ScheduleMailoutButton2.Visible;
        }

        private void RenderProgressGuide(VMessage message)
        {
            var currentStep = 0;
            var conditionChain = new Func<bool>[]
            {
                () => !string.IsNullOrEmpty(message.MessageType),
                () => !string.IsNullOrEmpty(message.MessageTitle),
                () => !string.IsNullOrEmpty(message.ContentText),
                () => SubscriberGrid.PersonCount > 0
            };

            for (var i = 0; i < conditionChain.Length; i++)
            {
                if (!conditionChain[i]())
                    break;

                currentStep = i + 1;
            }

            var html = new StringBuilder();

            html.Append("<ul class='dot-indicator'>");

            AppendItem(1, "Select Type");
            AppendItem(2, "Add Subject");
            AppendItem(3, "Author Content");
            AppendItem(4, "Add Recipients");
            AppendItem(5, "Schedule Delivery");

            html.Append("</ul>");

            ProgressGuide.Text = html.ToString();

            void AppendItem(int num, string title)
            {
                html.AppendFormat("<li data-num='{0}' ", num);

                if (num <= currentStep)
                    html.Append("class='step-complete'");

                html.Append(">").Append(WebUtility.HtmlEncode(title)).Append("</li>");
            }
        }

        #endregion

        #region Methods (respondents csv)

        private void SendRespondentsCsv()
        {
            if (!Entity.SurveyFormIdentifier.HasValue)
                return;

            var recipients = ServiceLocator.MessageSearch
                    .GetSubscriberUsers(Entity.MessageIdentifier)
                    .Select(x => x.UserIdentifier)
                    .Distinct()
                    .ToArray();

            var filter = new QResponseSessionFilter
            {
                SurveyFormIdentifier = Entity.SurveyFormIdentifier.Value,
                RespondentUserIdentifiers = recipients
            };
            var respondents = ServiceLocator.SurveySearch.GetResponseSessions(filter);

            if (respondents.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no form invitation recipients with submissions.");
                SetupContactSection();
                return;
            }

            var table = new DataTable();
            table.Columns.Add("Name");
            table.Columns.Add("Email");
            table.Columns.Add("Completed", typeof(DateTimeOffset));

            foreach (var respondent in respondents)
            {
                var row = table.NewRow();
                row["Name"] = respondent.RespondentName;
                row["Email"] = respondent.RespondentEmail;

                if (respondent.ResponseSessionCompleted.HasValue)
                    row["Completed"] = respondent.ResponseSessionCompleted;

                table.Rows.Add(row);
            }

            var filename = $"survey-respondents";
            var helper = new CsvExportHelper(table);

            helper.AddMapping("Name", "Name");
            helper.AddMapping("Email", "Email");
            helper.AddMapping("Completed", "Completed", "{0:MMM d, yyyy h:mm tt}");

            var bytes = helper.GetBytes(Encoding.Unicode);

            Page.Response.SendFile(filename, "csv", bytes);
        }

        #endregion

        #region Methods (helpers)

        public static int? GetSurveyFormAsset(Guid? id)
        {
            var survey2 = id.HasValue ? ServiceLocator.SurveySearch.GetSurveyForm(id.Value) : null;

            return survey2?.AssetNumber;
        }

        #endregion
    }
}