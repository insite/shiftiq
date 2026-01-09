using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectUsers : Query<IEnumerable<UserModel>>, IUserCriteria
    {
        public string UserEmailExact { get; set; }
        public string UserFullNameContains { get; set; }
    }
}