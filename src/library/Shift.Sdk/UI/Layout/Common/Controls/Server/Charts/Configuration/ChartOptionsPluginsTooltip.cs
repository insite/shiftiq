using System;
using System.ComponentModel;

using Newtonsoft.Json;

using Shift.Constant;
using Shift.Sdk.UI;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartOptionsPluginsTooltip
    {
        #region Properties

        [DefaultValue(ChartInteractionMode.Nearest)]
        [JsonProperty(PropertyName = "mode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(JsonChartInteractionModeConverter))]
        public ChartInteractionMode InteractionMode { get; set; }

        [DefaultValue(true)]
        [JsonProperty(PropertyName = "intersect", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Intersect { get; set; }

        [JsonProperty(PropertyName = "callbacks")]
        public ChartOptionsPluginsTooltipCallbacks Callbacks { get; private set; }

        #endregion

        #region Construction

        public ChartOptionsPluginsTooltip()
        {
            InteractionMode = ChartInteractionMode.Nearest;
            Intersect = true;
            Callbacks = new ChartOptionsPluginsTooltipCallbacks();
        }

        #endregion
    }
}
