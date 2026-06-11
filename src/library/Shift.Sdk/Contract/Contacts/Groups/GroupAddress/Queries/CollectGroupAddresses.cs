using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectGroupAddresses : Query<IEnumerable<GroupAddressModel>>, IGroupAddressCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
