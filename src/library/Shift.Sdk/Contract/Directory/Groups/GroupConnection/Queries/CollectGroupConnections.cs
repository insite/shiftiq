using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectGroupConnections : Query<IEnumerable<GroupConnectionModel>>, IGroupConnectionCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}