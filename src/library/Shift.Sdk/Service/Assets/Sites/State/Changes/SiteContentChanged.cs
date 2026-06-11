using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Sites.Sites
{
    public class SiteContentChanged : Change
    {
        public ContentContainer Content { get; }

        public SiteContentChanged(ContentContainer content)
        {
            Content = content;
        }
    }
}
