using System;

namespace InSite.Web.Helpers
{
    static class CaseAttachmentHelper
    {
        public static bool AllowLearnerToViewByIssue(Guid issueId)
        {
            var issue = ServiceLocator.IssueSearch.GetIssue(issueId) ?? throw new ArgumentException($"Case is nto found: {issueId}");
            return AllowLearnerToViewByTopicUser(issue.TopicUserIdentifier);
        }

        public static bool AllowLearnerToViewByTopicUser(Guid? topicUserId)
        {
            var identity = CurrentSessionState.Identity;

            return topicUserId == identity.UserId
                ? identity.Organization.Toolkits.Issues?.DefaultCandidateUploadFileView ?? true
                : identity.Organization.Toolkits.Issues?.DefaultAdministratorUploadFileView ?? false;
        }
    }
}