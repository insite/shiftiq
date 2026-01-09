using System;
using System.Collections.Generic;

using InSite.Application.Contacts.Read;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Events.Classes.Controls
{
    public partial class ConfirmPayment : System.Web.UI.UserControl
    {
        public void SetCreditCardVisibility(bool visible)
        {
            CreditCardPanel.Visible = visible;
        }

        public void SetPayment(PaymentDetail.Payment payment)
        {
            PaymentAmountLiteral.Text = payment.PaymentAmount.ToString("n2");
            CardDetailConfirm.SetInputValues(payment.Card);
        }

        public void ShowConfirmationDetails(
            Guid eventIdentifier,
            QGroup employer,
            QGroupAddress employerAddress,
            string employerContactEmail,
            Guid seatIdentifier,
            string billingCustomer,
            List<PersonItem> participants
            )
        {
            var @event = ServiceLocator.EventSearch.GetEvent(eventIdentifier, x => x.VenueLocation);
            var eventContent = ContentEventClass.Deserialize(@event.Content);
            var timeZone = CurrentSessionState.Identity.User.TimeZone;
            var classStartDate = @event.EventScheduledStart.FormatDateOnly(timeZone);
            var classEndDate = @event.EventScheduledEnd.HasValue ? @event.EventScheduledEnd.Value.FormatDateOnly(timeZone) : "N/A";
            var classCancellationPolicy = eventContent.Get(EventInstructionType.Cancellation.GetName())?.Default;
            var classCancellationPolicyText = !string.IsNullOrEmpty(classCancellationPolicy) ? Markdown.ToHtml(classCancellationPolicy) : null;
            var venueAddress = VenueAddress.GetAddress(@event.VenueLocationIdentifier, AddressType.Physical) ?? "N/A";
            var employerAddressText = employerAddress != null ? VenueAddress.GetAddress(employerAddress) : "N/A";
            var isPhoneVisible = !string.IsNullOrEmpty(employer?.GroupPhone);

            var seat = ServiceLocator.EventSearch.GetSeat(seatIdentifier);
            var seatContent = ContentSeat.Deserialize(seat.Content);
            var seatAgreement = seatContent.GetAgreementHtml();

            ConfirmClassName.Text = @event.EventTitle;
            ConfirmClassDates.Text = $"{classStartDate} - {classEndDate}";
            ConfirmClassVenue.Text = @event.VenueLocationName ?? "N/A";
            ConfirmClassVenueAddress.Text = venueAddress;

            ConfirmEmployerName.Text = employer?.GroupName ?? "N/A";
            ConfirmEmployerEmail.Text = employerContactEmail;
            ComfirmEmployerEmailDiv.Visible = !string.IsNullOrEmpty(employerContactEmail);
            ComfirmEmployerEmailDiv.Attributes["class"] = !string.IsNullOrEmpty(employerAddressText) ? "d-flex pb-3 border-bottom" : "d-flex pt-2";
            ConfirmEmployerAddress.Text = employerAddressText;
            ConfirmEmployerAddressDiv.Visible = !string.IsNullOrEmpty(employerAddressText);
            ConfirmEmployerAddressDiv.Attributes["class"] = isPhoneVisible ? "d-flex pb-3 border-bottom" : "d-flex pt-2";

            ConfirmEmployerPhone.Text = employer?.GroupPhone;
            ComfirmEmployerPhonesDiv.Visible = isPhoneVisible;

            ComfirmEmployerPhonesDiv.Visible = isPhoneVisible;

            ConfirmSeatName.Text = seat.SeatTitle;
            ConfirmSeatAgreement.Text = seatAgreement;
            BillingCustomerDiv.Visible = !string.IsNullOrEmpty(billingCustomer);
            //ConfirmSeatPaidBy.Text = !string.IsNullOrEmpty(BillingCustomer.SelectedValue) ? BillingCustomer.SelectedValue : "N/A";

            ConfirmClassRefundPanel.Visible = !string.IsNullOrEmpty(classCancellationPolicyText);
            ConfirmClassRefundContent.Text = !string.IsNullOrEmpty(classCancellationPolicyText) ? classCancellationPolicyText : "N/A";

            ParticipantRepeater.DataSource = participants;
            ParticipantRepeater.DataBind();
        }
    }
}