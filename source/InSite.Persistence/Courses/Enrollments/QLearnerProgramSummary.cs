using System;

namespace InSite.Persistence
{
    public class QLearnerProgramSummary
    {
        public Guid SummaryIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string ImmigrationArrivalStatus { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerGender { get; set; }
        public string LearnerName { get; set; }
        public string LearnerStreams { get; set; }
        public string ProgramName { get; set; }
        public string ProgramStatus { get; set; }
        public string ReferrerIndustry { get; set; }
        public string ReferrerName { get; set; }
        public string ReferrerNameOther { get; set; }
        public string ReferrerProvince { get; set; }
        public string ReferrerRole { get; set; }

        public int? ProgramGradeItems { get; set; }
        public int? ProgramGradeItemsCompleted { get; set; }

        public DateTimeOffset AsAt { get; set; }
        public DateTime? ImmigrationArrivalDate { get; set; }
        public DateTimeOffset? LearnerAccessGranted { get; set; }
        public DateTimeOffset? LearnerAccountCreated { get; set; }
        public DateTimeOffset? LearnerAddedToProgram { get; set; }

        public virtual User User { get; set; }
    }
}
