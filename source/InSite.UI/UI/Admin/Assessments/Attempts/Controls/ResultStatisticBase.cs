using System;
using System.Collections.Generic;
using System.Drawing;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public class ResultStatisticBase : BaseUserControl
    {
        protected class BellCurveData
        {
            #region Properties

            internal decimal MaxValue => _max;
            internal decimal MinValue => _min;
            internal decimal SumValue => _sum;

            #endregion

            #region Fields

            private int[] _data;
            private decimal _dataStep;

            private List<double> _values;

            private decimal _max;
            private decimal _min;
            private decimal _sum;

            #endregion

            #region Construction

            public BellCurveData()
            {
                _data = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                _dataStep = 1m / _data.Length;

                _values = new List<double>();

                _max = decimal.MinValue;
                _min = decimal.MaxValue;
                _sum = 0m;
            }

            #endregion

            #region Methods

            internal void AddValue(decimal value)
            {
                value = Number.CheckRange(value, 0, 1);

                _values.Add((double)value);
                _sum += value;

                var index = (int)Math.Floor(value / _dataStep);
                if (index >= _data.Length)
                    index = _data.Length - 1;

                _data[index] += 1;

                if (value > _max)
                    _max = value;

                if (value < _min)
                    _min = value;
            }

            internal void BindChart(Common.Web.UI.Chart.BarChart chart)
            {
                chart.Data.Clear();

                var dataset = (BarChartDataset)chart.Data.CreateDataset("bell_curve");
                dataset.Label = "Results";
                dataset.BackgroundColor = ColorTranslator.FromHtml("#86c557");

                dataset.NewItem("< 10%", _data[0]);
                dataset.NewItem("10% - 20%", _data[1]);
                dataset.NewItem("20% - 30%", _data[2]);
                dataset.NewItem("30% - 40%", _data[3]);
                dataset.NewItem("40% - 50%", _data[4]);
                dataset.NewItem("50% - 60%", _data[5]);
                dataset.NewItem("60% - 70%", _data[6]);
                dataset.NewItem("70% - 80%", _data[7]);
                dataset.NewItem("80% - 90%", _data[8]);
                dataset.NewItem("> 90%", _data[9]);
            }

            internal double GetMedian() => Calculator.CalculateMedian(_values);
            internal double GetStandardDeviation() => Calculator.CalculateStandardDeviation(_values) * 100D;

            #endregion
        }
    }
}