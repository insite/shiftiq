using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Application.Records.Write;
using InSite.Domain.Messages;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Records.Programs.Utilities
{
    // The request / approve / reject engine for paid program enrollment. The learner pre-enrollment
    // page calls Submit; the approver queue page calls Approve and Reject; the admin bypass
    // ("Enroll Learners") calls EnrollLearners. This helper owns the request lifecycle, the
    // achievement-granting enrollment on approval, and the learner notifications. The request row
    // is the durable billing/audit record - it outlives learner archival because the learner email
    // is snapshotted here.
    public static class ProgramEnrollmentRequestHelper
    {
        // Self-funded requests, and company-sponsored requests with no Company Administrator, route here.
        private static string PartitionAdminEmail = ServiceLocator.AppSettings.Partition.Email;

        // Invoicing groups (data rows, not code), which is how a program enrollment request routes to
        // one company's admins. The group itself is owned by the partition organization, not by the
        // company: TEC-854 moved these five teams off Keyera onto the CMDS account so there would be
        // one set for the whole partition. What makes a team "this company's" is the membership row,
        // which carries its own organization - so the group is resolved here and the company scoping
        // happens in GetGroupMembers. Look for the group under the company instead and it is never
        // found, which sends every company-sponsored request to the partition admin. The Invoicing
        // panel on the organization edit page (UI/CMDS/Common/Controls/User/InvoicingContacts.ascx.cs)
        // splits the same two predicates, and is what an administrator sees these teams through.
        private static Guid CompanyAdministratorsGroup()
        {
            return InvoicingGroup("Company Administrators");
        }
        private static Guid AccountManagersGroup()
        {
            return InvoicingGroup("Account Managers");
        }
        private static Guid InvoicingGroup(string groupName)
        {
            var partitionId = ServiceLocator.AppSettings.Partition.Identifier;

            return ServiceLocator.GroupSearch
                .BindGroups(x => x.GroupIdentifier, x => x.OrganizationIdentifier == partitionId && x.GroupType == "Team" && x.GroupCategory == "Invoicing" && x.GroupName == groupName)
                .FirstOrDefault();
        }

        // Confirm action. Creates the request row (status requested) and returns its identifier.
        // The filtered unique index allows only one open request per learner+program, so if one
        // already exists this returns it rather than colliding.
        public static Guid Submit(Guid programId, Guid learnerUserId, Guid organizationId, string fundingPath)
        {
            var open = TProgramEnrollmentRequestStore.GetOpen(programId, learnerUserId);
            if (open != null)
                return open.ProgramEnrollmentRequestIdentifier;

            var program = ProgramSearch.GetProgram(programId);

            var request = new TProgramEnrollmentRequest
            {
                ProgramEnrollmentRequestIdentifier = UniqueIdentifier.Create(),
                OrganizationIdentifier = organizationId,
                ProgramIdentifier = programId,
                LearnerUserIdentifier = learnerUserId,
                LearnerUserEmail = GetLearnerEmail(learnerUserId, organizationId),
                RequestFundingPath = fundingPath,
                RequestStatus = ProgramEnrollmentRequestStatus.Requested,
                RequestSubmittedAt = DateTimeOffset.UtcNow,
                RequestFee = program?.ProgramFee,
                RequestFunderEmail = ResolveFunderEmail(fundingPath, organizationId)
            };

            TProgramEnrollmentRequestStore.Insert(request);

            NotifyFunder(request, program);

            return request.ProgramEnrollmentRequestIdentifier;
        }

        // Approve. Enrolls the learner (which assigns the program's achievements and grants course
        // access), links the resulting enrollment, stamps the decision, and notifies the learner.
        public static void Approve(Guid requestId, Guid decidedBy)
        {
            var request = TProgramEnrollmentRequestStore.Get(requestId);
            if (request == null || request.RequestStatus != ProgramEnrollmentRequestStatus.Requested)
                return;

            ProgramHelper.EnrollLearner(request.OrganizationIdentifier, request.ProgramIdentifier, request.LearnerUserIdentifier);

            request.RequestStatus = ProgramEnrollmentRequestStatus.Approved;
            request.RequestDecidedAt = DateTimeOffset.UtcNow;
            request.RequestDecidedBy = decidedBy;
            request.ProgramEnrollmentIdentifier = ProgramStore.GetEnrollmentIdentifier(request.ProgramIdentifier, request.LearnerUserIdentifier);

            TProgramEnrollmentRequestStore.Update(request);

            NotifyLearner(request, approved: true);
        }

        // Reject. Stamps the decision and notifies the learner; assigns nothing.
        public static void Reject(Guid requestId, Guid decidedBy)
        {
            var request = TProgramEnrollmentRequestStore.Get(requestId);
            if (request == null || request.RequestStatus != ProgramEnrollmentRequestStatus.Requested)
                return;

            request.RequestStatus = ProgramEnrollmentRequestStatus.Rejected;
            request.RequestDecidedAt = DateTimeOffset.UtcNow;
            request.RequestDecidedBy = decidedBy;

            TProgramEnrollmentRequestStore.Update(request);

            NotifyLearner(request, approved: false);
        }

        // Admin bypass ("Enroll Learners"). Creates an approved request directly, enrolls the
        // learner, and supersedes any open request for the same learner+program. Silent - no
        // learner notification, and a superseded row closes the open request without denying it.
        public static void EnrollLearners(Guid programId, Guid learnerUserId, Guid organizationId, string fundingPath, Guid decidedBy)
        {
            var now = DateTimeOffset.UtcNow;

            var open = TProgramEnrollmentRequestStore.GetOpen(programId, learnerUserId);
            if (open != null)
            {
                open.RequestStatus = ProgramEnrollmentRequestStatus.Superseded;
                open.RequestDecidedAt = now;
                open.RequestDecidedBy = decidedBy;
                TProgramEnrollmentRequestStore.Update(open);
            }

            ProgramHelper.EnrollLearner(organizationId, programId, learnerUserId);

            var program = ProgramSearch.GetProgram(programId);

            var request = new TProgramEnrollmentRequest
            {
                ProgramEnrollmentRequestIdentifier = UniqueIdentifier.Create(),
                OrganizationIdentifier = organizationId,
                ProgramIdentifier = programId,
                LearnerUserIdentifier = learnerUserId,
                LearnerUserEmail = GetLearnerEmail(learnerUserId, organizationId),
                RequestFundingPath = fundingPath,
                RequestStatus = ProgramEnrollmentRequestStatus.Approved,
                RequestSubmittedAt = now,
                RequestDecidedAt = now,
                RequestDecidedBy = decidedBy,
                RequestFee = program?.ProgramFee,
                RequestFunderEmail = ResolveFunderEmail(fundingPath, organizationId),
                ProgramEnrollmentIdentifier = ProgramStore.GetEnrollmentIdentifier(programId, learnerUserId)
            };

            TProgramEnrollmentRequestStore.Insert(request);
        }

        // The decision is committed before this runs, so a notification failure - an unreachable
        // mail service, a template that will not render - must not reach the approver as a failed
        // decision. Log it and carry on; the request row is the record that matters.
        private static void NotifyLearner(TProgramEnrollmentRequest request, bool approved)
        {
            try
            {
                SendLearnerNotification(request, approved);
            }
            catch (Exception ex)
            {
                ServiceLocator.Logger.Error(ex,
                    "Could not notify the learner of the decision on enrollment request {request}.",
                    request.ProgramEnrollmentRequestIdentifier);
            }
        }

        // Notifies the learner of the decision. The template is shared across organizations and resolved by
        // the notification's type name, so there is no per-program template to look up here.
        private static void SendLearnerNotification(
            TProgramEnrollmentRequest request, bool approved)
        {
            var program = ProgramSearch.GetProgram(request.ProgramIdentifier);
            if (program == null)
                return;

            var user = ServiceLocator.ContactSearch.GetUser(request.LearnerUserIdentifier);
            if (user == null)
                return;

            var notifier = BuildNotifier();

            Notification notification = approved
                ? (Notification)notifier.ApprovedNotification(request.OrganizationIdentifier, user.UserFirstName, user.UserLastName, program.ProgramName)
                : notifier.RejectedNotification(request.OrganizationIdentifier, user.UserFirstName, user.UserLastName, program.ProgramName);

            notifier.SendNotification(notification, request.LearnerUserIdentifier);
        }

        private static string GetLearnerEmail(Guid learnerUserId, Guid organizationId)
        {
            var user = ServiceLocator.ContactSearch.GetUser(learnerUserId);
            if (user == null)
                return null;

            var person = ServiceLocator.ContactSearch.GetPerson(learnerUserId, organizationId);
            if (person == null)
                return user.UserEmail;

            return EmailAddress.GetEnabledEmail(user.UserEmail, person.UserEmailEnabled, user.UserEmailAlternate, person.UserEmailAlternateEnabled);
        }

        // The request row is written before this runs, so a notification failure must not reach
        // the learner as a failed submit - their request exists either way, and the funder can
        // still find it on the search page. Log it and carry on.
        private static void NotifyFunder(TProgramEnrollmentRequest request, TProgram program)
        {
            try
            {
                SendFunderNotification(request, program);
            }
            catch (Exception ex)
            {
                ServiceLocator.Logger.Error(ex,
                    "Could not notify the funder of enrollment request {request}.",
                    request.ProgramEnrollmentRequestIdentifier);
            }
        }

        // Triggers the Requested/Funder notification. The template is shared across organizations and
        // resolved by the notification's type name. A company-sponsored request goes To the organization's
        // Company Administrators and Cc its Account Managers; a self-funded request, or one whose
        // organization has no Company Administrator, goes to the default partition admin instead.
        private static void SendFunderNotification(
            TProgramEnrollmentRequest request, TProgram program)
        {
            if (program == null)
                return;

            var learner = ServiceLocator.ContactSearch.GetUser(request.LearnerUserIdentifier);
            if (learner == null)
                return;

            var organizationId = request.OrganizationIdentifier;
            var organizationCode = GetOrganizationCode(organizationId);
            var feeText = request.RequestFee.HasValue ? request.RequestFee.Value.ToString("C") : null;
            var link = BuildRequestLink(organizationCode, request.ProgramEnrollmentRequestIdentifier);

            var notifier = BuildNotifier();
            var notification = notifier.RequestedNotification(
                organizationId,
                learner.UserFirstName,
                learner.UserLastName,
                program.ProgramName,
                feeText,
                request.RequestFundingPath,
                link);

            if (request.RequestFundingPath == ProgramEnrollmentRequestFundingPath.CompanySponsored)
            {
                var administrators = GetGroupMembers(CompanyAdministratorsGroup(), organizationId);
                if (administrators.Count > 0)
                {
                    var managers = GetGroupMembers(AccountManagersGroup(), organizationId);

                    var to = administrators.Select(x => x.UserIdentifier).ToArray();
                    var cc = managers.Select(x => x.UserIdentifier).ToArray();

                    var mailouts = ServiceLocator.AlertMailer.Send(notification, to, cc);

                    ReportIfNothingComposed(mailouts, notification, administrators.Select(x => x.Email));
                    return;
                }
            }

            NotifyPartitionAdmin(notification);
        }

        // Partition admin is addressed by resolving the shared mailbox to its user identifier. When
        // no user holds that address the notice is skipped rather than failing the submit - but the
        // skip is reported, because it is a configuration fault and not a fact about this request.
        //
        // The notice is re-originated onto the partition before it goes out. Recipients are resolved as
        // people *within* the notification's origin organization (RecipientList.GetRecipientsById in
        // EmailOutbox), and a partition administrator's person row belongs to the partition organization,
        // not to the learner's company. Leave the company as the origin and the lookup finds nothing, the
        // To list empties, and Compose returns no draft at all - so the notice was never composed, let
        // alone sent. Originating on the partition passes a null organization filter instead, which finds
        // the administrator wherever their person row lives.
        private static void NotifyPartitionAdmin(Notification notification)
        {
            var userId = UserSearch.BindFirst(x => (Guid?)x.UserIdentifier, new UserFilter { EmailExact = PartitionAdminEmail });

            if (!userId.HasValue)
            {
                ReportUnresolvedPartitionAdmin(notification);
                return;
            }

            notification.OriginOrganization = ServiceLocator.AppSettings.Partition.Identifier;

            var mailouts = ServiceLocator.AlertMailer.Send(notification, userId);

            ReportIfNothingComposed(mailouts, notification, new[] { PartitionAdminEmail });
        }

        // AlertMailer returns one identifier per mailout it created, so an empty result means the
        // notification composed to nothing - every recipient was dropped for want of a person record in
        // the notification's organization, or the template did not resolve and MessageNotFoundException
        // was swallowed. Neither leaves a trace anywhere else: no mailout row is written, Mailgun is never
        // called, and the submit succeeds. This is the only place that silence can be turned into a signal.
        private static void ReportIfNothingComposed(Guid[] mailouts, Notification notification, IEnumerable<string> recipients)
        {
            if (mailouts != null && mailouts.Length > 0)
                return;

            var addresses = string.Join(", ", recipients.EmptyIfNull().Where(x => !string.IsNullOrEmpty(x)));

            ServiceLocator.Logger.Warning(
                "The {notification} notification composed no email for {recipients}, so nothing was sent.",
                notification.Type, addresses);

            AppSentry.SentryError(
                $"The {notification.Type} notification composed no email for '{addresses}', so nothing was sent and no mailout was recorded. " +
                "Either those recipients hold no person record in the notification's organization, or its message template did not resolve.",
                nameof(ProgramEnrollmentRequestHelper));
        }

        // Partitions[].Email names an address that no user account holds, so every self-funded request -
        // and every company-sponsored one whose organization has no Company Administrator - notifies
        // nobody. Nothing else on the path complains: the request row is written, the submit succeeds,
        // and Mailgun shows no message at all, which is a long way to travel to find a wrong address in
        // AppSettings.json. So it goes to Sentry as an error, not just to the log.
        private static void ReportUnresolvedPartitionAdmin(Notification notification)
        {
            ServiceLocator.Logger.Warning(
                "The partition administrator address {email} belongs to no user account, so the {notification} notification was not sent.",
                PartitionAdminEmail, notification.Type);

            AppSentry.SentryError(
                $"The partition administrator address '{PartitionAdminEmail}' belongs to no user account, so the {notification.Type} notification was not sent. " +
                "Set the partition's Email in AppSettings.json to an address a user account holds, or create a user account for that address.",
                nameof(ProgramEnrollmentRequestHelper));
        }

        // The funder contact snapshotted onto the request. Self-funded routes to partition admin;
        // company-sponsored routes to the first Company Administrator (alphabetically) whose
        // membership names the learner's active organization, falling back to partition admin when
        // that organization has none.
        private static string ResolveFunderEmail(string fundingPath, Guid organizationId)
        {
            if (fundingPath == ProgramEnrollmentRequestFundingPath.CompanySponsored)
            {
                var administrators = GetGroupMembers(CompanyAdministratorsGroup(), organizationId);

                var email = administrators.Select(x => x.Email).FirstOrDefault();
                if (!string.IsNullOrEmpty(email))
                    return email;
            }

            return PartitionAdminEmail;
        }

        private static ProgramEnrollment BuildNotifier()
        {
            return new ProgramEnrollment(
                ServiceLocator.AlertMailer,
                ServiceLocator.MessageSearch,
                ServiceLocator.ContentSearch,
                ServiceLocator.ContactSearch,
                ServiceLocator.ProgramSearch,
                ServiceLocator.ProgramStore,
                ServiceLocator.AchievementSearch);
        }

        private static string GetOrganizationCode(Guid organizationId)
        {
            return ServiceLocator.GroupSearch.GetOrganizationCode(organizationId);
        }

        // The members of one Invoicing team who speak for a single company, ordered by email so callers
        // can take the alphabetically first one. The team is partition-wide, so the membership's own
        // organization is what narrows it to this company - drop that predicate and every company's
        // contacts in the partition come back, and the funder notice fans out to all of them.
        private static List<FunderRecipient> GetGroupMembers(Guid groupId, Guid organizationId)
        {
            if (groupId == Guid.Empty)
                return new List<FunderRecipient>();

            var members = MembershipSearch.Bind(
                p => new FunderRecipient { UserIdentifier = p.UserIdentifier, Email = p.User.Email },
                q => q.GroupIdentifier == groupId && q.OrganizationIdentifier == organizationId);

            return members
                .Where(x => !string.IsNullOrEmpty(x.Email))
                .OrderBy(x => x.Email)
                .ToList();
        }

        // The approver's deep link to the request's outline page, keyed by the request identifier - the
        // $EnrollmentRequested template variable on the Requested notification.
        private static string BuildRequestLink(string organizationCode, Guid requestId)
        {
            var baseUrl = ServiceLocator.Urls.GetApplicationUrl(organizationCode);

            return baseUrl.TrimEnd('/') + "/ui/admin/learning/programs/enrollments/requests/outline?request=" + requestId;
        }

        private class FunderRecipient
        {
            public Guid UserIdentifier { get; set; }
            public string Email { get; set; }
        }
    }
}
