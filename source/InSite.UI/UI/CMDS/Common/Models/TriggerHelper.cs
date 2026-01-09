using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Changes;

using Humanizer;

using InSite.Application.Messages.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

using Mailout = InSite.Application.Messages.Read.QMailout;

namespace InSite.Admin.Messages.Messages.Forms
{
    public sealed class TriggerHelper
    {
        readonly object __lock = new object();

        #region Construction

        public TriggerHelper(IMessageSearch messageSearch, System.Web.UI.Control container, StateBag stateBag)
        {
            _messageSearch = messageSearch;
            _container = container;
            _stateBag = stateBag;

            GetControl<IButtonControl>("SubmitButton").Click += SubmitButton_Click;
        }

        #endregion

        #region Loading

        public void Load()
        {
            if (_container.Page.IsPostBack)
                return;

            OnSetTitle(
                TriggerModel.MessageTitle,
                TriggerModel.Notified == null ? "Never triggered" : $"Last triggered {TriggerModel.Notified.Humanize()}");

            GetControl<ITextControl>("MessageName").Text = TriggerModel.MessageName.GetName();
            GetControl("MessagePriorityField").Visible = TriggerModel.MessagePriority.IsNotEmpty();
            GetControl<ITextControl>("MessagePriority").Text = $"<span class='text-danger'>{TriggerModel.MessagePriority}</span>";
            GetControl<ITextControl>("MessageSender").Text = $@"{TriggerModel.SenderName} &lt;{TriggerModel.SenderEmail}&gt;";
            GetControl<ITextControl>("MessageBody").Text = Markdown.ToHtml($"{TriggerModel.TextContent}");

            if (EmailPreview == null)
            {
                SetTriggerStatus(AlertType.Error, "This notification cannot be manually triggered.");

                GetControl("MessageRecipientField").Visible = false;
                GetControl("RecipientTab").Visible = false;
                GetControl("SubmitPanel").Visible = false;
            }
            else if (EmailPreview == null || EmailPreview.Recipients.Count == 0)
            {
                SetTriggerStatus(
                    AlertType.Warning,
                    TriggerDate == null
                        ? "There are no current recipients for this notification."
                        : $"There are no recipients for this notification on {TriggerDate.Value:MMMM d, yyyy}.");

                GetControl("MessageRecipientField").Visible = false;
                GetControl("RecipientTab").Visible = false;
                GetControl("SubmitPanel").Visible = false;
            }
            else
            {
                var recipients = EmailPreview.Recipients;

                var recipientsRepeater = GetControl<Repeater>("MessageRecipients");
                recipientsRepeater.DataSource = recipients;
                recipientsRepeater.DataBind();

                GetControl<ITextControl>("MessageRecipient").Text = "recipient".ToQuantity(recipients.Count, "n0");

                GetControl("SubmitPanel").Visible = true;
            }
        }

        #endregion

        #region Event handlers

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            var modeStr = GetControl<ListControl>("SubmitMode").SelectedValue;
            var modeEnum = modeStr.ToEnum<NotificationMode>();

            if (!Enum.IsDefined(typeof(NotificationMode), modeEnum))
                throw new ApplicationError("Unexpected notification mode: " + modeStr);

            var notice = Notifications.Select(TriggerModel.MessageName);

            var when = TriggerDate ?? DateTimeOffset.UtcNow.AddMinutes(5);

            var success = ServiceLocator.CmdsProcessor.Send(EmailPreview, notice, modeEnum, when, ServiceLocator.SendCommand);

            if (!success)
                SetTriggerStatus(AlertType.Error, "An unexpected error occurred.");
            else if (modeEnum == NotificationMode.Authentic)
                SetTriggerStatus(AlertType.Success, $"The email message for this notification has been sent to {"recipient".ToQuantity(EmailPreview.Recipients.Count, "n0")}.");
            else if (modeEnum == NotificationMode.Counterfeit)
                SetTriggerStatus(AlertType.Success, $"No email message for this notification has been sent, but system logs have been updated so it appears the message was sent to {"recipient".ToQuantity(EmailPreview.Recipients.Count, "n0")}.");
            else
                SetTriggerStatus(AlertType.Success, "This notification has been successfully triggered. No email message has been sent.");

            GetControl("SubmitPanel").Visible = false;
        }

        #endregion

        #region Events

        public class SetTitleArgs : EventArgs
        {
            #region Construction

            public SetTitleArgs(string title, string subtitle)
            {
                Title = title;
                Subtitle = subtitle;
            }

            #endregion

            #region Properties

            public string Title { get; }
            public string Subtitle { get; }

            #endregion
        }

        public delegate void SetTitleHandler(object sender, SetTitleArgs args);

        public event SetTitleHandler SetTitle;

        private void OnSetTitle(string title, string subtitle)
        {
            SetTitle?.Invoke(this, new SetTitleArgs(title, subtitle));
        }

        #endregion

        #region Properties

        public Guid? MessageGuid => Guid.TryParse(_container.Page.Request.QueryString["message"], out var value) ? value : (Guid?)null;

        public DateTimeOffset? TriggerDate
        {
            get
            {
                var date = _container.Page.Request.QueryString["date"];
                if (DateTime.TryParseExact(date, "yyyy-MM-dd", Cultures.Default, DateTimeStyles.None, out var result))
                    return result.ToUniversalTime();
                return null;
            }
        }

