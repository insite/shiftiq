using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectUserMocks : Query<IEnumerable<UserMockModel>>, IUserMockCriteria
    {
    }
}
