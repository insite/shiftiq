using System;
using System.Collections.Generic;
using System.ComponentModel;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DateTimeChartDatasetConfig : BaseChartDataset
    {
        #region Properties

        [DefaultValue(0.4)]
        [JsonProperty(PropertyName = "lineTension", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public decimal LineTension { get; set; }

        [DefaultValue(3)]
        [JsonProperty(PropertyName = "pointRadius", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int PointRadius { get; set; }

        [JsonProperty(PropertyName = "data")]
        public IList<DateTimeChartDatasetItemConfig> Data => _data;

        [JsonProperty(PropertyName = "backgroundColor", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate), JsonConverter(typeof(JsonColorConverter))]
        public System.Drawing.Color? BackgroundColor { get; set; }

        [JsonProperty(PropertyName = "borderColor", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate), JsonConverter(typeof(JsonColorConverter))]
        public System.Drawing.Color? BorderColor { get; set; }

        [DefaultValue(3)]
        [JsonProperty(PropertyName = "borderWidth", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int BorderWidth { get; set; }

        public override int Count => _data.Count;

        #endregion

        #region Fields

        private List<DateTimeChartDatasetItemConfig> _data = new List<DateTimeChartDatasetItemConfig>();

        #endregion

        #region Construction

        public DateTimeChartDatasetConfig(string id)
            : base(id)
        {
            LineTension = 0.4M;
            PointRadius = 3;
            BorderWidth = 3;
        }

        #endregion

        #region Methods

        public void Add() => _data.Add(new DateTimeChartDatasetItemConfig());

        public void Remove(int index) => _data.RemoveAt(index);

        #endregion
    }
}
