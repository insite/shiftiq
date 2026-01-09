using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class NavigationUrlChanged : Change
    {
        public string NavigateUrl { get; set; }
        public NavigationUrlChanged(string navigateUrl)
        {
            NavigateUrl = navigateUrl;
        }
    }
}
