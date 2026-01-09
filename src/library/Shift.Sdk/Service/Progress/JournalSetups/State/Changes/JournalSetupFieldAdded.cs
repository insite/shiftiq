using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class JournalSetupFieldAdded : Change
    {
        public Guid Field { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public JournalSetupFieldType Type { get; }

        public int Sequence { get; }
        public bool IsRequired { get; }

        public JournalSetupFieldAdded(Guid field, JournalSetupFieldType type, int sequence, bool isRequired)
        {
            Field = field;
            Type = type;
            Sequence = sequence;
            IsRequired = isRequired;
        }
    }
}
