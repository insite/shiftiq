using System;

namespace InSite.Application.Records.Read
{
    public class VJournalSetupUser
    {
        public Guid JournalSetupIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? JournalIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public string UserRole { get; set; }
        public string JournalSetupName { get; set; }
        public DateTimeOffset? JournalSetupCreated { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserEmailAlternate { get; set; }
        public string EmployerGroupName { get; set; }
        public string PersonCode { get; set; }
    }
}
