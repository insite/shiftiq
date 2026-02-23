using Shift.Common.Timeline.Changes;

using InSite.Application.Messages.Write;
using InSite.Domain.Issues;
using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Application.Issues.Read
{
    public class IssueChangeProcessor
    {
        private readonly IAlertMailer _mailer;
        private readonly ICaseSearch _issues;

        public IssueChangeProcessor(IChangeQueue publisher, IAlertMailer mailer, ICaseSearch issues)
        {
            _mailer = mailer;
            _issues = issues;

            publisher.Subscribe<UserAssigned>(Handle);
        }

        private void Handle(UserAssigned change)
        {
            if (!StringHelper.Equals(change.Role, "Owner"))
                return;

            _mailer.Send(change.OriginOrganization, change.OriginUser, CreateNotification(change));
        }

        private Notification_IssueOwnerChanged CreateNotification(UserAssigned change)
        {
            var issue = _issues.GetIssue(change.AggregateIdentifier);

            var notification = new Notification_IssueOwnerChanged
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

            return notification;
        }
    }
}
