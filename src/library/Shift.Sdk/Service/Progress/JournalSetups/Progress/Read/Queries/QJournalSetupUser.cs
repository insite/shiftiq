using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Records.Read
{
    public class QJournalSetupUser
    {
        public Guid EnrollmentIdentifier { get; set; }
        public Guid JournalSetupIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public string UserRole { get; set; }

        public virtual QJournalSetup JournalSetup { get; set; }
        public virtual VUser User { get; set; }
    }
}
