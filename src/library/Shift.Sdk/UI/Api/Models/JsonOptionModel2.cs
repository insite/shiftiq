using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class JsonOptionModel2
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "subtype")]
        public string Subtype { get; set; }

        [JsonProperty(PropertyName = "number")]
        public int Number { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "parentSubtype")]
        public string ParentSubtype { get; set; }

        [JsonProperty(PropertyName = "parentNumber")]
        public int? ParentNumber { get; set; }

        [JsonProperty(PropertyName = "parentTitle")]
        public string ParentTitle { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonIgnore]
        public string ContentTitle { get; set; }
    }
}