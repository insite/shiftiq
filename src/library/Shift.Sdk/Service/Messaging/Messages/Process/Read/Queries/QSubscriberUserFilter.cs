using System;

using Shift.Common;

namespace InSite.Application.Messages.Read
{
    [Serializable]
    public class QSubscriberGroupFilter : Filter
    {
        public Guid MessageIdentifier { get; set; }
        public Guid? MessageOrganizationIdentifier { get; set; }
        public Guid? SubscriberIdentifier { get; set; }
        public Guid[] SubscriberIdentifiers { get; set; }

        public string MessageName { get; set; }
        public string SubscriberKeyword { get; set; }
    }

    [Serializable]
    public class QSubscriberUserFilter : Filter
    {
        public Guid MessageIdentifier { get; set; }
        public Guid? MessageOrganizationIdentifier { get; set; }
        public Guid? SubscriberIdentifier { get; set; }
        public Guid[] SubscriberIdentifiers { get; set; }

        public string MessageName { get; set; }
        public string SubscriberKeyword { get; set; }
    }
}
