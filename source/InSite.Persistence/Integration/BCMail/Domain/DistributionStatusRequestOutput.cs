using System;

using Newtonsoft.Json;

namespace InSite.Persistence.Integration.BCMail
{
    [Serializable]
    public class DistributionStatusRequestOutput
    {
        [JsonProperty("jobs")]
        public DistributionJob[] Jobs { get; set; }
    }
}
