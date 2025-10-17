using System;

using Shift.Constant;

namespace InSite.Domain.Records
{
    [Serializable]
    public class JournalSetupField
    {
        public static class ContentLabels
        {
            public const string LabelText = "Label Text";
            public const string HelpText = "Help Text";
        };

        public Guid Identifier { get; set; }
        public JournalSetupFieldType Type { get; set; }
        public int Sequence { get; set; }
        public bool IsRequired { get; set; }

        public Shift.Common.ContentContainer Content { get; set; }
    }
}
