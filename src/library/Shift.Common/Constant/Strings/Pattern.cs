namespace Shift.Constant
{
    public class Pattern
    {
        public const string ValidDomain = @"^\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public const string ValidEmail = @"^\s*\w[a-zA-Z0-9_+.'=-]*@\w+(?:[-.]\w+)*\.\w+(?:[-.]\w+)*\s*$";
        public const string ValidEmailLike = "%_@__%.__%";
    }
}
