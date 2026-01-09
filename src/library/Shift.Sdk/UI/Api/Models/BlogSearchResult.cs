using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BlogSearchResult
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "day")]
        public string Day { get; set; }

        [JsonProperty(PropertyName = "month")]
        public string Month { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }
    }
}