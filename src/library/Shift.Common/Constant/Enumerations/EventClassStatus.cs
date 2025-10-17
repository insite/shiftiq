using System.ComponentModel;

namespace Shift.Constant
{
    public enum EventClassStatus
    {
        [Description("Drafted"), ContextualClass("dark")]
        Drafted,

        [Description("Published"), ContextualClass("info")]
        Published,

        [Description("In Progress"), ContextualClass("warning")]
        InProgress,

        [Description("Completed"), ContextualClass("success")]
        Completed,

        [Description("Closed"), ContextualClass("danger")]
        Closed,

        [Description("Cancelled"), ContextualClass("danger")]
        Cancelled
    }
}
