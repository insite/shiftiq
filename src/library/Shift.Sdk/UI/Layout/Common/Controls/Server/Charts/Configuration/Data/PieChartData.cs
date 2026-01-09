using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace Shift.Sdk.UI
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PieChartData : IChartData
    {
        #region Properties

        [JsonProperty(PropertyName = "datasets")]
        private IReadOnlyList<PieChartDatasetConfig> Datasets => _datasets;

        [JsonProperty(PropertyName = "labels")]
        private IReadOnlyList<string> Labels => _labels;

        public bool IsEmpty => _labels.Count == 0;

        #endregion

        #region Fields

        private PieChartDatasetCollection _datasets = new PieChartDatasetCollection();
        private List<string> _labels = new List<string>();

        #endregion

        #region Methods

        public void Clear()
        {
            _datasets.Clear();
            _labels.Clear();
        }

        public PieChartDataset CreateDataset(string id) =>
            new PieChartDataset(_datasets.Create(id), _labels);

        public IReadOnlyList<PieChartDataset> GetDatasets() =>
            _datasets.Select(x => new PieChartDataset(x, _labels)).ToArray();

        public PieChartDataset GetDataset(int index) =>
            new PieChartDataset(_datasets[index], _labels);

        public PieChartDataset GetDataset(string id) =>
            new PieChartDataset(_datasets[id], _labels);

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
