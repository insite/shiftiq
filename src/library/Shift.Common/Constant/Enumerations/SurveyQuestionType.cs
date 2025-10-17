using System.ComponentModel;

namespace Shift.Constant
{
    public enum SurveyQuestionType
    {
        [Description("Break Question"), ContextualClass("warning")]
        BreakQuestion,

        [Description("Break Page"), ContextualClass("danger")]
        BreakPage,

        [Description("Check Box List"), ContextualClass("primary")]
        CheckList,

        [Description("Comment Box"), ContextualClass("info")]
        Comment,

        [Description("Date Selector"), ContextualClass("info")]
        Date,

        [Description("Likert Table"), ContextualClass("success")]
        Likert,

        [Description("Number Box"), ContextualClass("info")]
        Number,

        [Description("Radio Button List"), ContextualClass("primary")]
        RadioList,

        [Description("Dropdown List"), ContextualClass("primary")]
        Selection,

        [Description("Terminate Survey"), ContextualClass("danger")]
        Terminate,

        [Description("Text"), ContextualClass("info")]
        Text,

        [Description("File Upload"), ContextualClass("info")]
        Upload
    }
}