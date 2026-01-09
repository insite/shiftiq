namespace Shift.Common
{
    public interface ISystemRoles
    {
        bool IsAdministrator { get; set; }
        bool IsDeveloper { get; set; }
        bool IsLearner { get; set; }
        bool IsOperator { get; set; }
    }
}
