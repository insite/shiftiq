using Shift.Common;

namespace Shift.Contract
{
    public interface IUserCriteria
    {
        QueryFilter Filter { get; set; }

        string UserEmailExact { get; set; }
        string UserFullNameContains { get; set; }
    }
}