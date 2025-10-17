using System;
using System.ComponentModel;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class BaseChartDataset
    {
        #region Properties

        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [DefaultValue(false)]
        [JsonProperty(PropertyName = "fill", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Fill { get; set; }

        public abstract int Count { get; }

        #endregion

        #region Construction

        protected BaseChartDataset(string id)
        {
            Id = id;
        }

        #endregion
    }
}
