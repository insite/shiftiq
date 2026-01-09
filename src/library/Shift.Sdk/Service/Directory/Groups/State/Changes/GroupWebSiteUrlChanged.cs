using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupWebSiteUrlChanged : Change
    {
        public string WebSiteUrl { get; }

        public GroupWebSiteUrlChanged(string webSiteUrl)
        {
            WebSiteUrl = webSiteUrl;
        }
    }
}
