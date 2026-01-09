using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Events
{
    public class EventMessageSent : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public EventMessageType MessageType { get; set; }

        public Guid MessageId { get; set; }
        public Guid[] Recipients { get; set; }

        public EventMessageSent(EventMessageType messageType, Guid messageId, Guid[] recipients)
        {
            MessageType = messageType;
            MessageId = messageId;
            Recipients = recipients;
        }
    }
}
