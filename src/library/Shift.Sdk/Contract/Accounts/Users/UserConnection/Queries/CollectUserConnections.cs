using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectUserConnections : Query<IEnumerable<UserConnectionModel>>, IUserConnectionCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? ConnectedSince { get; set; }
        public DateTimeOffset? ConnectedBefore { get; set; }
        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
