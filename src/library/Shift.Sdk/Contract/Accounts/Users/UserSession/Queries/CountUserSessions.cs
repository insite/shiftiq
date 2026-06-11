using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountUserSessions : Query<int>, IUserSessionCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? SessionStartedSince { get; set; }
        public DateTimeOffset? SessionStartedBefore { get; set; }
        public DateTimeOffset? SessionStoppedSince { get; set; }
        public DateTimeOffset? SessionStoppedBefore { get; set; }
    }
}
