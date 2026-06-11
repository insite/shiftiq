using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class AddSubscribers : Command
    {
        public List<Guid> Contacts { get; set; }
        public string ContactRole { get; set; }
        public bool IsGroup { get; set; }

        public AddSubscribers(Guid message, List<Guid> contacts, string role, bool isGroup)
        {
            AggregateIdentifier = message;
            Contacts = contacts;
            ContactRole = role;
            IsGroup = isGroup;
        }
    }
}