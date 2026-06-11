using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectUserSessions : Query<IEnumerable<UserSessionModel>>, IUserSessionCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? SessionStartedSince { get; set; }
        public DateTimeOffset? SessionStartedBefore { get; set; }
        public DateTimeOffset? SessionStoppedSince { get; set; }
        public DateTimeOffset? SessionStoppedBefore { get; set; }
    }
}
