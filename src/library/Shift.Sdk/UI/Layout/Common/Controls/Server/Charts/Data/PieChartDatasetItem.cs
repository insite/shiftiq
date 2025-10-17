using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class PieChartDatasetItem
    {
        #region Properties

        public string Label
        {
            get => _labels[_index];
            set => _labels[_index] = value;
        }

        public double Value
        {
            get => _dataset.Data[_index];
            set => _dataset.Data[_index] = value;
        }

        public System.Drawing.Color BackgroundColor
        {
            get => _dataset.BackgroundColor[_index];
            set => _dataset.BackgroundColor[_index] = value;
        }

        public System.Drawing.Color BorderColor
        {
            get => _dataset.BorderColor[_index];
            set => _dataset.BorderColor[_index] = value;
        }

        #endregion

        #region Fields

        private int _index;
        private PieChartDatasetConfig _dataset;
        private IList<string> _labels;

        #endregion

        #region Construction

        internal PieChartDatasetItem(int index, PieChartDatasetConfig dataset, IList<string> labels)
        {
            _index = index;
            _dataset = dataset;
            _labels = labels;
        }

        #endregion
    }
}