using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DateTimeChartData : IChartData
    {
        #region Properties

        [JsonProperty(PropertyName = "datasets")]
        private IEnumerable<DateTimeChartDatasetConfig> Datasets => _datasets;

        public bool IsEmpty => _datasets.Count == 0;

        #endregion

        #region Fields

        private DateTimeChartDatasetCollection _datasets = new DateTimeChartDatasetCollection();

        #endregion

        #region Methods

        public void Clear()
        {
            _datasets.Clear();
        }

        public DateTimeChartDataset CreateDataset(string id) =>
            new DateTimeChartDataset(_datasets.Create(id));

        public IReadOnlyList<DateTimeChartDataset> GetDatasets() =>
            _datasets.Select(x => new DateTimeChartDataset(x)).ToArray();

        public DateTimeChartDataset GetDataset(int index) =>
            new DateTimeChartDataset(_datasets[index]);

        public DateTimeChartDataset GetDataset(string id) =>
            new DateTimeChartDataset(_datasets[id]);

        #endregion

        #region IChartData

        void IChartData.Clear() => Clear();

        IChartDataset IChartData.CreateDataset(string id) => CreateDataset(id);

        IReadOnlyList<IChartDataset> IChartData.GetDatasets() => GetDatasets();

        IChartDataset IChartData.GetDataset(int index) => GetDataset(index);

        IChartDataset IChartData.GetDataset(string id) => GetDataset(id);

        #endregion
    }
}
