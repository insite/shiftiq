using System;
using System.Text;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Events.Classes.Controls;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Registrations.Forms
{
    public partial class Card : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/classes/search";

        private Guid RegistrationIdentifier =>
            Guid.TryParse(Request["id"], out var result) ? result : Guid.Empty;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/edit") ? $"id={RegistrationIdentifier}" : null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var registration = ServiceLocator.RegistrationSearch.GetRegistration(RegistrationIdentifier, x => x.Event.VenueLocation, x => x.Candidate);
            if (registration == null || registration.Event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            var @event = registration.Event;
            var scheduledDate = @event.EventScheduledEnd.HasValue && @event.EventScheduledEnd.Value.Date != @event.EventScheduledStart.Date
                ? $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone)} - {@event.EventScheduledEnd.Value.FormatDateOnly(User.TimeZone)}"
                : $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}";

            PageHelper.AutoBindHeader(
                Page,
                qualifier: registration.Event.EventTitle
                + " <span class=form-text>for</span> "
                + registration.Candidate.UserFullName
                + $" <span class='form-text'>Scheduled: {scheduledDate}</span>");

            ClassSummaryInfo.Bind(@event);
            ClassLocationInfo.Bind(@event);

            var candidateUser = UserSearch.Select(registration.CandidateIdentifier);
            var candidatePerson = candidateUser != null
                ? PersonSearch.Select(Organization.OrganizationIdentifier, candidateUser.UserIdentifier, x => x.HomeAddress)
                : null;

            BindPerson(candidateUser, candidatePerson);
            BindCompanyDetails(registration.EmployerIdentifier);

            NumberWorkHoursToDate.Text = registration.WorkBasedHoursToDate.HasValue ? registration.WorkBasedHoursToDate.ToString() : "N/A";
            RegistrationFee.Text = $"{registration.RegistrationFee:c2}";
            BillingCustomer.Text = !string.IsNullOrEmpty(registration.BillingCustomer) ? registration.BillingCustomer : "None";

            if (registration.SeatIdentifier != null)
                BindSeat(registration.SeatIdentifier);
            else
                SeatSection.Visible = false;

            CancelButton.NavigateUrl = GetReturnUrl();
        }

        private void BindPerson(User candidateUser, Person candidatePerson)
        {
            LearnerIdNumberField.Visible = Organization.OrganizationIdentifier == OrganizationIdentifiers.RCABC;

            FullName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={candidateUser.UserIdentifier}\">{candidateUser.FullName}</a>";
            Email.Text = string.IsNullOrEmpty(candidateUser.Email) ? string.Empty : $"<a href=\"mailto:{candidateUser.Email.ToLower()}\">{candidateUser.Email.ToLower()}</a>";
            Birthdate.Text = $"{candidatePerson.Birthdate:MMM d, yyyy}";

            PersonCode.Text = string.IsNullOrEmpty(candidatePerson.PersonCode) ? "None" : candidatePerson.PersonCode;
            ESL.Checked = string.Equals(candidatePerson.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase) ? true : false;

            EmergencyContactName.Text = string.IsNullOrEmpty(candidatePerson.EmergencyContactName) ? "None" : candidatePerson.EmergencyContactName;
            EmergencyContactPhoneNumber.Text = string.IsNullOrEmpty(candidatePerson.EmergencyContactPhone) ? "None" : candidatePerson.EmergencyContactPhone;
            EmergencyContactRelationship.Text = string.IsNullOrEmpty(candidatePerson.EmergencyContactRelationship) ? "None" : candidatePerson.EmergencyContactRelationship;


            var phones = new StringBuilder();

            if (candidatePerson.Phone.HasValue())
                phones.AppendLine($"<div>Preferred: {candidatePerson.Phone}</div>");
            if (candidatePerson.PhoneHome.HasValue())
                phones.AppendLine($"<div>Home: {candidatePerson.PhoneHome}</div>");
            if (candidatePerson.PhoneWork.HasValue())
                phones.AppendLine($"<div>Work: {candidatePerson.PhoneWork}</div>");
            if (candidateUser.PhoneMobile.HasValue())
                phones.AppendLine($"<div>Cell: {candidateUser.PhoneMobile}</div>");
            if (candidatePerson.PhoneOther.HasValue())
                phones.AppendLine($"<div>Other: {candidatePerson.PhoneOther}</div>");


            if (phones.Length > 0)
                PhoneNumbers.Text = phones.ToString();
            else
                PhoneNumbers.Text = "None";

            if (candidatePerson.HomeAddress != null)
                HomeAddress.Text = ClassVenueAddressInfo.GetAddress(candidatePerson.HomeAddress);
            else
                HomeAddress.Text = "None";
        }

        private void BindCompanyDetails(Guid? employerIdentifier)
        {
            var employer = employerIdentifier.HasValue ? ServiceLocator.GroupSearch.GetGroup(employerIdentifier.Value) : null;

            if (employer != null)
            {
                var address = ServiceLocator.GroupSearch.GetAddress(employer.GroupIdentifier, AddressType.Shipping);

                var pN = new StringBuilder();

                if (employer.GroupName.HasValue())
                    pN.AppendLine($"<div><a href=\"/ui/admin/contacts/groups/edit?contact={employer.GroupIdentifier}\">{employer.GroupName}</a></div>");

                if (employer.GroupPhone.HasValue())
                    pN.AppendLine($"<div>Phone: {employer.GroupPhone}</div>");

                if (pN.Length > 0)
                    EmployerName.Text = pN.ToString();
                else
                    EmployerName.Text = "None";

                if (address != null)
                    EmployerMailingAddress.Text = ClassVenueAddressInfo.GetAddress(address);

                EmployerContactName.Text = "None";
                EmployerContactPhoneNumber.Text = "None";
                EmployerContactEmail.Text = "None";

                var membership = MembershipSearch.SelectFirst(x => x.GroupIdentifier == employer.GroupIdentifier && x.MembershipType == MembershipType.EmployerContact);
                if (membership != null)
                {
                    var manager = ServiceLocator.PersonSearch.GetPerson(membership.UserIdentifier, CurrentSessionState.Identity.Organization.Identifier, x => x.User);
                    if (manager != null)
                    {
                        EmployerContactName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={manager.UserIdentifier}\">{manager.User.FullName}</a>";
                        EmployerContactEmail.Text = $"<a href=\"mailto:{manager.User.Email.ToLower()}\">{manager.User.Email.ToLower()}</a>";

                        if (!string.IsNullOrEmpty(manager.Phone))
                            EmployerContactPhoneNumber.Text = manager.Phone;
                    }
                }

            }
            else
            {
                EmployerSection.Visible = false;
            }
        }

        private void BindSeat(Guid? seatIdentifier)
        {
            var seat = seatIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetSeat(seatIdentifier.Value)
                : null;

            if (seat == null)
            {
                SeatName.Text = "None";
                SeatAgreement.Visible = false;
                return;
            }

            SeatName.Text = seat.SeatTitle;

            var content = ContentSeat.Deserialize(seat.Content);
            var agreement = content.Get("Agreement")?.Default;

            AgreementText.Text = agreement.IsNotEmpty() ? agreement.Replace("\n", "<br>") : null;
            SeatAgreement.Visible = AgreementText.Text.IsNotEmpty();
            BillingCustomerField.Visible = BillingCustomer.Text.IsNotEmpty();
        }
    }
}