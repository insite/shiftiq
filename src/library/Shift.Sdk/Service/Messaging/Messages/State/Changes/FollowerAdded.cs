using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class FollowerAdded : Change
    {
        public Guid ContactIdentifier { get; set; }
        public Guid FollowerIdentifier { get; set; }

        public FollowerAdded(Guid contact, Guid follower)
        {
            ContactIdentifier = contact;
            FollowerIdentifier = follower;
        }
    }
}
