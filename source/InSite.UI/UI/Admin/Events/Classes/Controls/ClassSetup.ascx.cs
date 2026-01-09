using System;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Events.Classes.Controls
{
    public partial class ClassSetup : BaseUserControl
    {
        private const string DefaultColor = "Info";

        public event EventHandler FormsShown;
        public event EventHandler FormsHidden;

        private Guid EventId
        {
            get => (Guid)ViewState[nameof(EventId)];
            set => ViewState[nameof(EventId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InstructorRepeater.ItemCommand += InstructorRepeater_ItemCommand;

            LockRegistrationButton.Click += LockRegistrationButton_Click;
            UnlockRegistrationButton.Click += UnlockRegistrationButton_Click;

            AssessmentList.FormsShown += (a, b) => FormsShown?.Invoke(a, b);
            AssessmentList.FormsHidden += (a, b) => FormsHidden?.Invoke(a, b);
        }

        private void InstructorRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                var instructorID = Guid.Parse((string)e.CommandArgument);
                HttpResponseHelper.Redirect($"/ui/admin/events/classes/delete-instructor?event={EventId}&instructor={instructorID}");
            }
        }

        private void LockRegistrationButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new LockEventRegistration(EventId));
            ReloadRegistrationLocked();
        }

        private void UnlockRegistrationButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new UnlockEventRegistration(EventId));
            ReloadRegistrationLocked();
        }

        public bool LoadData(QEvent ev, bool isPublished, bool canEdit)
        {
            var eventId = EventId = ev.EventIdentifier;
            var achievement = ev.AchievementIdentifier.HasValue
                ? ServiceLocator.AchievementSearch.GetAchievement(ev.AchievementIdentifier.Value)
                : null;

            EventTitle.Text = ev.EventTitle;
            RegistrationStart.Text = ev.RegistrationStart.Format(null, true, nullValue: "None");
            RegistrationDeadline.Text = ev.RegistrationDeadline.Format(null, true, nullValue: "None");
            AchievementTitle.Text = achievement != null ?
                $"<a href=\"/ui/admin/records/achievements/outline?id={achievement?.AchievementIdentifier}\">{achievement?.AchievementTitle} </a>" : "None";
            EventScheduledStart.Text = ev.EventScheduledStart.Format(null, true);
            EventScheduledEnd.Text = ev.EventScheduledEnd.Format(null, true, nullValue: "None");
            Duration.Text = ev.DurationQuantity.HasValue && ev.DurationUnit.HasValue() ? ev.DurationUnit.ToQuantity(ev.DurationQuantity.Value) : "None";
            Capacity.Text = GetCapacityHtml(ev);
            Waitlist.Text = ev.WaitlistEnabled ? "Enabled" : "Disabled";
            ClassCalendarColorBox.Text = ColorBoxHtml(ev.EventCalendarColor ?? DefaultColor);
            ClassCalendarColorName.Text = ColorName(ev.EventCalendarColor ?? DefaultColor);

            PersonCodeIsRequiredLabel.Text = $"Is {LabelHelper.GetLabelContentText("Person Code")} Required to register?";
            PersonCodeIsRequired.Text = ev.PersonCodeIsRequired ? "Yes" : "No";

            EnableEventBillingCode.Text = ev.BillingCodeEnabled ? "Yes" : "No";
            AllowMultipleRegistrations.Text = ev.AllowMultipleRegistrations ? "Yes" : "No";
            CreditHours.Text = ev.CreditHours.HasValue ? $"{ev.CreditHours:n2}" : "None";

            FormPublicationStatus.Visible = isPublished;

            SetupRegistrationLocked(ev);

            EventSchedulingStatus.Text = ev.GetClassStatus().GetDescription();

            //1 - Physical Address
            VenueInfo.ShowChangeLink = canEdit;
            VenueInfo.Bind(eventId, ev.VenueLocation, AddressType.Physical);

            VenueRoomField.Visible = ev.VenueRoom.HasValue();
            VenueRoom.Text = ev.VenueRoom;

            var showForms = AssessmentList.Bind(eventId, canEdit);

            InstructorRepeater.DataSource = ServiceLocator.EventSearch.GetAttendees(
                new QEventAttendeeFilter { EventIdentifier = eventId, ContactRole = "Instructor" },
                null, null, x => x.Person.User);
            InstructorRepeater.DataBind();

            EventTitleLink.NavigateUrl = $"/ui/admin/events/classes/describe?event={eventId}&tab=Title";
            AchievementTitleLink.NavigateUrl = $"/ui/admin/events/classes/change-achievement?event={eventId}";
            ClassChangeVenue.NavigateUrl = $"/ui/admin/events/classes/change-venue?event={eventId}";

            ChangeEventScheduledStart.NavigateUrl = $"/ui/admin/events/classes/reschedule?event={eventId}";
            ChangeEventScheduledEnd.NavigateUrl = $"/ui/admin/events/classes/reschedule?event={eventId}";
            ChangeDuration.NavigateUrl = $"/ui/admin/events/classes/reschedule?event={eventId}";
            ChangeCreditHours.NavigateUrl = $"/ui/admin/events/classes/reschedule?event={eventId}";
            AssignInstructors.NavigateUrl = $"/ui/admin/events/classes/assign-instructor?event={eventId}";

            ClassLimitAttendance.NavigateUrl =
            WaitlistLink.NavigateUrl =
            EnableEventBillingCodeLink.NavigateUrl =
            PersonCodeIsRequiredLink.NavigateUrl =
            AllowMultipleRegistrationsLink.NavigateUrl =
                $"/ui/admin/events/classes/limit-capacity?event={eventId}";

            EventTitleLink.Visible = canEdit;
            AchievementTitleLink.Visible = canEdit;
            ClassChangeVenue.Visible = canEdit;
            ChangeEventScheduledStart.Visible = canEdit;
            ChangeEventScheduledEnd.Visible = canEdit;
            ChangeDuration.Visible = canEdit;
            ChangeCreditHours.Visible = canEdit;
            AssignInstructors.Visible = canEdit;
            ClassLimitAttendance.Visible = canEdit;

            return showForms;
        }

        private void ReloadRegistrationLocked()
        {
            var ev = ServiceLocator.EventSearch.GetEvent(EventId);
            SetupRegistrationLocked(ev);
        }

        private void SetupRegistrationLocked(QEvent ev)
        {
            var isLocked = ev.RegistrationLocked.HasValue;

            LockRegistrationButton.Visible = !isLocked;
            UnlockRegistrationButton.Visible = isLocked;
            RegistrationLocked.Visible = isLocked;
        }

        private static string GetCapacityHtml(QEvent @event)
        {
            string label;

            label = "No limitation";

            if ((@event.CapacityMinimum.HasValue) && (@event.CapacityMaximum.HasValue))
                label = $"{@event.CapacityMinimum} - {@event.CapacityMaximum}";
            else if (@event.CapacityMinimum.HasValue)
                label = $"from {@event.CapacityMinimum}";
            else if (@event.CapacityMaximum.HasValue)
                label = $"up to {@event.CapacityMaximum}";

            return label;
        }

        private static string ColorBoxHtml(string itemColor)
        {
            var indicator = itemColor.ToEnumNullable<Indicator>();
            return indicator.HasValue
                ? $"<i class='fas fa-square text-{indicator.GetContextualClass()} me-2'></i>"
                : null;
        }

        private static string ColorName(string itemColor)
        {
            var indicator = itemColor.ToEnumNullable<Indicator>();
            return indicator?.GetDescription();
        }
    }
}