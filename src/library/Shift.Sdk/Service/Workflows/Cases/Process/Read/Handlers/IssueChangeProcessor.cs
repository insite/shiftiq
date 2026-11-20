using System;

using InSite.Application.Messages.Write;
using InSite.Application.Organizations.Read;
using InSite.Domain.Issues;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Common.Timeline.Changes;

namespace InSite.Application.Issues.Read
{
    public class IssueChangeProcessor
    {
        private readonly IAlertMailer _mailer;
        private readonly ICaseSearch _issues;
        private readonly IOrganizationSearch _organizations;
        private readonly Urls _urls;

        public IssueChangeProcessor(
            IChangeQueue publisher, IAlertMailer mailer, ICaseSearch issues,
            IOrganizationSearch organizations, Urls urls)
        {
            _mailer = mailer;
            _issues = issues;
            _organizations = organizations;
            _urls = urls;

            publisher.Subscribe<UserAssigned>(Handle);
        }

        private void Handle(UserAssigned change)
        {
            if (StringHelper.Equals(change.Role, "Owner"))
                _mailer.Send(change.OriginOrganization, change.OriginUser, CreateOwnerNotification(change));

            else if (StringHelper.Equals(change.Role, "Administrator"))
                _mailer.Send(CreateAdminNotification(change), change.User);
        }

        private Notification_IssueOwnerChanged CreateOwnerNotification(UserAssigned change)
        {
            var issue = _issues.GetIssue(change.AggregateIdentifier);

            return new Notification_IssueOwnerChanged
            {
                OriginOrganization = change.OriginOrganization,
                OriginUser = change.OriginUser,

                IssueNumber = issue.IssueNumber.ToString(),
                IssueType = issue.IssueType,
                IssueStatus = issue.IssueStatusName,
                IssueSummary = issue.IssueTitle,

                OwnerEmail = issue.OwnerUserEmail,
                OwnerFirstName = issue.OwnerUserFirstName,
                OwnerLastName = issue.OwnerUserLastName,
            };
        }

        private Notification_CaseManagerAssigned CreateAdminNotification(UserAssigned change)
        {
            var issue = _issues.GetIssue(change.AggregateIdentifier);

            return new Notification_CaseManagerAssigned
            {
                OriginOrganization = change.OriginOrganization,
                OriginUser = change.OriginUser,

                CaseID = issue.IssueNumber.ToString(),
                CaseType = issue.IssueType,
                CaseStatus = issue.IssueStatusName,
                Member = issue.TopicUserName,
                CaseURL = GetCaseUrl(change.OriginOrganization, issue.IssueIdentifier),
            };
        }

        private string GetCaseUrl(Guid organizationIdentifier, Guid issueIdentifier)
        {
            var organization = _organizations.Get(organizationIdentifier);
            if (organization == null)
                return null;

            var appUrl = _urls.GetApplicationUrl(organization.OrganizationCode);

            return $"{appUrl}/ui/admin/workflow/cases/outline?case={issueIdentifier}";
        }
    }
}
