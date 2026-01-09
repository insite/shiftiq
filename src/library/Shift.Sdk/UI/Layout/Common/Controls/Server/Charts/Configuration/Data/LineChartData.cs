using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class LineChartData : IChartData
    {
        #region Properties

        [JsonProperty(PropertyName = "datasets")]
        private IReadOnlyList<LineChartDatasetConfig> Datasets => _datasets;

        [JsonProperty(PropertyName = "labels")]
        private IReadOnlyList<string> Labels => _labels;

        public bool IsEmpty => _labels.Count == 0;

        #endregion

        #region Fields

        private LineChartDatasetCollection _datasets = new LineChartDatasetCollection();
        private List<string> _labels = new List<string>();

        #endregion
        
        #region Methods

        public void Clear()
        {
            _datasets.Clear();
            _labels.Clear();
        }

        public LineChartDataset CreateDataset(string id) =>
            new LineChartDataset(_datasets.Create(id), _labels);

        public IReadOnlyList<LineChartDataset> GetDatasets() =>
            _datasets.Select(x => new LineChartDataset(x, _labels)).ToArray();

        public LineChartDataset GetDataset(int index) =>
            new LineChartDataset(_datasets[index], _labels);

        public LineChartDataset GetDataset(string id) =>
            new LineChartDataset(_datasets[id], _labels);

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
