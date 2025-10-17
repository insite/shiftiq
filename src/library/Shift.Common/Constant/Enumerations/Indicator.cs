using System.ComponentModel;

namespace Shift.Constant
{
    public enum Indicator
    {
        [ContextualClass("primary")]
        None,

        [Description("Grey"), ContextualClass("default")]
        Default,

        [Description("Blue"), ContextualClass("primary")]
        Primary,

        [Description("Green"), ContextualClass("success")]
        Success,

        [Description("Cyan"), ContextualClass("info")]
        Info,

        [Description("Yellow"), ContextualClass("warning")]
        Warning,

        [Description("Red"), ContextualClass("danger")]
        Danger,

        [Description("White"), ContextualClass("light")]
        Light,

        [Description("Black"), ContextualClass("dark")]
        Dark,
    }
}
