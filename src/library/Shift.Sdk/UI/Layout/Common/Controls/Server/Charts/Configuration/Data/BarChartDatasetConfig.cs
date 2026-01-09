using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BarChartDatasetConfig : BaseChartDataset
    {
        #region Properties

        [JsonProperty(PropertyName = "data")]
        public IList<double> Data => _data;

        [JsonProperty(PropertyName = "backgroundColor", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate), JsonConverter(typeof(JsonColorConverter))]
        public IList<System.Drawing.Color> BackgroundColor => _backgroundColor;

        [JsonProperty(PropertyName = "borderColor", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate), JsonConverter(typeof(JsonColorConverter))]
        public IList<System.Drawing.Color> BorderColor => _borderColor;

        public override int Count => _data.Count;

        #endregion

        #region Fields

        private List<double> _data = new List<double>();
        private List<System.Drawing.Color> _backgroundColor = new List<System.Drawing.Color>();
        private List<System.Drawing.Color> _borderColor = new List<System.Drawing.Color>();

        #endregion

        #region Construction

        public BarChartDatasetConfig(string id)
            : base(id)
        {
        }

        #endregion

        #region Methods

        public void Add()
        {
            _data.Add(default(double));
            _backgroundColor.Add(System.Drawing.Color.Gray);
            _borderColor.Add(System.Drawing.Color.White);
        }

        public void Remove(int index)
        {
            _data.RemoveAt(index);
            _backgroundColor.RemoveAt(index);
            _borderColor.RemoveAt(index);
        }

        #endregion
    }
}
