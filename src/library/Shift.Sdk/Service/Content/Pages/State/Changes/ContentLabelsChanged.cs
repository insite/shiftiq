using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class ContentLabelsChanged : Change
    {
        public string ContentLabels { get; set; }
        public ContentLabelsChanged(string contentLabels)
        {
            ContentLabels = contentLabels;
        }
    }
}
