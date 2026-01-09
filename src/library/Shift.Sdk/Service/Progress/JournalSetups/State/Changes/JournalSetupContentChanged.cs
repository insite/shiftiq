using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Records
{
    public class JournalSetupContentChanged : Change
    {
        public ContentContainer Content { get; }

        public JournalSetupContentChanged(ContentContainer content)
        {
            Content = content;
        }
    }
}
