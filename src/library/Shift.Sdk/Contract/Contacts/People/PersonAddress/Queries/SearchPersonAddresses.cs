using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchPersonAddresses : Query<IEnumerable<PersonAddressMatch>>, IPersonAddressCriteria
    {
    }
}
