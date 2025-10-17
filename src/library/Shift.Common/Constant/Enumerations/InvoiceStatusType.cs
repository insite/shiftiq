using System.ComponentModel;

namespace Shift.Constant
{
    public enum InvoiceStatusType 
    {
        [Description("Paid")]
        Paid,

        [Description("Payment Failed")]
        PaymentFailed,

        [Description("Awaiting Payment")]
        Submitted,

        [Description("Cancelled")]
        Cancelled,

        [Description("Refunded")]
        Refunded,

        [Description("Drafted")]
        Drafted
    }
}
