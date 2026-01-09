
using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class HtmlResultItem
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}