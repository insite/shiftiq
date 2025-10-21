using System;

namespace InSite.Persistence
{
    [Serializable]
    public class VLearnerActivitySummary
    {
        public Guid LearnerIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

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

        public DateTimeOffset? EnrollmentStarted { get; set; }
        public DateTime? ImmigrationArrival { get; set; }
        public DateTime? LearnerBirthdate { get; set; }
        public DateTimeOffset LearnerCreated { get; set; }
        public DateTimeOffset? SessionStartedFirst { get; set; }
        public DateTimeOffset? SessionStartedLast { get; set; }

        public DateTimeOffset? CertificateGranted { get; set; }
    }
}
