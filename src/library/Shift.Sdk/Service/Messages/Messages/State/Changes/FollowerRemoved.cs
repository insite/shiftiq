using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class FollowerRemoved : Change
    {
        public Guid ContactIdentifier { get; set; }
        public Guid FollowerIdentifier { get; set; }

        public FollowerRemoved(Guid contact, Guid follower)
        {
            ContactIdentifier = contact;
            FollowerIdentifier = follower;
        }
    }
}
