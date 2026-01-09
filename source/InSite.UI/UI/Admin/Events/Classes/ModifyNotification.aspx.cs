using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Domain.Events;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Events.Classes
{
    public partial class ModifyNotification : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/classes/search";

        private Guid EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : Guid.Empty;

        private string OutlineUrl => $"/ui/admin/events/classes/outline?event={EventIdentifier}&panel=notifications";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}&panel=notifications" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier, x => x.VenueLocation);
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled to start {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            SummaryInfo.Bind(@event);
            LocationInfo.Bind(@event);

            ReminderLearnerMessage.Filter.Type = MessageTypeName.Notification;
            ReminderLearnerMessage.Value = @event.WhenEventReminderRequestedNotifyLearnerMessageIdentifier;

            ReminderInstructorMessage.Filter.Type = MessageTypeName.Notification;
            ReminderInstructorMessage.Value = @event.WhenEventReminderRequestedNotifyInstructorMessageIdentifier;

            SendReminderBeforeDays.ValueAsInt = @event.SendReminderBeforeDays;

            CancelButton.NavigateUrl = OutlineUrl;
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var commands = new ICommand[]
            {
                new ConnectEventMessage(EventIdentifier, EventMessageType.ReminderLearner, ReminderLearnerMessage.Value),
                new ConnectEventMessage(EventIdentifier, EventMessageType.ReminderInstructor, ReminderInstructorMessage.Value),
                new ModifyEventMessagePeriod(EventIdentifier, SendReminderBeforeDays.ValueAsInt)
            };

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect(OutlineUrl);
        }
    }
}