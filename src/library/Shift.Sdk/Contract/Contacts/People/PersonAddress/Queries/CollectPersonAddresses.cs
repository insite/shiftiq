using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectPersonAddresses : Query<IEnumerable<PersonAddressModel>>, IPersonAddressCriteria
    {
    }
}
