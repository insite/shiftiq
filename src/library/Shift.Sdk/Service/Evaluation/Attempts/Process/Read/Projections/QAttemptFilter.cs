using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Attempts.Read
{
    [Serializable]
    public class QAttemptFilter : Filter
    {
        public Guid? AttemptIdentifier { get; set; }
        public Guid[] AttemptIdentifiers { get; set; }
        public Guid? BankIdentifier { get; set; }
        public Guid? LearnerUserIdentifier
        {
            get => LearnerUserIdentifiers != null && LearnerUserIdentifiers.Length == 1 ? LearnerUserIdentifiers[0] : (Guid?)null;
            set => LearnerUserIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }

        public Guid? EventIdentifier { get; set; }
        public Guid? FormIdentifier
        {
            get => FormIdentifiers != null && FormIdentifiers.Length == 1 ? FormIdentifiers[0] : (Guid?)null;
            set => FormIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid? FormOrganizationIdentifier { get; set; }
        public Guid? LearnerOrganizationIdentifier { get; set; }
        public Guid? FrameworkIdentifier { get; set; }
        public Guid? OccupationIdentifier { get; set; }
        public Guid? QuestionIdentifier { get; set; }
        public Guid? GradingAssessorIdentifier { get; set; }

        public Guid[] CandidateOrganizationIdentifiers { get; set; }
        public Guid[] FormIdentifiers { get; set; }
        public Guid[] LearnerUserIdentifiers { get; set; }

        public string AssessorName { get; set; }
        public string AttemptGrade { get; set; }
        public string AttemptStatus { get; set; }
        public string AttemptTagStatus { get; set; }
        public string LearnerKeyword { get; set; }
        public string LearnerCompany { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerName { get; set; }
        public string Event { get; set; }
        public string EventFormat { get; set; }
        public string BankLevel { get; set; }
        public string Form { get; set; }
        public string FormName { get; set; }

        public string[] AttemptTag { get; set; }
        public string[] CandidateType { get; set; }

        public bool ExcludeArchivedUsers { get; set; }
        
        public bool? IsCompleted { get; set; }
        public bool? IsSubmitted { get; set; }
        public bool? IsStarted { get; set; }
        public bool? IsImported { get; set; }

        public int? AttemptScoreFrom { get; set; }
        public int? AttemptScoreThru { get; set; }

        public DateTimeOffset? AttemptGradedSince { get; set; }
        public DateTimeOffset? AttemptGradedBefore { get; set; }
        public DateTimeOffset? AttemptStartedSince { get; set; }
        public DateTimeOffset? AttemptStartedBefore { get; set; }

        public InclusionType PilotAttemptsInclusion { get; set; }
        public Guid? RubricIdentifier { get; set; }
        public string[] OrganizationPersonTypes { get; set; }

        public bool HideLearnerName { get; set; }
        public bool? IsWithoutGradingAssessor { get; set; }

        public QAttemptFilter()
        {
            PilotAttemptsInclusion = InclusionType.Include;
        }

        public QAttemptFilter Clone()
        {
            return (QAttemptFilter)MemberwiseClone();
        }
    }

    [Serializable]
    public class QAttemptCommentaryFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public string FormTitle { get; set; }
        public int? AssetNumber { get; set; }
        public string CommentText { get; set; }
    }

    public class QAttemptCommentaryItem
    {
        public string FormTitle { get; set; }
        public int FormAssetNumber { get; set; }
        public int QuestionSequence { get; set; }
        public string QuestionText { get; set; }
        public DateTimeOffset CommentPosted { get; set; }
        public string AuthorName { get; set; }
        public string CommentText { get; set; }
    }
}