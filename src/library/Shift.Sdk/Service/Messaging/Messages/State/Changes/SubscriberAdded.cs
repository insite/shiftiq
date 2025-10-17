﻿using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class SubscriberAdded : Change
    {
        public Guid ContactIdentifier { get; set; }
        public string ContactRole { get; set; }
        public bool IsGroup { get; set; }

        public SubscriberAdded(Guid contact, string role, bool isGroup)
        {
            ContactIdentifier = contact;
            ContactRole = role;
            IsGroup = isGroup;
        }
    }
}
