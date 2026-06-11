using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class SlugChanged : Change
    {
        public string Slug { get; set; }
        public SlugChanged(string slug)
        {
            Slug = slug;
        }
    }
}
