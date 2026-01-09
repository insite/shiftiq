using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class RemoveMessageSubscribers : Command
    {
        public List<Guid> Contacts { get; set; }
        public bool IsGroup { get; set; }

        public RemoveMessageSubscribers(Guid message, List<Guid> contacts, bool isGroup)
        {
            AggregateIdentifier = message;
            Contacts = contacts;
            IsGroup = isGroup;
        }
    }
}