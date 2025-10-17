using System.ComponentModel;

namespace Shift.Constant
{
    public enum SpecificationTabTimeLimit
    {
        [Description("Disabled")]
        Disabled,

        [Description("Enabled for some tabs")]
        SomeTabs,

        [Description("Enabled for all tabs")]
        AllTabs
    }
}
