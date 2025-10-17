using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BarChartData : IChartData
    {
        #region Properties

        [JsonProperty(PropertyName = "datasets")]
        private IReadOnlyList<BarChartDatasetConfig> Datasets => _datasets;

        [JsonProperty(PropertyName = "labels")]
        private IReadOnlyList<string> Labels => _labels;

        public bool IsEmpty => _labels.Count == 0;

        #endregion

        #region Fields

        private BarChartDatasetCollection _datasets = new BarChartDatasetCollection();
        private List<string> _labels = new List<string>();

        #endregion

        #region Methods

        public void Clear()
        {
            _datasets.Clear();
            _labels.Clear();
        }

        public BarChartDataset CreateDataset(string id) =>
            new BarChartDataset(_datasets.Create(id), _labels);

        public IReadOnlyList<BarChartDataset> GetDatasets() =>
            _datasets.Select(x => new BarChartDataset(x, _labels)).ToArray();

        public BarChartDataset GetDataset(int index) =>
            new BarChartDataset(_datasets[index], _labels);

        public BarChartDataset GetDataset(string id) =>
            new BarChartDataset(_datasets[id], _labels);

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