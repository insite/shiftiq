using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupFieldsReordered : Change
    {
        public (Guid FieldIdentifier, int Sequence)[] Fields { get; set; }

        public JournalSetupFieldsReordered((Guid, int)[] fields)
        {
            Fields = fields;
        }
    }
}
