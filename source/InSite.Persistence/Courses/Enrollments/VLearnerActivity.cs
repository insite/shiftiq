using System;

namespace InSite.Persistence
{
    [Serializable]
    public class VLearnerActivity
    {
        public int FakeKey { get; set; }
        public int RowNumber { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public string UserFullName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public string UserGender { get; set; }
        public string UserPhone { get; set; }
        public DateTime? UserBirthdate { get; set; }
        public string PersonCode { get; set; }
        public string UserCitizenship { get; set; }
        public Guid? MembershipStatusItemIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsLearner { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public string ProgramName { get; set; }
        public DateTimeOffset? EnrollmentCreated { get; set; }
        public Guid? GradebookIdentifier { get; set; }
        public string GradebookTitle { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public DateTimeOffset? CredentialGranted { get; set; }
        public string TaskType { get; set; }
    }
}
