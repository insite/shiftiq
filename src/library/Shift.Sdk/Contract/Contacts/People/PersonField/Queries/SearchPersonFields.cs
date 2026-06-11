using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchPersonFields : Query<IEnumerable<PersonFieldMatch>>, IPersonFieldCriteria
    {
    }
}
