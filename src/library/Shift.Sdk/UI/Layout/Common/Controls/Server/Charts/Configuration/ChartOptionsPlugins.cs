using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartOptionsPlugins
    {
        [JsonProperty(PropertyName = "legend")]
        public ChartOptionsPluginsLegend Legend { get; private set; }

        [JsonProperty(PropertyName = "tooltip")]
        public ChartOptionsPluginsTooltip Tooltip { get; private set; }

        public ChartOptionsPlugins()
        {
            Legend = new ChartOptionsPluginsLegend();
            Tooltip = new ChartOptionsPluginsTooltip();
        }
    }
}