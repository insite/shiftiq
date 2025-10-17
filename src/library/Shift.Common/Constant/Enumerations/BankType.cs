using System.ComponentModel;

namespace Shift.Constant
{
    public enum BankType
    {
        [Description("Advanced")]
        Advanced,

        [Description("Basic")]
        Basic,

        // We need to keep these two enumeration values until after they are removed from our data. Otherwise the
        // deserialization of existing bank aggregates will fail.
        SingleForm, MultipleForm
    }
}