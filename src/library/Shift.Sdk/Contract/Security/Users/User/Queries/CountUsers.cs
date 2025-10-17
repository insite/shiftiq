using Shift.Common;

namespace Shift.Contract
{
    public class CountUsers : Query<int>, IUserCriteria
    {
        public string UserEmailExact { get; set; }
        public string UserFullNameContains { get; set; }
    }
}