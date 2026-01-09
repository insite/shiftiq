using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Records
{
    public class JournalSetupFieldContentChanged : Change
    {
        public Guid Field { get; }
        public ContentContainer Content { get; }

        public JournalSetupFieldContentChanged(Guid field, ContentContainer content)
        {
            Field = field;
            Content = content;
        }
    }
}
