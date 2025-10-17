using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Records.Read
{
    public class QJournalSetupGroup
    {
        public Guid JournalSetupIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public DateTimeOffset Created { get; set; }

        public virtual QJournalSetup JournalSetup { get; set; }
        public virtual QGroup Group { get; set; }
    }
}
