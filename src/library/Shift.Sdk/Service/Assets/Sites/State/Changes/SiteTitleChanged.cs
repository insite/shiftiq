using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Sites
{
    public class SiteTitleChanged : Change
    {
        public string Title { get; set; }
        public SiteTitleChanged(string title)
        {
            Title = title;
        }
    }
}
