using System;

using Newtonsoft.Json;

namespace InSite.Persistence.Integration.BCMail
{
    [Serializable]
    public class DistributionStatusRequestInput
    {
        [JsonProperty("jobs")]
        public string[] Jobs { get; set; }
    }
}
