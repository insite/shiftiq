using System;

namespace InSite.Application.Messages.Read
{
    public class ArchivedFollower
    {
        public Guid FollowerIdentifier { get; set; }
        public Guid MessageIdentifier { get; set; }
        public Guid SubscriberIdentifier { get; set; }
    }
}
