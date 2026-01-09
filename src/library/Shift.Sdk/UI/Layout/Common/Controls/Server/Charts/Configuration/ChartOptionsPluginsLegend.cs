using System;
using System.ComponentModel;

using Newtonsoft.Json;

using Shift.Constant;
using Shift.Sdk.UI;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartOptionsPluginsLegend
    {
        #region Properties

        [DefaultValue(true)]
        [JsonProperty(PropertyName = "display", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Visible { get; set; }

        [DefaultValue(ChartPosition.Top)]
        [JsonConverter(typeof(JsonChartPositionConverter))]
        [JsonProperty(PropertyName = "position", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ChartPosition Position { get; set; }

        [JsonConverter(typeof(JsonJsFunctionConverter))]
        [JsonProperty(PropertyName = "onClick", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string OnClickJsFunction { get; set; }

        [JsonConverter(typeof(JsonJsFunctionConverter))]
        [JsonProperty(PropertyName = "onHover", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string OnHoverJsFunction { get; set; }

        #endregion

        #region Construction

        public ChartOptionsPluginsLegend()
        {
            Visible = true;
            Position = ChartPosition.Top;
        }

        #endregion
    }
}
