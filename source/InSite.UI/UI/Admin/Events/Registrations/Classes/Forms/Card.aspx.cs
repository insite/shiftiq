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
            if (candidateUser != null)
            {
                var candidatePerson = PersonSearch.Select(Organization.OrganizationIdentifier, candidateUser.UserIdentifier, x => x.HomeAddress);
                BindPerson(candidateUser, candidatePerson);
            }
            else
            {
                ScreenStatus.AddMessage(AlertType.Error, $"User record not found.");
            }

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

        private void BindPerson(User user, Person person)
        {
            const string NoneValue = "None";

            var hasPerson = person != null;
            if (!hasPerson)
                ScreenStatus.AddMessage(AlertType.Warning, $"{user.FullName} is not a member of {Organization.CompanyName}.");

            LearnerIdNumberField.Visible = Organization.OrganizationIdentifier == OrganizationIdentifiers.RCABC;

            FullName.Text = hasPerson
                ? $"<a href=\"/ui/admin/contacts/people/edit?contact={user.UserIdentifier}\">{user.FullName}</a>"
                : user.FullName;
            Email.Text = $"<a href=\"mailto:{user.Email.ToLower()}\">{user.Email.ToLower()}</a>";
            Birthdate.Text = hasPerson && person.Birthdate.HasValue ? $"{person.Birthdate:MMM d, yyyy}" : NoneValue;

            PersonCode.Text = hasPerson ? person.PersonCode.IfNullOrEmpty(NoneValue) : NoneValue;
            ESL.Checked = hasPerson && string.Equals(person.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase) ? true : false;

            EmergencyContactName.Text = hasPerson ? person.EmergencyContactName.IfNullOrEmpty(NoneValue) : NoneValue;
            EmergencyContactPhoneNumber.Text = hasPerson ? person.EmergencyContactPhone.IfNullOrEmpty(NoneValue) : NoneValue;
            EmergencyContactRelationship.Text = hasPerson ? person.EmergencyContactRelationship.IfNullOrEmpty(NoneValue) : NoneValue;

            var phones = new StringBuilder();

            if (hasPerson)
            {
                if (person.Phone.HasValue())
                    phones.AppendLine($"<div>Preferred: {person.Phone}</div>");
                if (person.PhoneHome.HasValue())
                    phones.AppendLine($"<div>Home: {person.PhoneHome}</div>");
                if (person.PhoneWork.HasValue())
                    phones.AppendLine($"<div>Work: {person.PhoneWork}</div>");
                if (user.PhoneMobile.HasValue())
                    phones.AppendLine($"<div>Cell: {user.PhoneMobile}</div>");
                if (person.PhoneOther.HasValue())
                    phones.AppendLine($"<div>Other: {person.PhoneOther}</div>");
            }

            if (phones.Length > 0)
                PhoneNumbers.Text = phones.ToString();
            else
                PhoneNumbers.Text = NoneValue;

            if (!hasPerson || person.HomeAddress == null)
                HomeAddress.Text = NoneValue;
            else
                HomeAddress.Text = ClassVenueAddressInfo.GetAddress(person.HomeAddress);
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