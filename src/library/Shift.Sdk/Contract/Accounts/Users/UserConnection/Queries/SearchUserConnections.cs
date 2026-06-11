using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchUserConnections : Query<IEnumerable<UserConnectionMatch>>, IUserConnectionCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? ConnectedSince { get; set; }
        public DateTimeOffset? ConnectedBefore { get; set; }
    }
}
