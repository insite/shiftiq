using System.ComponentModel;

namespace Shift.Constant
{
    public enum GradeItemWeighting
    {
        [Description("None")]
        None,

        [Description("Equally weight all items")]
        Equally,

        [Description("Equally weight all items and missing scores")]
        EquallyWithNulls,

        [Description("Weight items by percent")]
        ByPercent,

        [Description("Weight items by points")]
        ByPoints,

        [Description("Sum all items")]
        Sum
    }
}