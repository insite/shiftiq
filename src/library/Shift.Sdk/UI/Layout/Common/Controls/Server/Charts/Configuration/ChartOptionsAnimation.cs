using System;
using System.ComponentModel;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartOptionsAnimation
    {
        [DefaultValue(1000)]
        [JsonProperty(PropertyName = "duration", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int Duration { get; set; } = 1000;

        [JsonProperty(PropertyName = "resize")]
        public ChartOptionsAnimationResize Resize { get; private set; }

        public ChartOptionsAnimation()
        {
            Resize = new ChartOptionsAnimationResize();
        }
    }
}