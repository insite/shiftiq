using System.ComponentModel;

namespace Shift.Constant
{
    public enum AssetCriticality
    {
        [Description("1. Not Critical (no risk)")]
        NotCritical = 1,

        [Description("2. Somewhat Critical (minimal risk)")]
        SomewhatCritical = 2,

        [Description("3. Critical (moderate risk)")]
        Critical = 3,

        [Description("4. Very Critical (high probability risk)")]
        VeryCritical = 4,

        [Description("5. Extremely Critical (severe risk)")]
        ExtremelyCritical = 5,
    }
}
