using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartOptionsScales
    {
        #region Classes

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class Item
        {
            [JsonProperty(PropertyName = "stacked", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool Stacked { get; set; }

            [JsonProperty(PropertyName = "beginAtZero", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool BeginAtZero { get; set; }
        }

        #endregion

        #region Properties

        [JsonProperty(PropertyName = "x")]
        public Item X { get; private set; }

        [JsonProperty(PropertyName = "y")]
        public Item Y { get; private set; }

        #endregion

        #region Construction

        public ChartOptionsScales()
        {
            X = new Item();
            Y = new Item();
        }

        #endregion
    }
}
