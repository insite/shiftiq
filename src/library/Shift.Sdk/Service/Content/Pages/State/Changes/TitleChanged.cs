using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class TitleChanged : Change
    {
        public string Title { get; set; }
        public TitleChanged(string title)
        {
            Title = title;
        }
    }
}
