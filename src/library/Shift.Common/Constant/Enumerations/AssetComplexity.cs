using System.ComponentModel;

namespace Shift.Constant
{
    public enum AssetComplexity
    {
        [Description("1. Recall")]
        Recall = 1,

        [Description("2. Understand")]
        Understand = 2,

        [Description("3. Apply")]
        Apply = 3,

        [Description("4. Analyze")]
        Analyze = 4,

        [Description("5. Evaluate")]
        Evaluate = 5,

        [Description("6. Create")]
        Create = 6,
    }
}
