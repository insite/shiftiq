using System.ComponentModel;

namespace Shift.Constant
{
    public enum FormMessageType
    {
        [Description("Notify administrator when an attempt is started")]
        WhenAttemptStartedNotifyAdmin,

        [Description("Notify administrator when an attempt is completed")]
        WhenAttemptCompletedNotifyAdmin
    }
}
