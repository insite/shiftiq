using System.ComponentModel;

namespace Shift.Constant
{
    public enum AssetDifficulty
    {
        [Description("1. No difficulty")]
        Lowest = 1,

        [Description("2. Minimal difficulty")]
        Minimal = 2,

        [Description("3. Moderate difficulty")]
        Moderate = 3,

        [Description("4. Very hard or challenging")]
        VeryHard = 4,

        [Description("5. Intense, highly difficult")]
        Intense = 5,
    }
}
