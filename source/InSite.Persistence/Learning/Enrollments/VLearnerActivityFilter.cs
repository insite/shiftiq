using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VLearnerActivityFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }

        public Guid? MembershipStatusItemIdentifier
        {
            get => MembershipStatusItemIdentifiers != null && MembershipStatusItemIdentifiers.Length == 1 ? MembershipStatusItemIdentifiers[0] : (Guid?)null;
            set => MembershipStatusItemIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] MembershipStatusItemIdentifiers { get; set; }

        public Guid? EmployerGroupIdentifier
        {
            get => EmployerGroupIdentifiers != null && EmployerGroupIdentifiers.Length == 1 ? EmployerGroupIdentifiers[0] : (Guid?)null;
            set => EmployerGroupIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] EmployerGroupIdentifiers { get; set; }

        public Guid? ProgramIdentifier
        {
            get => ProgramIdentifiers != null && ProgramIdentifiers.Length == 1 ? ProgramIdentifiers[0] : (Guid?)null;
            set => ProgramIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] ProgramIdentifiers { get; set; }

        public string LearnerNameLast { get; set; }
        public string LearnerNameFirst { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerRole { get; set; }
        public string PersonCode { get; set; }

        public string[] LearnerGenders { get; set; }
        public string[] LearnerCitizenships { get; set; }
        public string[] ImmigrationStatuses { get; set; }
        public string[] ReferrerNames { get; set; }
        public string[] ProgramNames { get; set; }
        public string[] GradebookNames { get; set; }

        public string CertificateStatus { get; set; }
        public string EnrollmentStatus { get; set; }
        public string CountStrategy { get; set; }

        public DateTimeOffset? AchievementGrantedSince { get; set; }
        public DateTimeOffset? AchievementGrantedBefore { get; set; }
        public DateTimeOffset? EnrollmentStartedSince { get; set; }
        public DateTimeOffset? EnrollmentStartedBefore { get; set; }

        public bool IsSummaryCountStrategy => CountStrategy == "Summary";

        public EngagementStatusType EngagementStatus { get; set; }
        public EngagementPromptType EngagementPrompt { get; set; }

        public VLearnerActivityFilter()
        {

        }

        public VLearnerActivityFilter Clone()
        {
            return (VLearnerActivityFilter)MemberwiseClone();
        }
    }
}
