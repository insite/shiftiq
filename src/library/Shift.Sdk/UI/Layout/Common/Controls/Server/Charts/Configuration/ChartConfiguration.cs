using System;

using Newtonsoft.Json;

using Shift.Constant;
using Shift.Sdk.UI;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartConfiguration
    {
        #region Properties

        [JsonProperty(PropertyName = "type"), JsonConverter(typeof(JsonChartTypeConverter))]
        public ChartType Type { get; private set; }

        [JsonProperty(PropertyName = "data")]
        public IChartData Data { get; set; }

        [JsonProperty(PropertyName = "options")]
        public ChartOptions Options { get; private set; }

        #endregion

        #region Construction

        public ChartConfiguration(ChartType type)
        {
            Type = type;
            Options = new ChartOptions();
        }

        #endregion
    }
}
