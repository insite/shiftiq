using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

using InSite.Admin.Invoices.Controls;
using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Domain.Invoices;
using InSite.Domain.Messages;
using InSite.Domain.Registrations;
using InSite.Persistence;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes.Models
{
    public static class RegistrationHelper
    {
        public static void CheckMandatorySurveyResponse(QEvent @event, Guid userId)
        {
            if (!@event.MandatorySurveyFormIdentifier.HasValue)
                return;

            var surveyForm = ServiceLocator.SurveySearch.GetSurveyForm(@event.MandatorySurveyFormIdentifier.Value);
            if (surveyForm == null)
                return;

            var sessions = ServiceLocator.SurveySearch.GetResponseSessions(new QResponseSessionFilter
            {
                SurveyFormIdentifier = @event.MandatorySurveyFormIdentifier.Value,
                RespondentUserIdentifier = userId,
            });

            if (sessions.IsNotEmpty() && sessions.Any(x => x.ResponseSessionCompleted.HasValue))
                return;

            var returnUrl = new ReturnUrl();
            var redirectUrl = returnUrl.GetRedirectUrl($"/form/{surveyForm.AssetNumber}/{userId}");

            HttpResponseHelper.Redirect(redirectUrl);
        }

        public static CheckClassResult CheckClass(QEvent @event)
        {
            if (@event.RegistrationDeadline.HasValue)
            {
                if (@event.RegistrationDeadline <= DateTimeOffset.UtcNow)
                    return CheckClassResult.RegistrationEnded;
            }
            else if (@event.EventScheduledStart <= DateTimeOffset.UtcNow)
            {
                return CheckClassResult.ClassStarted;
            }

            if (@event.RegistrationStart.HasValue)
            {
                if (@event.RegistrationStart > DateTimeOffset.UtcNow)
                    return CheckClassResult.RegistrationNotStarted;
            }

            if (string.Equals(@event.EventPublicationStatus, PublicationStatus.Published.GetDescription(), StringComparison.OrdinalIgnoreCase))
            {
                return @event.Availability != EventAvailabilityType.Full && @event.Availability != EventAvailabilityType.Over
                    ? CheckClassResult.ClassOpen
                    : CheckClassResult.ClassFull;
            }

            return CheckClassResult.ClassClosed;
        }

        public static void SendPersonCodeNotEnteredAlert(QUser user, string eventTitle, RegistrantChangedField[] changedFields)
        {
            var orgId = CurrentSessionState.Identity.Organization.Identifier;
            var registrantUser = CurrentSessionState.Identity.User;
            var variables = BuildVariableList();

            try
            {
                var alert = new AlertPersonCodeNotEntered { PersonCodeNotEnteredProperties = variables };
                ServiceLocator.AlertMailer.Send(orgId, CurrentSessionState.Identity.User.Identifier, alert);
            }
            catch (MessageNotFoundException)
            {
            }


            StringDictionary BuildVariableList()
            {
                var candidate = PersonSearch.Select(orgId, user.UserIdentifier, x => x.User, x => x.HomeAddress);

                var dict = new StringDictionary
                {
                    { "EventTitle", eventTitle },
                    { "CandidateName", candidate.User.FullName },
                    { "CandidateEmail", candidate.User.Email },
                    { "CandidatePhone", candidate.Phone ?? "None" },
                    { "RegistrantEmail", registrantUser.Email },
                    { "RegistrantName", registrantUser.FullName },
                    { "RegistrantPhone", registrantUser.Phone != null ? registrantUser.Phone.ToString() : "n/a" }
                };

                return dict;
            }
        }

        public static void SendRegistratrionCompleteAlert(Guid registrationId, Guid registeredUserId)
        {
            var user = CurrentSessionState.Identity.User;
            var userId = user.Identifier;
            var orgId = CurrentSessionState.Identity.Organization.Identifier;
            var registration = ServiceLocator.RegistrationSearch.GetRegistration(registrationId);
            var report = registration.PaymentIdentifier.HasValue ? CreateInvoiceReport(registrationId) : null;
            var variables = BuildVariableList();

            try
            {
                var alert = new AlertRegistrationComplete { RegistrationCompleteProperties = variables };
                var attachments = report != null ? new string[] { report } : null;
                var cc = registration.RegistrationRequestedBy.HasValue && registration.RegistrationRequestedBy.Value != registeredUserId
                    ? new[] { registration.RegistrationRequestedBy.Value }
                    : null;
                var bcc = userId != registeredUserId
                    ? new Guid[] { userId, registeredUserId }
                    : new Guid[] { userId };

                ServiceLocator.AlertMailer.Send(orgId, userId, alert, cc, bcc, attachments);
            }
            catch (MessageNotFoundException)
            {
            }

            StringDictionary BuildVariableList()
            {
                var @event = ServiceLocator.EventSearch.GetEvent(registration.EventIdentifier, x => x.VenueLocation);
                var content = ContentEventClass.Deserialize(@event.Content);
                var seat = registration.SeatIdentifier.HasValue ? ServiceLocator.EventSearch.GetSeat(registration.SeatIdentifier.Value) : null;
                var payment = registration.PaymentIdentifier.HasValue ? ServiceLocator.PaymentSearch.GetPayment(registration.PaymentIdentifier.Value) : null;
                var candidate = PersonSearch.Select(orgId, registration.CandidateIdentifier, x => x.User, x => x.HomeAddress);

                var employer = candidate.EmployerGroupIdentifier.HasValue ? ServiceLocator.GroupSearch.GetGroup(candidate.EmployerGroupIdentifier.Value) : null;

                var employerAddress = employer != null
                    ? ServiceLocator.GroupSearch.GetAddress(employer.GroupIdentifier, AddressType.Shipping)
                    : null;

                var venueAddress = @event.VenueLocationIdentifier.HasValue
                    ? ServiceLocator.GroupSearch.GetAddress(@event.VenueLocationIdentifier.Value, AddressType.Physical)
                    : null;

                string invoiceNumber = "None";
                if (payment != null)
                {
                    var invoice = ServiceLocator.InvoiceSearch.GetInvoice(payment.InvoiceIdentifier);
                    invoiceNumber = invoice?.InvoiceNumber != null
                        ? Invoice.FormatInvoiceNumber(invoice.InvoiceNumber.Value)
                        : invoice.InvoiceIdentifier.ToString();
                }

                Person manager = null;
                string employerStatus = null;

                if (employer != null)
                {
                    var managerReference = MembershipSearch.SelectFirst(x => x.GroupIdentifier == employer.GroupIdentifier && x.MembershipType == MembershipType.EmployerContact);
                    if (managerReference != null)
                        manager = PersonSearch.Select(orgId, managerReference.UserIdentifier, x => x.User);

                    employerStatus = TCollectionItemCache.GetName(employer.GroupStatusItemIdentifier);
                }

                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(candidate.User.TimeZone);
                var eventDate = @event.EventScheduledEnd.HasValue
                    ? $"{@event.EventScheduledStart.FormatDateOnly(timeZone)} - {@event.EventScheduledEnd.Value.FormatDateOnly(timeZone)}"
                    : $"{@event.EventScheduledStart.FormatDateOnly(timeZone)}";

                var eventTime = @event.EventScheduledEnd.HasValue
                    ? $"{@event.EventScheduledStart.FormatTimeOnly(timeZone)} - {@event.EventScheduledEnd.Value.FormatTimeOnly(timeZone)}"
                    : $"{@event.EventScheduledStart.FormatTimeOnly(timeZone)}";

                var dict = new StringDictionary
                {
                    { "CandidateName", candidate.User.FullName },
                    { "CandidateEmail", candidate.User.Email },
                    { "CandidateStreet1", candidate.HomeAddress?.Street1 },
                    { "CandidateCity", candidate.HomeAddress?.City },
                    { "CandidateProvince", candidate.HomeAddress?.Province },
                    { "CandidatePostalCode", candidate.HomeAddress?.PostalCode },
                    { "EmployerStatus", employerStatus ?? "Unknown" },
                    { "CandidateBirthdate", candidate.Birthdate.HasValue ? $"{candidate.Birthdate:MM/dd/yyyy}" : "Unknown" },
                    { "CandidateTradeworkerNumber", candidate.TradeworkerNumber },
                    { "CandidateWorkBasedHoursToDate", registration.WorkBasedHoursToDate.HasValue ? $"{registration.WorkBasedHoursToDate:n0}" : "None" },
                    { "CandidatePhone", candidate.Phone ?? "None" },
                    { "CandidateIsEnglishFirstLanguage", string.Equals(candidate.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase) ? "No" : "Yes" },
                    { "EmergencyContactName", candidate.EmergencyContactName },
                    { "EmergencyContactPhone", candidate.EmergencyContactPhone },
                    { "EmergencyContactRelationship", candidate.EmergencyContactRelationship },
                    { "EmployerName", employer?.GroupName ?? "None" },
                    { "EmployerContactName", manager?.User?.FullName ?? "None" },
                    { "EmployerContactPhone", manager?.Phone ?? "None" },
                    { "EmployerContactEmail", manager?.User?.Email ?? "None" },
                    { "EmployerStreet1", employerAddress?.Street1 },
                    { "EmployerCity", employerAddress?.City },
                    { "EmployerProvince", employerAddress?.Province },
                    { "EmployerPostalCode", employerAddress?.PostalCode },
                    { "InvoiceNumber", invoiceNumber },
                    { "EventTitle", @event.EventTitle },
                    { "EventDate", eventDate },
                    { "EventTime", eventTime },
                    { "VenueName", @event.VenueLocation?.GroupName },
                    { "VenueStreet1", venueAddress?.Street1 },
                    { "VenueCity", venueAddress?.City },
                    { "VenueProvince", venueAddress?.Province },
                    { "VenuePostalCode", venueAddress?.PostalCode },
                    { "SeatTitle", seat?.SeatTitle ?? "Free" },
                    { "SeatPrice", $"{registration.RegistrationFee ?? 0:n2}" },
                    { "RegistrantEmail", user.Email },
                    { "RegistrantName", user.FullName },
                    { "RegistrantPhone", user.Phone != null ? user.Phone.ToString() : "n/a" },
                    { "CandidateAuthenticationDetails", GenerateCandidateAuthenticationDetailsMarkdown(candidate.User.FullName, candidate.PersonCode, registration.RegistrationPassword) },
                    { "PersonCode", candidate.PersonCode },
                    { "MeetingLink", content.ClassLink.Default }
                };

                return dict;
            }
        }

        public static string GenerateCandidateAuthenticationDetailsMarkdown(string name, string code, string password)
        {
            var md = new StringBuilder();
            md.AppendLine("Name | Code | Password");
            md.AppendLine("-- | -- | --");
            md.AppendLine($"{name} | {code} | {password}");
            return md.ToString();
        }

        private static string CreateInvoiceReport(Guid registrationIdentifier)
        {
            var data = InvoiceEventReport.PrintByRegistration(registrationIdentifier, InvoiceEventReportType.Invoice);
            if (data == null)
                return null;

            string folder = ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Temp", "Invoices", Guid.NewGuid().ToString());

            var path = Path.Combine(folder, "Invoice.pdf");

            File.WriteAllBytes(path, data);

            return path;
        }

        internal static void AddLearnerToRegistrationGroup(Guid groupIdentifier, Guid userIdentifier, Guid organizationIdentifier)
        {
            if (!MembershipPermissionHelper.CanModifyMembership(groupIdentifier))
                return;

            MembershipHelper.Save(new Membership
            {
                GroupIdentifier = groupIdentifier,
                UserIdentifier = userIdentifier,
                Assigned = DateTimeOffset.UtcNow,
                OrganizationIdentifier = organizationIdentifier
            });
        }
    }
}