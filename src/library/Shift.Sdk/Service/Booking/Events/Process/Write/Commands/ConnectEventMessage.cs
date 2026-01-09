using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Events;

namespace InSite.Application.Events.Write
{
    public class ConnectEventMessage : Command
    {
        public EventMessageType MessageType { get; set; }
        public Guid? MessageId { get; set; }

        public ConnectEventMessage(Guid eventId, EventMessageType messageType, Guid? messageId)
        {
            AggregateIdentifier = eventId;
            MessageType = messageType;
            MessageId = messageId;
        }
    }
}
