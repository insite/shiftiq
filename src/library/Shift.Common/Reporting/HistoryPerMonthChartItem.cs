using System;

namespace Shift.Common
{
    public class HistoryPerMonthChartItem
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }

        public DateTime GetDateTime() => new DateTime(Year, Month, 1);
    }
}
