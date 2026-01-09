using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchGroupConnections : Query<IEnumerable<GroupConnectionMatch>>, IGroupConnectionCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}