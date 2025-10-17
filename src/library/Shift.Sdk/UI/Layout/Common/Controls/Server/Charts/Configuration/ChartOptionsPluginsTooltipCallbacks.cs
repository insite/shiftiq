using System;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartOptionsPluginsTooltipCallbacks
    {
        [JsonConverter(typeof(JsonJsFunctionConverter))]
        [JsonProperty(PropertyName = "title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string TitleJsFunction { get; set; }

        [JsonConverter(typeof(JsonJsFunctionConverter))]
        [JsonProperty(PropertyName = "label", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LabelJsFunction { get; set; }
    }
}
