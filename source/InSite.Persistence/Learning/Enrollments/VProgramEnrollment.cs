using System;

namespace InSite.Persistence
{
    [Serializable]
    public class VProgramEnrollment
    {
        public Guid EnrollmentIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string ProgramCode { get; set; }
        public string ProgramDescription { get; set; }
        public string ProgramName { get; set; }
        public string UserEmail { get; set; }
        public string UserEmailAlternate { get; set; }
        public string UserFullName { get; set; }
        public string UserPhone { get; set; }

        public DateTimeOffset ProgressAssigned { get; set; }
        public DateTimeOffset? ProgressStarted { get; set; }
        public DateTimeOffset? ProgressCompleted { get; set; }

        public int? TimeTaken { get; set; }

        public string CreatedWho { get; set; }
        public DateTimeOffset? CreatedWhen { get; set; }
    }
}
