using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class VisibilityChanged : Change
    {
        public bool IsHidden { get; set; }
        public VisibilityChanged(bool isHidden)
        {
            IsHidden = isHidden;
        }
    }
}
