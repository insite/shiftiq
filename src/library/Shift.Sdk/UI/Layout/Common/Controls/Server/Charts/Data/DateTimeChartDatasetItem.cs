using System;

namespace Shift.Sdk.UI
{
    public class DateTimeChartDatasetItem
    {
        #region Properties

        public DateTime Date
        {
            get => _dataset.Data[_index].Date;
            set => _dataset.Data[_index].Date = value;
        }

        public double Value
        {
            get => _dataset.Data[_index].Value;
            set => _dataset.Data[_index].Value = value;
        }

        #endregion

        #region Fields

        private int _index;
        private DateTimeChartDatasetConfig _dataset;

        #endregion

        #region Construction

        internal DateTimeChartDatasetItem(int index, DateTimeChartDatasetConfig dataset)
        {
            _index = index;
            _dataset = dataset;
        }

        #endregion
    }
}