        public TriggerModel TriggerModel
        {
            get
            {
                if (_stateBag[nameof(TriggerModel)] == null && (bool?)_stateBag[nameof(TriggerModel) + "_Loaded"] != true)
                {
                    var data = MessageGuid.HasValue ? _messageSearch.GetMessage(MessageGuid.Value) : null;

                    if (data != null)
                    {
                        var lastCompletedMailout = _messageSearch.GetMailouts(new MailoutFilter
                        {
                            IsCompleted = true,
                            OrderBy = nameof(Mailout.MailoutCompleted) + " DESC",
                            Paging = Paging.SetSkipTake(0, 1)
                        });

                        var type = data.MessageName.ToEnum<NotificationType>();

                        var senderId = Guid.Parse("afe9a2a4-8f06-4cd1-a0a7-af7a01523591"); // CMDS Admin (Mailgun)

                        var sender = TSenderSearch.Select(senderId);

                        _stateBag[nameof(TriggerModel)] = new TriggerModel
                        {
                            MessageName = type,
                            MessageTitle = data.MessageTitle.IfNullOrEmpty("Untitled"),
                            Notified = lastCompletedMailout.Count == 1 ? lastCompletedMailout[0].MailoutCompleted : null,
                            SenderEmail = sender.SenderEmail,
                            SenderName = sender.SenderName,
                            TextContent = data.ContentText,
                            MessagePriority = MessageRepository.GetEmailPriority(type)
                        };
                    }
                    else
                    {
                        _stateBag[nameof(TriggerModel)] = null;
                    }

                    _stateBag[nameof(TriggerModel) + "_Loaded"] = true;
                }

                return (TriggerModel)_stateBag[nameof(TriggerModel)];
            }
        }

        public EmailDraft EmailPreview
        {
            get
            {
                if (_stateBag[nameof(EmailPreview)] == null && TriggerModel != null)
                    _stateBag[nameof(EmailPreview)] = CreateNotification(TriggerModel.MessageName, TriggerDate ?? DateTimeOffset.UtcNow);

                return (EmailDraft)_stateBag[nameof(EmailPreview)];
            }
        }

        #endregion

        #region Fields

        private readonly IMessageSearch _messageSearch;
        private readonly System.Web.UI.Control _container;
        private readonly StateBag _stateBag;

        #endregion

        #region Helper methods

        private void SetTriggerStatus(AlertType type, string message)
        {
            var alert = GetControl<Alert>("ScreenStatus");
            alert.Indicator = type;
            alert.Text = message;
        }

        private System.Web.UI.Control GetControl(string id)
        {
            return ControlHelper.GetControl(_container, id) ?? throw new ApplicationError("Control not found: " + id);
        }

        private T GetControl<T>(string id)
        {
            object ctrl = GetControl(id);
            if (ctrl is T t)
                return t;

            throw new ApplicationError($"Unexpected type of control ({id}): {typeof(T)}");
        }

        public static string MetadataToHtml(Dictionary<string, string> dictionary)
        {
            var html = new StringBuilder();
            html.Append("<table class='metadata'>");
            foreach (var key in dictionary.Keys)
            {
                var value = Markdown.ToHtml(dictionary[key]);
                if (key.Equals("cc", StringComparison.OrdinalIgnoreCase))
                {
                    var emails = value.Split(',');
                    value = string.Join(", ", emails);
                }

                html.Append("<tr><th>");
                html.AppendFormat("{0}: </th><td>{1}", key, value);
                html.Append("</td></tr>");
            }

            html.Append("</table>");
            return html.ToString();
        }

        private EmailDraft CreateNotification(NotificationType name, DateTimeOffset now)
        {
            var reminderType = ReminderType.Today;
            var kind = ExpiryKind.None;
            var priority = MessageRepository.GetEmailPriority(name);

            if (name == NotificationType.CmdsCompetenciesExpired)
            {
                reminderType = ReminderType.Today;
                kind = ExpiryKind.Competencies;
            }
            else if (name == NotificationType.AchievementCredentialsExpiredToday)
            {
                reminderType = ReminderType.Today;
                kind = ExpiryKind.Achievements;
            }
            else if (name == NotificationType.AchievementCredentialsExpiringInOneMonth)
            {
                reminderType = ReminderType.InOneMonth;
                kind = ExpiryKind.Achievements;
            }
            else if (name == NotificationType.AchievementCredentialsExpiringInTwoMonths)
            {
                reminderType = ReminderType.InTwoMonths;
                kind = ExpiryKind.Achievements;
            }
            else if (name == NotificationType.AchievementCredentialsExpiringInThreeMonths)
            {
                reminderType = ReminderType.InThreeMonths;
                kind = ExpiryKind.Achievements;
            }

            return CreateNotification(reminderType, kind, now, priority);
        }

        private EmailDraft CreateNotification(ReminderType reminderType, ExpiryKind kind, DateTimeOffset now, string priority)
        {
            lock (__lock)
            {
                IChange e = kind == ExpiryKind.Competencies
                    ? e = new CmdsCompetenciesExpired(now)
                    : new CmdsAchievementExpirationDelivered(reminderType, priority);

                return ServiceLocator.CmdsProcessor.BuildEmail(e);
            }
        }

        #endregion
    }
}