using System.ComponentModel;

namespace Shift.Constant
{
    public enum PublicationStatus
    {
        [Description("Drafted (In Development)")]
        Drafted,

        [Description("Published")]
        Published,

        [Description("Unpublished")]
        Unpublished,

        [Description("Archived")]
        Archived,
    }
}
