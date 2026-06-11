using System;

using Newtonsoft.Json;

namespace InSite.Persistence.Integration.BCMail
{
    [Serializable]
    public class DistributionCreateRequestInput
    {
        [JsonProperty("job")]
        public DistributionRequest Request { get; set; }

        public string Serialize()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(this, Formatting.Indented, settings);
        }
    }
}
