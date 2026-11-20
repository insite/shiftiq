using System;
using System.Linq;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardChartSettings
    {
        public DashboardChartType Type { get; set; }
        public DashboardChartDataType DataType { get; set; }
        public string Label { get; set; }
        public DashboardQuery Query { get; set; }
        public bool Stacked { get; set; }
        public string Height { get; set; }

        public DashboardChartLegend Legend { get; set; }
        public DashboardChartLayout Layout { get; set; }
        public DashboardChartDataset[] Datasets { get; set; }

        public DashboardChartSettings Clone() => new DashboardChartSettings
        {
            Type = Type,
            DataType = DataType,
            Label = Label,
            Query = Query?.Clone(),
            Stacked = Stacked,
            Height = Height,
            Legend = Legend?.Clone(),
            Layout = Layout?.Clone(),
            Datasets = Datasets?.Select(d => d.Clone()).ToArray()
        };
    }
}
