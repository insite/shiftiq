using System.Collections;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class PieChartDataset : IChartDataset, IReadOnlyList<PieChartDatasetItem>
    {
        #region Properties

        public PieChartDatasetItem this[int index] => new PieChartDatasetItem(index, _dataset, _labels);

        public int Count => _labels.Count;

        public string Label
        {
            get => _dataset.Label;
            set => _dataset.Label = value;
        }

        public bool Fill
        {
            get => _dataset.Fill;
            set => _dataset.Fill = value;
        }
        
        #endregion

        #region Fields

        private PieChartDatasetConfig _dataset;
        private IList<string> _labels;

        #endregion

        #region Construction

        internal PieChartDataset(PieChartDatasetConfig dataset, IList<string> labels)
        {
            _dataset = dataset;
            _labels = labels;
        }

        #endregion

        #region Methods

        public PieChartDatasetItem NewItem()
        {
            _dataset.Add();

            if (_dataset.Data.Count > _labels.Count)
                _labels.Add(string.Empty);

            return new PieChartDatasetItem(_dataset.Data.Count - 1, _dataset, _labels);
        }

        public PieChartDatasetItem NewItem(string label, double value)
        {
            var item = NewItem();

            item.Label = label;
            item.Value = value;

            return item;
        }

        #endregion

        #region IEnumerator

        public IEnumerator<PieChartDatasetItem> GetEnumerator()
        {
            for (var i = 0; i < _labels.Count; i++)
                yield return new PieChartDatasetItem(i, _dataset, _labels);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
