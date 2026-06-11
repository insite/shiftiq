using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Records
{
    public class RubricTranslated : Change
    {
        public ContentContainer Content { get; }

        public RubricTranslated(ContentContainer content)
        {
            Content = content;
        }
    }
}
