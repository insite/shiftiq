using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Application.Issues.Read
{
    [Serializable]
    public class QIssueFilter : Filter
    {
        public DateTimeOffset? DateClosedSince { get; set; }
        public DateTimeOffset? DateClosedBefore { get; set; }
        public DateTimeOffset? DateOpenedSince { get; set; }
        public DateTimeOffset? DateOpenedBefore { get; set; }
        public DateTimeOffset? DateReportedSince { get; set; }
        public DateTimeOffset? DateReportedBefore { get; set; }
        public DateTimeOffset? DateCaseStatusEffectiveSince { get; set; }
        public DateTimeOffset? DateCaseStatusEffectiveBefore { get; set; }

        public Guid OrganizationIdentifier { get; set; }
        public Guid? AdministratorUserIdentifier { get; set; }
        public Guid? OwnerUserIdentifier { get; set; }
        public Guid? LawyerIdentifier { get; set; }
        public Guid? ResponseSessionIdentifier { get; set; }
        public Guid? TopicUserIdentifier { get; set; }
        public Guid[] TopicUserIdentifiers { get; set; }
        public Guid? TopicUserConnectedFromUserIdentifier { get; set; }
        public int? IssueNumber { get; set; }
        public List<Guid?> AssigneeEmployer { get; set; }
        public string PersonCode { get; set; }
        public string AssigneeName { get; set; }
        public string AssigneeOrganization { get; set; }
        public string IssueDescription { get; set; }
        public Guid? IssueStatusIdentifier { get; set; }
        public string IssueStatusCategory { get; set; }
        public string IssueTitle { get; set; }
        public string IssueType { get; set; }

        // Comments

        public string IssueCommentsDescription { get; set; }
        public Guid? IssueCommentAssigneeIdentifier { get; set; }
        public string IssueCommentCategory { get; set; }
        public string IssueCommentSubCategory { get; set; }
        public string IssueCommentFlag { get; set; }
        public string IssueCommentTag { get; set; }
        public string[] MembershipStatuses { get; set; }

        // Attachments

        public string AttachmentFileStatus { get; set; }
        public string AttachmentFileCategory { get; set; }
        public string AttachmentDocumentName { get; set; }
        public bool? AttachmentHasClaims { get; set; }
        public DateTimeOffset? AttachmentFileExpirySince { get; set; }
        public DateTimeOffset? AttachmentFileExpiryBefore { get; set; }
        public DateTimeOffset? AttachmentFileReceivedSince { get; set; }
        public DateTimeOffset? AttachmentFileReceivedBefore { get; set; }
        public DateTimeOffset? AttachmentFileAlternatedSince { get; set; }
        public DateTimeOffset? AttachmentFileAlternatedBefore { get; set; }
        public DateTimeOffset? AttachmentApprovedSince { get; set; }
        public DateTimeOffset? AttachmentApprovedBefore { get; set; }
        public DateTimeOffset? AttachmentUploadedSince { get; set; }
        public DateTimeOffset? AttachmentUploadedBefore { get; set; }
        public bool OnlyRequestedFiles { get; set; }

        public QIssueFilter Clone()
        {
            return (QIssueFilter)MemberwiseClone();
        }
    }
}
