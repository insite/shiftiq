using System;

namespace InSite.Persistence
{
    public enum EngagementStatusType
    {
        None, NoActivity, LastActivityOverOneWeekAgo, LastActivityOverOneMonthAgo
    }

    public enum EngagementPromptType
    {
        None, PromptNeeded, NoPromptNeeded
    }

    [Serializable]
    public class VLearnerActivity
    {
        public Guid? AchievementIdentifier { get; set; }
        public Guid? GradebookIdentifier { get; set; }
        public Guid LearnerIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
        public string CertificateStatus { get; set; }
        public string EnrollmentStatus { get; set; }
        public string GradebookName { get; set; }
        public string ImmigrationDestination { get; set; }
        public string ImmigrationNumber { get; set; }
        public string ImmigrationStatus { get; set; }
        public string LearnerCitizenship { get; set; }
        public string LearnerConsent { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerGender { get; set; }
        public string LearnerName { get; set; }
        public string LearnerNameFirst { get; set; }
        public string LearnerNameLast { get; set; }
        public string LearnerOccupation { get; set; }
        public string LearnerPhone { get; set; }
        public string LearnerRole { get; set; }
        public string PersonCode { get; set; }
        public string ProgramName { get; set; }
        public string ReferrerName { get; set; }
        public string ReferrerOther { get; set; }

        public int? SessionCount { get; set; }
        public int? SessionMinutes { get; set; }

        public DateTimeOffset? CertificateGranted { get; set; }
        public DateTimeOffset EnrollmentStarted { get; set; }
        public DateTime? ImmigrationArrival { get; set; }
        public DateTime? LearnerBirthdate { get; set; }
        public DateTimeOffset LearnerCreated { get; set; }
        public DateTimeOffset? SessionStartedFirst { get; set; }
        public DateTimeOffset? SessionStartedLast { get; set; }

        public string EngagementStatus
        {
            get
            {
                if (SessionCount == 0)
                    return "4. Never Authenticated";

                var oneWeekAgo = DateTimeOffset.Now.AddDays(-7);
                var oneMonthAgo = DateTimeOffset.Now.AddMonths(-1);

                if (SessionStartedLast.HasValue && SessionStartedLast.Value < oneWeekAgo)
                    return "2. Last Authenticated Over 1 Week Ago";

                if (SessionStartedLast.HasValue && SessionStartedLast.Value < oneMonthAgo)
                    return "3. Last Authenticated Over 1 Month Ago";

                return "1. Authenticated Within the Past Week";
            }
        }

        public string EngagementFlag
        {
            get
            {
                if (SessionCount == 0)
                    return "<i class='fas fa-flag text-danger'></i>";

                var oneWeekAgo = DateTimeOffset.Now.AddDays(-7);
                var oneMonthAgo = DateTimeOffset.Now.AddMonths(-1);

                if (SessionStartedLast.HasValue && SessionStartedLast.Value < oneWeekAgo)
                    return "<i class='fas fa-flag text-primary'></i>";

                if (SessionStartedLast.HasValue && SessionStartedLast.Value < oneMonthAgo)
                    return "<i class='fas fa-flag text-warning'></i>";

                return "<i class='fas fa-flag text-success'></i>";
            }
        }
    }
}
