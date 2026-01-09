using System;
using System.ComponentModel;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartOptions
    {
        #region Properties

        [DefaultValue(true)]
        [JsonProperty(PropertyName = "responsive", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Responsive { get; set; }

        [DefaultValue(true)]
        [JsonProperty(PropertyName = "maintainAspectRatio", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool MaintainAspectRatio { get; set; }

        [JsonConverter(typeof(JsonJsFunctionConverter))]
        [JsonProperty(PropertyName = "onHover", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OnHoverJsFunction { get; set; }

        [JsonProperty(PropertyName = "plugins")]
        public ChartOptionsPlugins Plugins { get; private set; }

        [JsonProperty(PropertyName = "animation")]
        public ChartOptionsAnimation Animation { get; private set; }

        #endregion

        #region Construction

        internal ChartOptions()
        {
            Responsive = true;
            MaintainAspectRatio = true;

            Plugins = new ChartOptionsPlugins();
            Animation = new ChartOptionsAnimation();
        }

        #endregion
    }
}
