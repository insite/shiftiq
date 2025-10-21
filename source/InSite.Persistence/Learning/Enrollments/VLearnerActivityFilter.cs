using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VLearnerActivityFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }

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
