using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class IconChanged : Change
    {
        public string Icon { get; set; }

        public IconChanged(string icon)
        {
            Icon = icon;
        }
    }
}
