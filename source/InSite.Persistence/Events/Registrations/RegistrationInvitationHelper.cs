using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Messages.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class RegistrationInvitationHelper
    {
        public const int InvitationExpiresInHours = 48;
        private const string InvitationReportType = "ClassRegisInvitation";
        private const string InvitationReportData = "Invitation";

        public static DateTimeOffset? GetInvitationSentTime(Guid registrationIdentifier, IRegistrationSearch registrationSearch)
        {
            var registration = registrationSearch.GetRegistration(registrationIdentifier);

            var registrationLink = GetRegistrationLink(registration.EventIdentifier);

            var existing = TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == registration.OrganizationIdentifier
                  && x.UserIdentifier == registration.CandidateIdentifier
                  && x.ReportType == InvitationReportType
                  && x.ReportDescription == registrationLink
                  && x.ReportData == InvitationReportData
            );

            return existing?.Created;
        }

        public static bool IsInvitationValid(Guid registrationIdentifier, IRegistrationSearch registrationSearch)
        {
            var sentTime = GetInvitationSentTime(registrationIdentifier, registrationSearch);

            var date = DateTimeOffset.UtcNow.AddHours(-InvitationExpiresInHours);

            return sentTime.HasValue && sentTime >= date;
        }

        public static QRegistration SendInvitation(
            Guid registrationIdentifier,
            Guid createdByUser,
            bool validateWhitelistedStatus,
            Action<ICommand> send,
            IRegistrationSearch registrationSearch
            )
        {
            var registration = registrationSearch.GetRegistration(registrationIdentifier, x => x.Event, y => y.Candidate, z => z.RegistrationRequestedByPerson);
            if (registration == null
                || validateWhitelistedStatus && !string.Equals(registration.ApprovalStatus, "Waitlisted", StringComparison.OrdinalIgnoreCase)
                )
            {
                return null;
            }

            var registrationLink = GetRegistrationLink(registration.EventIdentifier);

            SetupInvitation(registration, createdByUser, registrationLink);

            var reason = $"The invitation was triggered and will be expired in {InvitationExpiresInHours} hours";

            send(new ChangeApproval(registration.RegistrationIdentifier, "Invitation Sent", reason, null, registration.ApprovalStatus));

            return registration;
        }

        private static void SetupInvitation(QRegistration registration, Guid createdByUser, string registrationLink)
        {
            var existing = TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == registration.OrganizationIdentifier
                  && x.UserIdentifier == registration.CandidateIdentifier
                  && x.ReportType == InvitationReportType
                  && x.ReportDescription == registrationLink
            );

            if (existing != null)
                TReportStore.Delete(existing.ReportIdentifier);

            var report = new TReport
            {
                ReportIdentifier = UniqueIdentifier.Create(),
                OrganizationIdentifier = registration.OrganizationIdentifier,
                UserIdentifier = registration.CandidateIdentifier,
                ReportType = InvitationReportType,
                ReportTitle = "Registration Invitation",
                ReportDescription = registrationLink,
                ReportData = InvitationReportData,
                Created = DateTimeOffset.Now,
                CreatedBy = createdByUser,
                Modified = DateTimeOffset.Now,
                ModifiedBy = createdByUser
            };

            TReportStore.Insert(report);
        }

        public static void DeleteInvitation(
            Guid registrationId,
            string domain,
            EnvironmentModel environment,
            IRegistrationSearch registrationSearch,
            IAlertMailer mailer
            )
        {
            var registration = registrationSearch.GetRegistration(registrationId, x => x.Candidate, x => x.Event, x => x.RegistrationRequestedByPerson);
            var registrationLink = GetRegistrationLink(registration.EventIdentifier);

            var reports = TReportSearch.Select(
                organizationId: registration.OrganizationIdentifier,
                userId: registration.CandidateIdentifier,
                reportType: InvitationReportType,
                reportData: InvitationReportData,
                reportDescription: registrationLink
            );

            if (reports.Count == 0)
                return;

            SendInvitationExpiredAlert(domain, environment, mailer, registration);

            foreach (var report in reports)
                TReportStore.Delete(report.ReportIdentifier);
        }

        public static void DeleteExpiredInvitations(
            string domain,
            EnvironmentModel environment,
            Action<ICommand> send,
            IRegistrationSearch registrationSearch,
            IAlertMailer mailer
            )
        {
            var expiredReports = GetExpiredReports();
            if (expiredReports.Count == 0)
                return;

            var registrations = registrationSearch.GetRegistrations(new QRegistrationFilter { ApprovalStatus = "Invitation Sent" }
                , x => x.Candidate, x => x.Event, x => x.RegistrationRequestedByPerson);

            foreach (var registration in registrations)
            {
                var registrationLink = GetRegistrationLink(registration.EventIdentifier);

                var expiredReport = expiredReports.FirstOrDefault(x =>
                    x.OrganizationIdentifier == registration.OrganizationIdentifier
                    && x.UserIdentifier == registration.CandidateIdentifier
                    && x.ReportDescription == registrationLink
                );

                if (expiredReport == null)
                    continue;

                var command = new ChangeApproval(registration.RegistrationIdentifier, "Invitation Expired", "Invitation Expired", null, registration.ApprovalStatus);
                send(command);

                SendInvitationExpiredAlert(domain, environment, mailer, registration);
            }

            foreach (var report in expiredReports)
                TReportStore.Delete(report.ReportIdentifier);
        }

        private static void SendInvitationExpiredAlert(string domain, EnvironmentModel environment, IAlertMailer mailer, QRegistration registration)
        {
            var organization = OrganizationSearch.Select(registration.OrganizationIdentifier);
            if (organization == null)
                return;

            var classRegistrationLink = UrlHelper.GetAbsoluteUrl(
                domain,
                environment,
                $"ui/portal/events/classes/register?event={registration.EventIdentifier}",
                organization.OrganizationCode
            );

            mailer.Send(
                registration.OrganizationIdentifier,
                UserIdentifiers.Someone,
                new AlertRegistrationInvitationExpired
                {
                    CandidateFirstName = registration.Candidate.UserFirstName,
                    CandidateLastName = registration.Candidate.UserLastName,
                    CandidateEmail = registration.Candidate.UserEmail,
                    ClassTitle = registration.Event.EventTitle,
                    ClassRegistrationLink = classRegistrationLink
                }, registration.RegistrationRequestedByPerson?.UserIdentifier

            );
        }

        private static IReadOnlyList<TReport> GetExpiredReports()
        {
            var date = DateTimeOffset.UtcNow.AddHours(-InvitationExpiresInHours);
            return TReportSearch.Select(x => x.ReportType == InvitationReportType && x.ReportData == InvitationReportData && x.Created < date);
        }

        private static string GetRegistrationLink(Guid eventIdentifier)
            => $"ui/portal/events/classes/register?event={eventIdentifier}";
    }
}