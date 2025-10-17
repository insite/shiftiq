using System.ComponentModel;

namespace Shift.Constant
{
    public enum DateRangeShortcut
    {
        [Description("Today")]
        Today,

        [Description("Yesterday")]
        Yesterday,

        [Description("This Week")]
        ThisWeek,

        [Description("Last Week")]
        LastWeek,

        [Description("This Month")]
        ThisMonth,

        [Description("Last Month")]
        LastMonth,

        [Description("This Year")]
        ThisYear,

        [Description("Last Year")]
        LastYear
    }
}
