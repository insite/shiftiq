using System.ComponentModel;

namespace Shift.Constant
{
    public enum FlagType
    {
        [Description("None")]
        None,

        [Description("Black"), ContextualClass("dark")]
        Black,
        
        [Description("Blue"), ContextualClass("primary")]
        Blue,

        [Description("Cyan"), ContextualClass("info")]
        Cyan,

        [Description("Red"), ContextualClass("danger")]
        Red,

        [Description("Gray"), ContextualClass("default")]
        Gray,

        [Description("Green"), ContextualClass("success")]
        Green,

        [Description("Yellow"), ContextualClass("warning")]
        Yellow,

        [Description("White"), ContextualClass("light")]
        White
    }
}