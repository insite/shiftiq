using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupFieldChanged : Change
    {
        public Guid Field { get; }
        public bool IsRequired { get; }

        public JournalSetupFieldChanged(Guid field, bool isRequired)
        {
            Field = field;
            IsRequired = isRequired;
        }
    }
}
