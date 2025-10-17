using System;

namespace Shift.Constant
{
    [Flags]
    public enum ResolvePath
    {
        None = 0,
        Internal = 1,
        Target = 2
    }
}