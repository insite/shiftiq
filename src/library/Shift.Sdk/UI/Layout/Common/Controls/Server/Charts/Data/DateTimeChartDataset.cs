using System;
using System.Collections;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class DateTimeChartDataset : IChartDataset, IReadOnlyList<DateTimeChartDatasetItem>
    {
        #region Properties

        public DateTimeChartDatasetItem this[int index] => new DateTimeChartDatasetItem(index, _dataset);

        public int Count => _dataset.Count;

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

        private DateTimeChartDatasetConfig _dataset;

        #endregion

        #region Construction

        internal DateTimeChartDataset(DateTimeChartDatasetConfig dataset)
        {
            _dataset = dataset;
        }

        #endregion

        #region Methods

        public DateTimeChartDatasetItem NewItem()
        {
            _dataset.Add();

            return new DateTimeChartDatasetItem(_dataset.Data.Count - 1, _dataset);
        }

        public DateTimeChartDatasetItem NewItem(DateTime date, double value)
        {
            var item = NewItem();

            item.Date = date;
            item.Value = value;

            return item;
        }

        #endregion

        #region IEnumerator

        public IEnumerator<DateTimeChartDatasetItem> GetEnumerator()
        {
            for (var i = 0; i < _dataset.Count; i++)
                yield return new DateTimeChartDatasetItem(i, _dataset);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}