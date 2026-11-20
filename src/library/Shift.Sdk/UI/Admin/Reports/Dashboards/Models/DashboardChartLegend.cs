using System;

using Shift.Constant;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardChartLegend
    {
        public bool? Visible { get; set; }
        public ChartPosition Position { get; set; }

        public DashboardChartLegend Clone() => new DashboardChartLegend
        {
            Visible = Visible,
            Position = Position
        };
    }
}
