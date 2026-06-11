using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class SubscribersAdded : Change
    {
        public Guid[] ContactIdentifiers { get; set; }
        public string ContactRole { get; set; }
        public bool IsGroup { get; set; }

        public SubscribersAdded(Guid[] contacts, string role, bool isGroup)
        {
            ContactIdentifiers = contacts;
            ContactRole = role;
            IsGroup = isGroup;
        }
    }
}