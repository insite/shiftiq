using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartTitle
    {
        [JsonProperty(PropertyName = "display")]
        public bool Visible { get; set; }

        [JsonProperty(PropertyName = "text", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string[] Text { get; set; }

        public ChartTitle()
        {
            Visible = true;
        }
    }
}
