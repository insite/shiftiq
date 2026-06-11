using System;

namespace InSite.Application.Messages.Read
{
    public class QFollower
    {
        public Guid JoinIdentifier { get; set; }

        public Guid FollowerIdentifier { get; set; }
        public Guid MessageIdentifier { get; set; }
        public Guid SubscriberIdentifier { get; set; }
    }

    public class VFollower
    {
        public Guid MessageIdentifier { get; set; }
        public Guid SubscriberIdentifier { get; set; }
        public Guid FollowerIdentifier { get; set; }
        public DateTimeOffset? Subscribed { get; set; }
        public String FollowerEmail { get; set; }
        public String FollowerFullName { get; set; }
        public string MessageTitle { get; set; }
        public String SubscriberEmail { get; set; }
        public String SubscriberName { get; set; }
    }
}
