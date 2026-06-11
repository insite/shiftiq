using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountUserConnections : Query<int>, IUserConnectionCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? ConnectedSince { get; set; }
        public DateTimeOffset? ConnectedBefore { get; set; }
    }
}
