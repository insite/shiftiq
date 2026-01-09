using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupSocialMediaUrlChanged : Change
    {
        public string Type { get; }
        public string Url { get; }

        public GroupSocialMediaUrlChanged(string type, string url)
        {
            Type = type;
            Url = url;
        }
    }
}
