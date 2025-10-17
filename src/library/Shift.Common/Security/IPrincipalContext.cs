namespace Shift.Common
{
    public interface IPrincipalContext
    {
        IShiftPrincipal Current { get; }
    }
}