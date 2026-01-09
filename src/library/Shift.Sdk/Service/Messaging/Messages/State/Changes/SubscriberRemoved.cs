using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class SubscriberRemoved : Change
    {
        public Guid ContactIdentifier { get; set; }
        public string ContactRole { get; set; }
        public bool IsGroup { get; set; }

        public SubscriberRemoved(Guid contact, string role, bool isGroup)
        {
            ContactIdentifier = contact;
            ContactRole = role;
            IsGroup = isGroup;
        }
    }
}
