using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardChartDataset
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Color { get; set; }
        public string BackgroundColor { get; set; }
        public bool AutoColors { get; set; }
    }
}
