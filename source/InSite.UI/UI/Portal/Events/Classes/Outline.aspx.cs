using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Admin.Invoices.Controls;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Events.Classes.Models;
using InSite.Web.Helpers;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes
{
    public partial class Outline : PortalBasePage
    {
        #region Properties

        private Guid? EventID => Guid.TryParse(Request["event"], out var eventID) ? eventID : (Guid?)null;

        protected bool ShowFirstPriceColumn { get; set; }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SeatRepeater.ItemDataBound += SeatRepeater_ItemDataBound;
            PrintReceiptButton.Click += PrintReceiptButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();

            var status = GetStatus();
            if (status != default)
                StatusAlert.AddMessage(status.Type, status.Message);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!IsPostBack)
                RemoveStatus();

            base.OnPreRender(e);
        }

        #endregion

        #region Methods (load)

        private void LoadData()
        {
            if (EventID == null)
                HttpResponseHelper.Redirect("/ui/portal/events/classes/search");

            var @event = ServiceLocator.EventSearch.GetEvent(EventID.Value, x => x.VenueLocation, x => x.Registrations);

            if (@event == null || @event.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect("/ui/portal/events/classes/search");

            var allowedForRegistrationByAnyone = @event.AllowRegistrationWithLink.HasValue && @event.AllowRegistrationWithLink.Value;

            if (TGroupPermissionSearch.IsAccessDenied(EventID.Value, Identity) && !allowedForRegistrationByAnyone)
                HttpResponseHelper.Redirect("/ui/portal/events/classes/search");

            RegisterButton.NavigateUrl = $"/ui/portal/events/classes/register?event={EventID}";
            RegisterEmployeeLink.NavigateUrl = $"/ui/portal/events/classes/register-employee?event={EventID}";
            AddEmployeeToWaitingListLink.NavigateUrl = $"/ui/portal/events/classes/register-employee?event={EventID}";
            AddToWaitingListButton.NavigateUrl = $"/ui/portal/events/classes/add-to-waiting-list?event={EventID}&candidate={User.Identifier}";

            SetInputValues(@event);

            CheckRegistrationStatus(@event);

            BindSeats(@event.EventIdentifier);
        }

        private void CheckRegistrationStatus(QEvent @event)
        {
            AlertClosed.Visible = false;
            AlertWaiting.Visible = false;
            AlertRegistered.Visible = false;
            AlertInvited.Visible = false;
            AlertInvitationExpired.Visible = false;
            AlertMoved.Visible = false;
            AlertCancelled.Visible = false;
            AlertFull.Visible = false;
            RegisterButton.Visible = false;
            AddToWaitingListButton.Visible = false;
            RegisterEmployeeLink.Visible = false;
            AddEmployeeToWaitingListLink.Visible = false;
            PrintReceiptButton.Visible = false;

            var checkResult = RegistrationHelper.CheckClass(@event);

            if (checkResult == CheckClassResult.RegistrationEnded)
            {
                AlertClosed.Visible = true;
                ClosedDetails.Text = $"{Translate("The registration deadline for this class was")} <strong>{@event.RegistrationDeadline.Value.FormatDateOnly(User.TimeZone)}</strong>";
                return;
            }
            else if (checkResult == CheckClassResult.ClassStarted)
            {
                AlertClosed.Visible = true;
                ClosedDetails.Text = $"{Translate("The event started on")} <strong>{@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</strong>";
                return;
            }

            if (checkResult == CheckClassResult.RegistrationNotStarted)
            {
                AlertClosed.Visible = true;
                ClosedDetails.Text = $"{Translate("The registration for this class will start on")} <strong>{@event.RegistrationStart.Value.FormatDateOnly(User.TimeZone)}</strong>";
                return;
            }

            var hasSelfRegistration = CheckSelfRegistrationStatus(@event);

            if (checkResult != CheckClassResult.ClassClosed)
            {
                if (checkResult == CheckClassResult.ClassOpen)
                {
                    RegisterButton.Visible = !hasSelfRegistration;
                    RegisterEmployeeLink.Visible = (Identity.Organization.Toolkits.Events?.AllowUsersRegisterEmployees) ?? false;
                }
                else
                {
                    AlertFull.Visible = !hasSelfRegistration;
                    WaitlistNote.Visible = @event.WaitlistEnabled;
                    AddToWaitingListButton.Visible = !hasSelfRegistration && @event.WaitlistEnabled;
                    AddEmployeeToWaitingListLink.Visible = @event.WaitlistEnabled;
                }
            }
        }

        private bool CheckSelfRegistrationStatus(QEvent @event)
        {
            var registration = @event.Registrations.FirstOrDefault(x => x.CandidateIdentifier == User.UserIdentifier);
            if (registration == null)
                return false;

            if (string.Equals(registration.AttendanceStatus, "Withdrawn/Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                AlertCancelled.Visible = true;
            }
            else if (string.Equals(registration.ApprovalStatus, "Waitlisted", StringComparison.OrdinalIgnoreCase))
            {
                AlertWaiting.Visible = true;
            }
            else if (string.Equals(registration.ApprovalStatus, "Registered", StringComparison.OrdinalIgnoreCase))
            {
                AlertRegistered.Visible = true;
                PrintReceiptButton.Visible = registration.PaymentIdentifier.HasValue;
                RegistrationCompetedCard.Visible = true;
                RegistrationCompetedCard.Visible = !string.IsNullOrEmpty(RegistrationCompeted.Text);

            }
            else if (string.Equals(registration.ApprovalStatus, "Invitation Sent", StringComparison.OrdinalIgnoreCase))
            {
                if (RegistrationInvitationHelper.IsInvitationValid(registration.RegistrationIdentifier, ServiceLocator.RegistrationSearch))
                    AlertInvited.Visible = true;
                else
                    AlertInvitationExpired.Visible = true;
            }
            else if (string.Equals(registration.ApprovalStatus, "Moved", StringComparison.OrdinalIgnoreCase))
            {
                AlertMoved.Visible = true;
            }

            return true;
        }

        private void SetInputValues(QEvent @event)
        {
            PageHelper.AutoBindHeader(this);

            var scheduledDate = @event.EventScheduledEnd.HasValue && @event.EventScheduledEnd.Value.Date != @event.EventScheduledStart.Date
                ? $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone)} - {@event.EventScheduledEnd.Value.FormatDateOnly(User.TimeZone)}"
                : $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}";

            PortalMaster.Breadcrumbs.BindTitleAndSubtitleNoTranslate(
                @event.EventTitle,
                Translate("Scheduled ") + scheduledDate);

            var content = ContentEventClass.Deserialize(@event.Content);
            var contactInstruction = content.Get(EventInstructionType.Contact.GetName());
            var accommodationInstruction = content.Get(EventInstructionType.Accommodation.GetName());
            var additionalInstruction = content.Get(EventInstructionType.Additional.GetName());
            var cancellationInstruction = content.Get(EventInstructionType.Cancellation.GetName());
            var registrationCompetedInstruction = content.Get(EventInstructionType.Completion.GetName());

            var summary = content.Summary != null && content.Summary.Default.HasValue() ? Markdown.ToHtml(content.Summary.Default) : string.Empty;
            SummaryPanel.Visible = !string.IsNullOrEmpty(summary);
            Summary.Text = summary;

            Date.Text = GetEventDateAndTimeText(@event.EventScheduledStart, @event.EventScheduledEnd);

            if (@event.EventScheduledEnd.HasValue)
            {
                var start = TimeZones.GetDate(@event.EventScheduledStart, User.TimeZone);
                var end = TimeZones.GetDate(@event.EventScheduledEnd.Value, User.TimeZone);
                var title = @event.EventTitle;
                var location = @event.VenueLocationName;
                var helper = new CalendarHelper();

                AddToGoogleLink.Text = helper.GenerateGoogleCalendarLink(start, end, title, location);
                AddToOfficeLink.Text = helper.GenerateOffice365CalendarLink(start, end, title, location);
                DownloadIcsLink.Text = helper.GenerateIcsDownloadLink(start, end, title, location);
            }

            RegistrationStartField.Visible = @event.RegistrationStart.HasValue;
            RegistrationStart.Text = @event.RegistrationStart.Format(null, true);

            RegistrationDeadline.Text = @event.RegistrationDeadline.HasValue
                ? @event.RegistrationDeadline.Format(null, true)
                : @event.EventScheduledStart.Format(User.TimeZone, true);

            Venue.BindVenue(@event, "Location", "Venue");

            var description = content.Description != null && content.Description.Default.HasValue() ? Markdown.ToHtml(content.Description.Default) : string.Empty;
            DescriptionPanel.Visible = !string.IsNullOrEmpty(description);
            Description.Text = description;

            var materialsForParticipation = content.MaterialsForParticipation != null && content.MaterialsForParticipation.Default.HasValue()
                ? Markdown.ToHtml(content.MaterialsForParticipation.Default)
                : string.Empty;

            MaterialsForParticipationCard.Visible = !string.IsNullOrEmpty(materialsForParticipation);
            MaterialsForParticipation.Text = materialsForParticipation;

            var contactInstructionText = contactInstruction != null && contactInstruction.Default.HasValue() ? Markdown.ToHtml(contactInstruction.Default) : string.Empty;
            ContactPanel.Visible = !string.IsNullOrEmpty(contactInstructionText);
            ContactInstruction.Text = contactInstructionText;

            var accommodationInstructionText = accommodationInstruction != null && accommodationInstruction.Default.HasValue() ? Markdown.ToHtml(accommodationInstruction.Default) : string.Empty;
            AccommodationsPanel.Visible = !string.IsNullOrEmpty(accommodationInstructionText);
            AccommodationInstruction.Text = accommodationInstructionText;

            var additionalInstructionText = additionalInstruction != null && additionalInstruction.Default.HasValue() ? Markdown.ToHtml(additionalInstruction.Default) : string.Empty;
            AdditionalInstructionPanel.Visible = !string.IsNullOrEmpty(additionalInstructionText);
            AdditionalInstruction.Text = additionalInstructionText;

            var cancellationInstructionText = cancellationInstruction != null && cancellationInstruction.Default.HasValue() ? Markdown.ToHtml(cancellationInstruction.Default) : string.Empty;
            CancellationInstructionPanel.Visible = !string.IsNullOrEmpty(cancellationInstructionText);
            CancellationInstruction.Text = cancellationInstructionText;

            var registrationCompetedText = registrationCompetedInstruction != null && registrationCompetedInstruction.Default.HasValue() ? Markdown.ToHtml(registrationCompetedInstruction.Default) : string.Empty;
            RegistrationCompeted.Text = registrationCompetedText;

            var localeDate = TimeZoneInfo.ConvertTime(@event.EventScheduledStart, User.TimeZone).Date;
            ReturnToCalendar.NavigateUrl = $"/ui/portal/events/calendar?date={localeDate.ToShortDateString()}";
            ReturnToCalendar.Visible = !(Identity.Organization.Toolkits.Events?.HideReturnToCalendar ?? false);
        }

        private string GetEventDateAndTimeText(DateTimeOffset start, DateTimeOffset? end)
        {
            if (end == null)
                return $"{start.Format(User.TimeZone, true)}";

            return start.Date != end.Value.Date
                ? $"{start.Format(User.TimeZone, true)} - {end.Value.Format(User.TimeZone, true)}"
                : $"{start.Format(User.TimeZone, true)}<span class='form-text text-body-secondary'> - {end.Value.FormatTimeOnly(User.TimeZone)}</span>";
        }

        private void BindSeats(Guid eventIdentifier)
        {
            var seats = ServiceLocator.EventSearch
                .GetSeats(eventIdentifier, false);

            if (AreSeatPricesEqual(seats))
            {
                ShowFirstPriceColumn = false;
                seats.RemoveRange(0, seats.Count - 1);
            }
            else
            {
                ShowFirstPriceColumn = seats.Count > 1;
            }

            PricesPanel.Visible = seats.Count > 0;

            SeatRepeater.DataSource = seats;
            SeatRepeater.DataBind();
        }

        private static bool AreSeatPricesEqual(List<QSeat> seats)
        {
            if (seats.Count <= 1)
                return false;

            var first = JsonConvert.DeserializeObject<SeatConfiguration>(seats[0].Configuration);

            if (first.Prices == null)
                return false;

            var firstPrices = first.Prices.OrderBy(x => x.Amount).ToList();

            for (int i = 1; i < seats.Count; i++)
            {
                var current = JsonConvert.DeserializeObject<SeatConfiguration>(seats[i].Configuration);
                if (current.Prices == null || current.Prices.Count != first.Prices.Count)
                    return false;

                var currentPrices = current.Prices.OrderBy(x => x.Amount).ToList();
                for (int k = 0; k < currentPrices.Count; k++)
                {
                    if (currentPrices[k].Amount != firstPrices[k].Amount)
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Event handlers

        private void SeatRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var seat = (QSeat)e.Item.DataItem;
            var configuration = JsonConvert.DeserializeObject<SeatConfiguration>(seat.Configuration);

            var freePriceLiteral = e.Item.FindControl("FreePrice");
            freePriceLiteral.Visible = false;

            if (configuration.Prices == null)
            {
                freePriceLiteral.Visible = true;
            }
            else if (configuration.Prices.Count == 1 && !configuration.Prices[0].Name.HasValue())
            {
                var singlePriceLiteral = (Literal)e.Item.FindControl("SinglePrice");
                singlePriceLiteral.Text = $"{configuration.Prices[0].Amount:c2}";
            }
            else
            {
                var multiplePriceRepeater = (Repeater)e.Item.FindControl("MultiplePrice");

                multiplePriceRepeater.DataSource = configuration.Prices;
                multiplePriceRepeater.DataBind();
            }
        }

        private void PrintReceiptButton_Click(object sender, EventArgs e)
        {
            var registration = ServiceLocator.RegistrationSearch.GetRegistration(new QRegistrationFilter { EventIdentifier = EventID, CandidateIdentifier = User.UserIdentifier });
            if (registration != null)
            {
                var data = InvoiceEventReport.PrintByRegistration(registration.RegistrationIdentifier, InvoiceEventReportType.Receipt);

                Response.SendFile("Receipt", "pdf", data);
            }
        }

        #endregion

        #region Methods (helpers)

        protected static string GetDescription(object item)
        {
            var seat = (QSeat)item;
            return ContentSeat.Deserialize(seat.Content).Description.Default;
        }

        [Serializable]
        private class ExternalStatusList : List<(Guid ID, AlertType Type, string Message)>
        {

        }

        private static readonly string ExternalStatusListSessionId = typeof(Outline).FullName + ".ExternalStatus";

        private static ExternalStatusList GetExternalStatusList()
        {
            return (ExternalStatusList)(HttpContext.Current.Session[ExternalStatusListSessionId]
                ?? (HttpContext.Current.Session[ExternalStatusListSessionId] = new ExternalStatusList()));
        }

        internal static Guid SetStatus(AlertType type, string message)
        {
            var id = Guid.NewGuid();
            var list = GetExternalStatusList();

            list.Add((id, type, message));

            return id;
        }

        private (AlertType Type, string Message) GetStatus()
        {
            if (!Guid.TryParse(Request["status"], out var id))
                return default;

            var list = GetExternalStatusList();
            var item = list.Find(x => x.ID == id);

            return (item.Type, item.Message);
        }

        private void RemoveStatus()
        {
            if (!Guid.TryParse(Request["status"], out var id))
                return;

            var list = GetExternalStatusList();
            list.RemoveAll(x => x.ID == id);
        }

        #endregion
    }
}