using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Messages.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Admin.Events.Timers.Controls
{
    public partial class TimerPanel : BaseUserControl
    {
        TimerPanelProperties Properties;

        #region UI Event Handling

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventNotificationButton.Click += AddEventTimer_Clicked;
            EventTimerRepeater.ItemCommand += EventTimer_Clicked;
            EventTimerRepeater.ItemDataBound += EventTimerRepeater_ItemDataBound;

            RegistrationNotificationButton.Click += AddRegistrationTimer_Clicked;
            RegistrationTimerRepeater.ItemCommand += RegistrationTimer_Clicked;

            MailoutGrid.RowCommand += Mailout_Clicked;
        }

        private void EventTimerRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var elapse = (IconButton)e.Item.FindControl("ElapseButton");
            var cancel = (IconButton)e.Item.FindControl("CancelButton");
            var timer = (QEventTimer)e.Item.DataItem;
            elapse.Visible = timer.TimerStatus != "Elapsed";
            cancel.Visible = elapse.Visible;
        }

        private void AddEventTimer_Clicked(object sender, EventArgs e)
            => AddEventTimer(Properties.EventIdentifier, EventNotificationCombo.Value);

        private void EventTimer_Clicked(object source, RepeaterCommandEventArgs e)
            => ExecuteEventCommand(e.CommandName, Properties.EventIdentifier, Guid.Parse((string)e.CommandArgument));

        private void AddRegistrationTimer_Clicked(object sender, EventArgs e)
        {
            if (RegistrationCombo.ValueAsGuid != null && RegistrationNotificationCombo.Value != null)
                AddRegistrationTimer(RegistrationCombo.ValueAsGuid, RegistrationCombo.GetSelectedOption().Text, RegistrationNotificationCombo.Value);
        }

        private void RegistrationTimer_Clicked(object source, RepeaterCommandEventArgs e)
            => ExecuteRegistrationCommand(e.CommandName, Guid.Parse((string)e.CommandArgument));

        private void Mailout_Clicked(object sender, GridViewCommandEventArgs e)
        {
            var grid = (Grid)sender;
            var row = GridViewExtensions.GetRow(e);
            var messageId = grid.GetDataKey<Guid>(row, "MessageIdentifier");
            var mailoutId = grid.GetDataKey<Guid>(row, "MailoutIdentifier");

            ExecuteMailoutCommand(e.CommandName, messageId, mailoutId);

        }

        #endregion

        #region UI Navigation

        private void Reload()
            => HttpResponseHelper.Redirect(Properties.EventOutlineUrl);

        #endregion

        #region Data Binding

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Properties = new TimerPanelProperties(Request, ViewState);

            if (!IsPostBack)
            {
                BindEventTimers();
                BindNotificationCombo(EventNotificationCombo, "Event");
                EventNotificationCombo.Items.Add(new ComboBoxOption(Properties.ValidateAndPublishOption, Properties.ValidateAndPublishOption));
                EventNotificationCombo.Items.Add(new ComboBoxOption("ITA001: Training Provider Notice - Class Event", NotificationType.ITA001.ToString()));
                EventNotificationCombo.Items.Add(new ComboBoxOption("ITA013: Administrator Notice - Exam Results Failed Validation", NotificationType.ITA013.ToString()));
                EventNotificationCombo.Items.Add(new ComboBoxOption("ITA016: Exam Event Results", NotificationType.ITA016.ToString()));
                EventNotificationCombo.Items.Add(new ComboBoxOption("ITA026: No Return Shipment Received", NotificationType.ITA026.ToString()));

                BindRegistrationTimers();
                BindNotificationCombo(RegistrationNotificationCombo, "Registration");
                RegistrationNotificationCombo.Items.Add(new ComboBoxOption("ITA016: Instructor Notice - Candidate Results", NotificationType.ITA016.ToString()));
                BindRegistrationCombo(RegistrationCombo);

                BindMailouts();
            }
        }

        private void BindEventTimers()
        {
            var timers = ServiceLocator.EventSearch.GetTimers(new QEventTimerFilter
            {
                EventIdentifier = Properties.EventIdentifier
            });

            EventTimerRepeater.DataSource = timers.OrderByDescending(x => x.TriggerTime);
            EventTimerRepeater.DataBind();
            EventTimerRepeater.Visible = timers.Count > 0;
            EventTimerHelp.Visible = !EventTimerRepeater.Visible;
        }

        private void BindRegistrationTimers()
        {
            var timers = ServiceLocator.RegistrationSearch
                .GetTimers(new QRegistrationTimerFilter { EventIdentifier = Properties.EventIdentifier });

            RegistrationTimerRepeater.DataSource = timers.OrderByDescending(x => x.TriggerTime);
            RegistrationTimerRepeater.DataBind();
            RegistrationTimerRepeater.Visible = timers.Count > 0;
            RegistrationTimerHelp.Visible = !RegistrationTimerRepeater.Visible;
        }

        private void BindNotificationCombo(ComboBox combo, string scope)
        {
            combo.Items.Clear();
            combo.Items.Add(new ComboBoxOption());

            var notifications = Notifications.All
                .Where(x => x.Slug.ToString().StartsWith("ITA") && x.Aggregate == scope)
                .OrderBy(x => x.Slug);

            foreach (var notification in notifications)
                combo.Items.Add(new ComboBoxOption(
                    $"{notification.Slug}: {notification.Purpose}",
                    notification.Slug.ToString()));
        }

        private void BindRegistrationCombo(ComboBox combo)
        {
            combo.LoadItems(
                ServiceLocator.RegistrationSearch.GetRegistrationsByEvent(
                    Properties.EventIdentifier,
                    null, null, null, false, false, true, false),
                "RegistrationIdentifier", "Candidate.UserFullName"
            );
        }

        private void BindMailouts()
        {
            var mailouts = ServiceLocator.MessageSearch.GetMailouts(new MailoutFilter
            {
                EventIdentifier = Properties.EventIdentifier
            });

            MailoutGrid.DataSource = mailouts.Select(mailout => new MailoutItem(
                mailout.MailoutScheduled,
                mailout.MailoutStarted,
                mailout.MailoutCompleted,
                mailout.MailoutCancelled,
                mailout.MailoutStatus,
                mailout.MailoutError,
                mailout.MessageName,
                mailout.ContentSubject,
                mailout.DeliveryCount,
                mailout.MailoutIdentifier,
                mailout.MessageIdentifier.Value,
                mailout.MailoutScheduled.Humanize(),
                mailout.MailoutCompleted == null && mailout.MailoutScheduled < DateTime.UtcNow,
                mailout.MailoutStarted != null,
                mailout.MailoutCompleted != null
            )).OrderByDescending(x => x.MailoutScheduled);
            MailoutGrid.DataBind();
            MailoutGrid.Visible = mailouts.Count > 0;
            MailoutHelp.Visible = !MailoutGrid.Visible;
        }

        protected string GetCompletionStatus(object o)
        {
            var html = new StringBuilder();
            var mailout = (MailoutItem)o;

            if (mailout.IsCompleted)
            {
                var indicator = "default";
                if (mailout.MailoutStatus == "Completed")
                    indicator = "success";
                if (mailout.MailoutStatus == "Aborted")
                    indicator = "danger";

                html.Append($"<span class='badge bg-{indicator}'>");
                html.Append(mailout.MailoutStatus);
                html.Append("</span>");
                if (mailout.MailoutStatusReason != null)
                {
                    var mailoutStatusReason = AdjustMailoutStatusReason(mailout.MailoutStatusReason);
                    html.Append($"<div class='form-text mt-2' ><p class='text-danger'>{mailoutStatusReason}</p></div>");
                }
            }
            return html.ToString();
        }

        private string AdjustMailoutStatusReason(string mailoutStatusReason)
        {
            if (string.IsNullOrEmpty(mailoutStatusReason))
                return mailoutStatusReason;

            const int maxLength = 500;

            var text = StringHelper.StripHtml(mailoutStatusReason);

            return text.Length > maxLength ? text.Substring(0, maxLength) : text;
        }

        #endregion

        #region Command Sending

        private void AddEventTimer(Guid @event, string name)
        {
            if (name.IsEmpty())
                return;

            if (name == "ITA013")
            {
                var trigger = new TriggerEventNotification(@event, name);
                ServiceLocator.SendCommand(trigger);
                return;
            }

            var now = DateTimeOffset.Now;
            var notification = name.ToEnumNullable<NotificationType>();

            if (notification.HasValue)
            {
                ServiceLocator.SendCommand(new TriggerEventNotification(@event, notification.Value.ToString()));
            }

            else if (name == Properties.ValidateAndPublishOption)
                StartEventPublicationTimer(@event, now.AddHours(4), "Wait 4 hours, then validate and publish exam results");

            Reload();
        }

        private void StartEventPublicationTimer(Guid @event, DateTimeOffset when, string description)
        {
            // Create a command to trigger the notification.
            var timer = new PublishEventScores(@event, null, false);

            // Store the command for future reference.
            ServiceLocator.BookmarkCommand(timer, when);

            // Start the timer.
            ServiceLocator.SendCommand(new StartEventTimer(@event, timer.CommandIdentifier, when, description));
        }

        private void ExecuteEventCommand(string command, Guid @event, Guid timer)
        {
            if (command == "Elapse")
                ServiceLocator.SendCommand(new ElapseEventTimer(@event, timer));

            else if (command == "Cancel")
                ServiceLocator.SendCommand(new CancelEventTimer(@event, timer));

            Reload();
        }

        private void AddRegistrationTimer(Guid? registrationId, string learnerName, string notification)
        {
            if (notification.IsEmpty() || !registrationId.HasValue)
                return;

            var type = notification.ToEnumNullable<NotificationType>();
            if (type.HasValue)
                ServiceLocator.SendCommand(new TriggerNotification(registrationId.Value, notification));

            Reload();
        }

        private void ExecuteRegistrationCommand(string command, Guid timer)
        {
            var t = ServiceLocator.RegistrationSearch.GetTimer(timer);
            if (t != null)
            {
                var registration = t.RegistrationIdentifier;

                if (command == "Elapse")
                    ServiceLocator.SendCommand(new ElapseRegistrationTimer(registration, timer));

                else if (command == "Cancel")
                    ServiceLocator.SendCommand(new CancelRegistrationTimer(registration, timer));
            }
            Reload();
        }

        private void ExecuteMailoutCommand(string command, Guid message, Guid mailout)
        {
            if (command == "Cancel")
                ServiceLocator.SendCommand(new CancelMailout(message, mailout));
            Reload();
        }

        #endregion
    }

    internal class MailoutItem
    {
        public DateTimeOffset MailoutScheduled { get; }
        public DateTimeOffset? MailoutStarted { get; }
        public DateTimeOffset? MailoutCompleted { get; }
        public DateTimeOffset? MailoutCancelled { get; }
        public string MessageName { get; }
        public string MessageTitle { get; }
        public string MailoutStatus { get; }
        public string MailoutStatusReason { get; }
        public int? DeliveryCount { get; }
        public Guid MailoutIdentifier { get; }
        public Guid MessageIdentifier { get; }
        public string Age { get; }
        public bool IsOverdue { get; }
        public bool IsStarted { get; }
        public bool IsCompleted { get; }

        public MailoutItem(DateTimeOffset mailoutScheduled, DateTimeOffset? mailoutStarted, DateTimeOffset? mailoutCompleted, DateTimeOffset? mailoutCancelled, string mailoutStatus, string mailoutStatusReason, string messageName, string messageTitle, int? deliveryCount, Guid mailoutIdentifier, Guid messageIdentifier, string age, bool isOverdue, bool isStarted, bool isCompleted)
        {
            MailoutScheduled = mailoutScheduled;
            MailoutStarted = mailoutStarted;
            MailoutCompleted = mailoutCompleted;
            MailoutCancelled = mailoutCancelled;
            MailoutStatus = mailoutStatus;
            MailoutStatusReason = mailoutStatusReason;
            MessageName = messageName;
            MessageTitle = messageTitle;
            DeliveryCount = deliveryCount;
            MailoutIdentifier = mailoutIdentifier;
            MessageIdentifier = messageIdentifier;
            Age = age;
            IsOverdue = isOverdue;
            IsStarted = isStarted;
            IsCompleted = isCompleted;
        }

        public override bool Equals(object obj)
        {
            return obj is MailoutItem other &&
                   MailoutScheduled.Equals(other.MailoutScheduled) &&
                   EqualityComparer<DateTimeOffset?>.Default.Equals(MailoutStarted, other.MailoutStarted) &&
                   EqualityComparer<DateTimeOffset?>.Default.Equals(MailoutCompleted, other.MailoutCompleted) &&
                   EqualityComparer<DateTimeOffset?>.Default.Equals(MailoutCancelled, other.MailoutCancelled) &&
                   MessageName == other.MessageName &&
                   MessageTitle == other.MessageTitle &&
                   DeliveryCount == other.DeliveryCount &&
                   MailoutIdentifier.Equals(other.MailoutIdentifier) &&
                   MessageIdentifier.Equals(other.MessageIdentifier) &&
                   Age == other.Age &&
                   IsOverdue == other.IsOverdue &&
                   IsStarted == other.IsStarted &&
                   IsCompleted == other.IsCompleted;
        }

        public override int GetHashCode()
        {
            int hashCode = -2046957412;
            hashCode = hashCode * -1521134295 + MailoutScheduled.GetHashCode();
            hashCode = hashCode * -1521134295 + MailoutStarted.GetHashCode();
            hashCode = hashCode * -1521134295 + MailoutCompleted.GetHashCode();
            hashCode = hashCode * -1521134295 + MailoutCancelled.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MessageName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MessageTitle);
            hashCode = hashCode * -1521134295 + DeliveryCount.GetHashCode();
            hashCode = hashCode * -1521134295 + MailoutIdentifier.GetHashCode();
            hashCode = hashCode * -1521134295 + MessageIdentifier.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Age);
            hashCode = hashCode * -1521134295 + IsOverdue.GetHashCode();
            hashCode = hashCode * -1521134295 + IsStarted.GetHashCode();
            hashCode = hashCode * -1521134295 + IsCompleted.GetHashCode();
            return hashCode;
        }
    }
}