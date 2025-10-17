using System.ComponentModel;

namespace Shift.Constant
{
    public enum ReferenceMaterialType
    {
        [Description("None")]
        None = 0,

        [Description("Acronyms")]
        Acronyms = 1,

        [Description("Formulas")]
        Formulas = 2,

        [Description("Acronyms and Formulas")]
        All = 255
    }
}
