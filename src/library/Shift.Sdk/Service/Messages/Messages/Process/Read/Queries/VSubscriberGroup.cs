using System;

namespace InSite.Application.Messages.Read
{
    public class VSubscriberGroup
    {
        public Guid MessageIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public DateTimeOffset Subscribed { get; set; }

        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public int GroupSize { get; set; }

        public string MessageName { get; set; }
        public Guid MessageOrganizationIdentifier { get; set; }
    }
}
