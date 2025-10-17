using System;
using System.ComponentModel;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartOptionsAnimationResize
    {
        [DefaultValue(400)]
        [JsonProperty(PropertyName = "duration", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int Duration { get; set; } = 400;
    }
}