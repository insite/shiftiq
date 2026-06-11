using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectPersonFields : Query<IEnumerable<PersonFieldModel>>, IPersonFieldCriteria
    {
    }
}
