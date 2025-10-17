using System.ComponentModel;

namespace Shift.Constant
{
    public enum UserFeedbackType
    {
        [Description("Summary Feedback")]
        Summary,

        [Description("Detailed Feedback")]
        Detailed,

        [Description("Detailed Feedback, answered questions only")]
        Answered,

        [Description("Feedback Disabled")]
        Disabled
    }
}