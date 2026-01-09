using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupLocationChanged : Change
    {
        public string Office { get; }
        public string Region { get; }
        public string ShippingPreference { get; }
        public string WebSiteUrl { get; }

        public GroupLocationChanged(string office, string region, string shippingPreference, string webSiteUrl)
        {
            Office = office;
            Region = region;
            ShippingPreference = shippingPreference;
            WebSiteUrl = webSiteUrl;
        }
    }
}
