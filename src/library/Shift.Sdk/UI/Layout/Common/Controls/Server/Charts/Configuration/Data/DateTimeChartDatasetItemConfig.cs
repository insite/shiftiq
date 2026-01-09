using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DateTimeChartDatasetItemConfig
    {
        #region Property

        [JsonProperty(PropertyName = "x"), JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "y")]
        public double Value { get; set; }

        #endregion

        #region Construction

        internal DateTimeChartDatasetItemConfig()
        {
            Date = DateTime.MinValue;
            Value = 0;
        }

        #endregion
    }
}
