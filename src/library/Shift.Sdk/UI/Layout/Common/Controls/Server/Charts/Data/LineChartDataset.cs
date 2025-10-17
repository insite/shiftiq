using System.Collections;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class LineChartDataset : IChartDataset, IReadOnlyList<LineChartDatasetItem>
    {
        #region Properties

        public LineChartDatasetItem this[int index] => new LineChartDatasetItem(index, _dataset, _labels);

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

        public decimal LineTension
        {
            get => _dataset.LineTension;
            set => _dataset.LineTension = value;
        }

        public int PointRadius
        {
            get => _dataset.PointRadius;
            set => _dataset.PointRadius = value;
        }

        public System.Drawing.Color? BackgroundColor
        {
            get => _dataset.BackgroundColor;
            set => _dataset.BackgroundColor = value;
        }

        public System.Drawing.Color? BorderColor
        {
            get => _dataset.BorderColor;
            set => _dataset.BorderColor = value;
        }

        public int BorderWidth
        {
            get => _dataset.BorderWidth;
            set => _dataset.BorderWidth = value;
        }

        #endregion

        #region Fields

        private LineChartDatasetConfig _dataset;
        private IList<string> _labels;

        #endregion

        #region Construction

        internal LineChartDataset(LineChartDatasetConfig dataset, IList<string> labels)
        {
            _dataset = dataset;
            _labels = labels;
        }

        #endregion

        #region Methods

        public LineChartDatasetItem NewItem()
        {
            _dataset.Add();

            if (_dataset.Data.Count > _labels.Count)
                _labels.Add(string.Empty);

            return new LineChartDatasetItem(_dataset.Data.Count - 1, _dataset, _labels);
        }

        public LineChartDatasetItem NewItem(string label, double value)
        {
            var item = NewItem();

            item.Label = label;
            item.Value = value * 100d;

            return item;
        }

        #endregion

        #region IEnumerator

        public IEnumerator<LineChartDatasetItem> GetEnumerator()
        {
            for (var i = 0; i < _labels.Count; i++)
                yield return new LineChartDatasetItem(i, _dataset, _labels);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
