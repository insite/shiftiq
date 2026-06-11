using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchGroupAddresses : Query<IEnumerable<GroupAddressMatch>>, IGroupAddressCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
