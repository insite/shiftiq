using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class RemoveMessageSubscriber : Command
    {
        public Guid ContactIdentifier { get; set; }
        public bool IsGroup { get; set; }

        public RemoveMessageSubscriber(Guid message, Guid contact, bool isGroup)
        {
            AggregateIdentifier = message;
            ContactIdentifier = contact;
            IsGroup = isGroup;
        }
    }
}
