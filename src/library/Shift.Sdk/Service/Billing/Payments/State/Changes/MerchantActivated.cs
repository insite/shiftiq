using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Payments
{
    public class MerchantActivated : Change
    {
        public MerchantActivated(Guid tenant, EnvironmentName environment, string token)
        {
            Tenant = tenant;
            Environment = environment;
            Token = token;
        }

        public Guid Tenant { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public EnvironmentName Environment { get; }

        public string Token { get; }
    }
}