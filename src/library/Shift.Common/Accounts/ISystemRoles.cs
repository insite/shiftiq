namespace Shift.Common
{
    public interface ISystemRoles
    {
        bool IsAdministrator { get; }
        bool IsDeveloper { get; }
        bool IsLearner { get; }
        bool IsOperator { get; }
    }
}
