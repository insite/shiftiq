using System;
using System.Collections.Generic;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Records.Read
{
    public class QJournal
    {
        public Guid JournalIdentifier { get; set; }
        public Guid JournalSetupIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public DateTimeOffset JournalCreated { get; set; }

        public virtual QJournalSetup JournalSetup { get; set; }
        public virtual VUser User { get; set; }

        public virtual ICollection<QExperience> Experiences { get; set; } = new HashSet<QExperience>();
    }
}
