using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class SubscribersRemoved : Change
    {
        public Guid[] ContactIdentifiers { get; set; }
        public bool IsGroup { get; set; }

        public SubscribersRemoved(Guid[] contacts, bool isGroup)
        {
            ContactIdentifiers = contacts;
            IsGroup = isGroup;
        }
    }
}