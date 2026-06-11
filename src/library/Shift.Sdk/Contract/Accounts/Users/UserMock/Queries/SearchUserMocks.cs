using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchUserMocks : Query<IEnumerable<UserMockMatch>>, IUserMockCriteria
    {
    }
}
