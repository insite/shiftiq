using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseAggregate : AggregateRoot
    {
        #region Properties (state)

        public override AggregateState CreateState() => new CaseState();
        public CaseState Data => (CaseState)State;

        #endregion

        public void AddAttachment(string fileName, string fileType, Guid fileIdentifier, DateTimeOffset posted, Guid poster)
        {
            Apply(new CaseAttachmentAdded(fileName, fileType, fileIdentifier, posted, poster));
        }

        public void ChangeAttachmentFile(string fileName, Guid fileIdentifier)
        {
            Apply(new CaseAttachmentFileChanged(fileName, fileIdentifier));
        }

        public void RenameAttachmentFile(string oldFileName, string newFileName)
        {
            Apply(new CaseAttachmentFileRenamed(oldFileName, newFileName));
        }

        public void ModifyFileRequirement(string requestedFileCategory, string requestedFileSubcategory, string requestedFrom, string requestedFileDescription, string requestedFileStatus)
        {
            Apply(new CaseFileRequirementModified(requestedFileCategory, requestedFileSubcategory, requestedFrom, requestedFileDescription, requestedFileStatus));
        }

        public void CompleteRequest(string requestedFileCategory, string fileName, string fileType, Guid fileIdentifier, DateTimeOffset posted, Guid poster)
        {
            Apply(new CaseAttachmentAdded(fileName, fileType, fileIdentifier, posted, poster));
            Apply(new CaseFileRequirementCompleted(requestedFileCategory, fileName, fileIdentifier));
        }

        public void DeleteRequest(string requestedFileCategory)
        {
            Apply(new CaseFileRequirementDeleted(requestedFileCategory));
        }

        public void AssignGroup(Guid group, string role)
        {
            Apply(new GroupAssigned(group, role));
        }

        public void AssignUser(Guid user, string role)
        {
            Apply(new UserAssigned(user, role));
        }

        public void ChangeIssueStatus(Guid status, DateTimeOffset effective)
        {
            Apply(new CaseStatusChanged(status, effective));
        }

        public void ChangeIssueTitle(string issueTitle)
        {
            Apply(new CaseTitleChanged(issueTitle));
        }

        public void ChangeIssueType(string issueType)
        {
            Apply(new CaseTypeChanged(issueType));
        }

        public void CloseIssue()
        {
            Apply(new CaseClosed());
        }

        public void ConnectIssueToSurveyResponse(Guid response)
        {
            Apply(new CaseConnectedToSurveyResponse(response));
        }

        public void DescribeIssue(string description)
        {
            Apply(new CaseDescribed(description));
        }

        public void OpenIssue(Guid organization, int number, string title, string description, DateTimeOffset opened, string source, string type, DateTimeOffset? reported)
        {
            Apply(new CaseOpened2(organization, number, title, description, opened, source, type, reported));
        }

        public void RemoveAttachment(string fileName)
        {
            Apply(new CaseAttachmentDeleted(fileName));
        }

        public void RemoveComment(Guid comment)
        {
            Apply(new CaseCommentDeleted(comment));
        }

        public void ReopenIssue()
        {
            Apply(new CaseReopened());
        }

        public void AuthorComment(Guid comment, string text, string category, string flag,
            Guid author, string authorRole, Guid? assignedTo, Guid? resolvedBy, string subCategory, string tag,
            DateTimeOffset posted, DateTimeOffset? flagged, DateTimeOffset? submitted, DateTimeOffset? resolved)
        {
            Apply(new CaseCommentPosted(comment, text, category, flag, author, authorRole, assignedTo, resolvedBy, subCategory, tag, posted, flagged, submitted, resolved));
        }

        public void ChangeCommentPrivacy(Guid comment, bool commentPrivacy)
        {
            Apply(new CommentPrivacyChanged(comment, commentPrivacy));
        }

        public void ReviseComment(Guid comment, string text, string category, string flag,
            Guid revisor, Guid? assignedTo, Guid? resolvedBy, string subCategory, string tag,
            DateTimeOffset revised, DateTimeOffset? flagged, DateTimeOffset? submitted, DateTimeOffset? resolved)
        {
            Apply(new CaseCommentModified(comment, text, category, flag, revisor, assignedTo, resolvedBy, subCategory, tag, revised, flagged, submitted, resolved));
        }

        public void UnassignUser(Guid user, string role)
        {
            Apply(new UserUnassigned(user, role));
        }

        public void DeleteIssue()
        {
            Apply(new CaseDeleted());
        }
    }
}
