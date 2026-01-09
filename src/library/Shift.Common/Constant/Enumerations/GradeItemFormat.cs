using System.ComponentModel;

namespace Shift.Constant
{
    public enum GradeItemFormat
    {
        [Description("None"), ContextualClass("danger")]
        None,

        [Description("Score (%)"), ContextualClass("success")]
        Percent,

        [Description("Text"), ContextualClass("warning")]
        Text,

        [Description("Number"), ContextualClass("info")]
        Number,

        [Description("Points"), ContextualClass("info")]
        Point,

        [Description("Complete | Incomplete"), ContextualClass("primary")]
        Boolean 
    }
}