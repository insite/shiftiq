using System;

namespace InSite.Application.Messages.Read
{
    public class QFollowerFilter
    {
        public Guid? FollowerIdentifier { get; set; }
        public Guid? MessageIdentifier { get; set; }
        public Guid? SubscriberIdentifier { get; set; }
    }
}
