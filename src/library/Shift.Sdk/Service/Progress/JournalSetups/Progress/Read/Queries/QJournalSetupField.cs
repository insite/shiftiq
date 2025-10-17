using System;

namespace InSite.Application.Records.Read
{
    public class QJournalSetupField
    {
        public Guid JournalSetupFieldIdentifier { get; set; }
        public Guid JournalSetupIdentifier { get; set; }
        public string FieldType { get; set; }
        public int Sequence { get; set; }
        public bool FieldIsRequired { get; set; }

        public virtual QJournalSetup JournalSetup { get; set; }
    }
}
