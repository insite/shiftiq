using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Application.Cases.Read;
using InSite.Application.Contents.Read;

namespace InSite.Application.Issues.Read
{
    public interface ICaseSearch
    {
        int CountIssues(QIssueFilter filter);
        List<VIssue> GetIssues(IEnumerable<Guid> ids);
        List<ExportCase> GetExportCases(QIssueFilter filter);
        List<VIssue> GetIssues(QIssueFilter filter);
        List<VIssue> GetIssuesWithCommentMentions(Guid organization, Guid user);
        List<Guid> GetIssuesTopicUserIdentifiers(QIssueFilter filter);

        int CountUsers(QIssueUserFilter filter);

        List<VIssueUser> GetUsers(QIssueUserFilter filter, params Expression<Func<VIssueUser, object>>[] includes);

        List<Guid> GetUsersHavingRole(Guid organization, string role);

        VIssue GetIssue(Guid id);
        int GetNextIssueNumber(Guid identifier);

        int CountComments(QIssueCommentFilter filter);
        VComment GetComment(Guid comment);
        List<VComment> GetComments(QIssueCommentFilter filter);

        int CountAttachments(QIssueAttachmentFilter filter);
        List<VIssueAttachment> GetAttachments(QIssueAttachmentFilter filter);
        VIssueAttachment GetAttachment(Guid issue, string fileName);

        QIssueFileRequirement GetFileRequirement(Guid issue, string requestedFileCategory);
        List<VIssueFileRequirement> GetFileRequirements(Guid issue);

        TCaseStatus GetStatus(Guid status);
        List<TCaseStatus> GetStatuses(Guid organization);
        List<TCaseStatus> GetStatuses(Guid organization, string issueType);
        List<TCaseStatus> GetStatuses(Guid organization, string issueType, string statusCategory);
        List<string> GetStatusNamesByCategory(Guid organization, string statusCategory);
    }
}
