using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class LineChartDatasetItem
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

        #endregion

        #region Fields

        private int _index;
        private LineChartDatasetConfig _dataset;
        private IList<string> _labels;

        #endregion

        #region Construction

        internal LineChartDatasetItem(int index, LineChartDatasetConfig dataset, IList<string> labels)
        {
            _index = index;
            _dataset = dataset;
            _labels = labels;
        }

        #endregion
    }
}