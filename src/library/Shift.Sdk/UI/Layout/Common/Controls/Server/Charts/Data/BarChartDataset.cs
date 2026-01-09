using System.Collections;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class BarChartDataset : IChartDataset, IReadOnlyList<BarChartDatasetItem>
    {
        #region Properties

        public BarChartDatasetItem this[int index] => new BarChartDatasetItem(index, _dataset, _labels);

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

        public System.Drawing.Color? BackgroundColor
        {
            get => _defaultBackgroundColor;
            set
            {
                _defaultBackgroundColor = value;

                for (var i = 0; i < _dataset.BackgroundColor.Count; i++)
                    _dataset.BackgroundColor[i] = value ?? System.Drawing.Color.Gray;
            }
        }

        public System.Drawing.Color? BorderColor
        {
            get => _defaultBorderColor;
            set
            {
                _defaultBorderColor = value;

                for (var i = 0; i < _dataset.BorderColor.Count; i++)
                    _dataset.BorderColor[i] = value ?? System.Drawing.Color.White;
            }
        }

        #endregion

        #region Fields

        private BarChartDatasetConfig _dataset;
        private IList<string> _labels;
        public System.Drawing.Color? _defaultBackgroundColor;
        public System.Drawing.Color? _defaultBorderColor;

        #endregion

        #region Construction

        internal BarChartDataset(BarChartDatasetConfig dataset, IList<string> labels)
        {
            _dataset = dataset;
            _labels = labels;
        }

        #endregion

        #region Methods

        public BarChartDatasetItem NewItem()
        {
            _dataset.Add();

            if (_dataset.Data.Count > _labels.Count)
                _labels.Add(string.Empty);

            var result = new BarChartDatasetItem(_dataset.Data.Count - 1, _dataset, _labels);

            if (_defaultBackgroundColor.HasValue)
                result.BackgroundColor = _defaultBackgroundColor.Value;

            if (_defaultBorderColor.HasValue)
                result.BorderColor = _defaultBorderColor.Value;

            return result;
        }

        public BarChartDatasetItem NewItem(string label, double value)
        {
            var item = NewItem();

            item.Label = label;
            item.Value = value;

            return item;
        }

        #endregion

        #region IEnumerator

        public IEnumerator<BarChartDatasetItem> GetEnumerator()
        {
            for (var i = 0; i < _labels.Count; i++)
                yield return new BarChartDatasetItem(i, _dataset, _labels);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}