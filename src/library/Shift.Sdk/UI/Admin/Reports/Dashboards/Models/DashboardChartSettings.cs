using System;

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
    }
}
