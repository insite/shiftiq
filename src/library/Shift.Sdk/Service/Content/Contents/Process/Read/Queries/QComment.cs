using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Contents.Read
{
    public class QComment
    {
        public Guid? AssessmentAttemptIdentifier { get; set; }
        public Guid? AssessmentBankIdentifier { get; set; }
        public Guid? AssessmentFieldIdentifier { get; set; }
        public Guid? AssessmentFormIdentifier { get; set; }
        public Guid? AssessmentQuestionIdentifier { get; set; }
        public Guid? AssessmentSpecificationIdentifier { get; set; }
        public Guid AuthorUserIdentifier { get; set; }
        public Guid CommentIdentifier { get; set; }
        public Guid ContainerIdentifier { get; set; }
        public Guid? EventIdentifier { get; set; }
        public Guid? IssueIdentifier { get; set; }
        public Guid? LogbookExperienceIdentifier { get; set; }
        public Guid? LogbookIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }
        public Guid? RevisorUserIdentifier { get; set; }
        public Guid? TimestampCreatedBy { get; set; }
        public Guid? TimestampModifiedBy { get; set; }
        public Guid? TopicUserIdentifier { get; set; }
        public Guid? TrainingProviderGroupIdentifier { get; set; }
        public Guid? UploadIdentifier { get; set; }
        public Guid? CommentAssignedToUserIdentifier { get; set; }
        public Guid? CommentResolvedByUserIdentifier { get; set; }

        public string AuthorUserName { get; set; }
        public string AuthorUserRole { get; set; }
        public string CommentCategory { get; set; }
        public string CommentSubCategory { get; set; }
        public string CommentFlag { get; set; }
        public string CommentTag { get; set; }
        public string CommentText { get; set; }
        public string ContainerDescription { get; set; }
        public string ContainerSubtype { get; set; }
        public string ContainerType { get; set; }
        public string EventFormat { get; set; }
        public string OriginText { get; set; }

        public bool CommentIsHidden { get; set; }
        public bool CommentIsPrivate { get; set; }

        public DateTimeOffset? CommentFlagged { get; set; }
        public DateTimeOffset CommentPosted { get; set; }
        public DateTimeOffset? CommentResolved { get; set; }
        public DateTimeOffset? CommentRevised { get; set; }
        public DateTimeOffset? CommentSubmitted { get; set; }
        public DateTimeOffset? EventStarted { get; set; }
        public DateTimeOffset TimestampCreated { get; set; }
        public DateTimeOffset? TimestampModified { get; set; }
    }

    public class VComment
    {
        public Guid? AssessmentAttemptIdentifier { get; set; }
        public Guid? AssessmentBankIdentifier { get; set; }
        public Guid? AssessmentFieldIdentifier { get; set; }
        public Guid? AssessmentFormIdentifier { get; set; }
        public Guid? AssessmentQuestionIdentifier { get; set; }
        public Guid? AssessmentSpecificationIdentifier { get; set; }
        public Guid AuthorUserIdentifier { get; set; }
        public Guid CommentIdentifier { get; set; }
        public Guid ContainerIdentifier { get; set; }
        public Guid? EventIdentifier { get; set; }
        public Guid? IssueIdentifier { get; set; }
        public Guid? LogbookExperienceIdentifier { get; set; }
        public Guid? LogbookIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }
        public Guid? RevisorUserIdentifier { get; set; }
        public Guid? TimestampCreatedBy { get; set; }
        public Guid? TimestampModifiedBy { get; set; }
        public Guid? TopicUserIdentifier { get; set; }
        public Guid? TrainingProviderGroupIdentifier { get; set; }
        public Guid? UploadIdentifier { get; set; }
        public Guid? CommentAssignedToUserIdentifier { get; set; }
        public Guid? CommentResolvedByUserIdentifier { get; set; }

        public string AuthorEmail { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorUserRole { get; set; }
        public string BankName { get; set; }
        public string BankTitle { get; set; }
        public string CommentCategory { get; set; }
        public string CommentSubCategory { get; set; }
        public string CommentFlag { get; set; }
        public string CommentTag { get; set; }
        public string CommentText { get; set; }
        public string ContainerDescription { get; set; }
        public string ContainerSubtype { get; set; }
        public string ContainerType { get; set; }
        public string EventFormat { get; set; }
        public string RevisorUserName { get; set; }
        public string TopicEmail { get; set; }
        public string TopicFirstName { get; set; }
        public string TopicLastName { get; set; }
        public string TopicUserName { get; set; }
        public string TrainingProviderGroupName { get; set; }
        public string UploadFingerprint { get; set; }
        public string UploadName { get; set; }
        public string UploadType { get; set; }
        public string UploadUrl { get; set; }

        public bool CommentIsHidden { get; set; }
        public bool CommentIsPrivate { get; set; }

        public int? UploadSize { get; set; }

        public DateTimeOffset? CommentFlagged { get; set; }
        public DateTimeOffset CommentPosted { get; set; }
        public DateTimeOffset? CommentResolved { get; set; }
        public DateTimeOffset? CommentRevised { get; set; }
        public DateTimeOffset? CommentSubmitted { get; set; }
        public DateTimeOffset? EventStarted { get; set; }
        public DateTimeOffset TimestampCreated { get; set; }
        public DateTimeOffset? TimestampModified { get; set; }
        public DateTimeOffset? UploadModified { get; set; }

        public string CommentCategoryHtml
        {
            get
            {
                if (CommentCategory.IsEmpty())
                    return string.Empty;

                var result = $"<span class='badge bg-info'>{CommentCategory}</span>";

                if (CommentSubCategory.IsNotEmpty())
                    result += $"<span class='badge bg-success ms-1'>{CommentSubCategory}</span>";

                return result;
            }
        }

        public string CommentFlagHtml
        {
            get
            {
                return CommentFlag.ToEnumNullable<FlagType>()?.ToIconHtml() ?? string.Empty;
            }
        }

        public string CommentPrivacyHtml
        {
            get
            {
                return CommentIsPrivate
                    ? $"<span class='badge bg-danger'>Private</span>"
                    : string.Empty;
            }
        }

        public string CommentTextHtml
        {
            get
            {
                return Markdown.ToHtml(CommentText);
            }
        }
    }
}