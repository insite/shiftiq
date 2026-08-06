using System;

using Shift.Constant;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardChartLayout
    {
        public const int DefaultSize = 6;

        public ChartPosition Position { get; set; }
        public int? Size { get; set; }

        public int ChartColumns
        {
            get
            {
                var size = Size ?? 6;
                return size < 1 ? 1 : (size > 11 ? 11 : size);
            }
        }

        public int TableColumns => 12 - ChartColumns;
    }
}
