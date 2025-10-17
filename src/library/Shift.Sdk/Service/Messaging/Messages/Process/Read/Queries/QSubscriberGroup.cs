using System;

namespace InSite.Application.Messages.Read
{
    public class QSubscriberGroup
    {
        public Guid GroupIdentifier { get; set; }
        public Guid MessageIdentifier { get; set; }
        public DateTimeOffset Subscribed { get; set; }

        public virtual QMessage Message { get; set; }
    }
}
