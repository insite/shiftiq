using System;

namespace Shift.Common.Timeline.Changes
{
    public class AggregateRunContext
    {
        public Guid Organization { get; }
        public Guid User { get; }

        public AggregateRunContext(Guid organization, Guid user)
        {
            Organization = organization;
            User = user;
        }
    }
}
