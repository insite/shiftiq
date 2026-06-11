using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class UnfollowSubscriber : Command
    {
        public Guid ContactIdentifier { get; set; }
        public Guid FollowerIdentifier { get; set; }

        public UnfollowSubscriber(Guid id, Guid contact, Guid follower)
        {
            AggregateIdentifier = id;
            ContactIdentifier = contact;
            FollowerIdentifier = follower;
        }
    }
}