using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Events;

namespace InSite.Application.Events.Write
{
    public class SendEventMessage : Command
    {
        public EventMessageType MessageType { get; set; }
        public Guid MessageId { get; set; }
        public Guid[] Recipients { get; set; }

        public SendEventMessage(Guid eventId, EventMessageType messageType, Guid messageId, Guid[] recipients)
        {
            AggregateIdentifier = eventId;
            MessageId = messageId;
            Recipients = recipients;
        }
    }
}
