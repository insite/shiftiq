using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Payments
{
    public class GatewayOpened : Change
    {
        public GatewayOpened(Guid tenant, GatewayType type, string name)
        {
            Tenant = tenant;
            Type = type;
            Name = name;
        }

        public Guid Tenant { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public GatewayType Type { get; }

        public string Name { get; }
    }
}
