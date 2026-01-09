using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class JsonOptionModel1
    {
        [JsonProperty(PropertyName = "number")]
        public int Number { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
    }
}