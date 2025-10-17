using System;

namespace InSite.Application.Messages.Read
{
    public class QSubscriberUser
    {
        public Guid MessageIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public DateTimeOffset Subscribed { get; set; }

        public virtual QMessage Message { get; set; }
    }
}
