using System;

using Newtonsoft.Json;

namespace InSite.Persistence.Integration.BCMail
{
    [Serializable]
    public class DistributionJob
    {
        [JsonProperty("jobId")]
        public string Code { get; set; }

        [JsonProperty("jobStatus")]
        public string Status { get; set; }

        [JsonProperty("errors")]
        public string Errors { get; set; }
    }
}