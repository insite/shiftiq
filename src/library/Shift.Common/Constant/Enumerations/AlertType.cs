namespace Shift.Constant
{
    public enum AlertType
    {
        [ContextualClass("light")]
        None,

        [Icon("stop-circle"), ContextualClass("danger")]
        Error,

        [Icon("info-square"), ContextualClass("info")]
        Information,

        [Icon("check-circle"), ContextualClass("success")]
        Success,

        [Icon("exclamation-triangle"), ContextualClass("warning")]
        Warning
    }
}