using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupLocation : Command
    {
        public string Office { get; }
        public string Region { get; }
        public string ShippingPreference { get; }
        public string WebSiteUrl { get; }

        public ChangeGroupLocation(Guid group, string office, string region, string shippingPreference, string webSiteUrl)
        {
            AggregateIdentifier = group;
            Office = office.NullIfEmpty();
            Region = region.NullIfEmpty();
            ShippingPreference = shippingPreference.NullIfEmpty();
            WebSiteUrl = webSiteUrl.NullIfEmpty();
        }
    }
}
