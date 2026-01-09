using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Sites.Pages
{
    public class PageContentChanged : Change
    {
        public ContentContainer Content { get; }

        public PageContentChanged(ContentContainer content)
        {
            Content = content;
        }
    }
}
