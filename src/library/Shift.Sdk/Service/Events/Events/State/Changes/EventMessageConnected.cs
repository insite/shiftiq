using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Events
{
    public class EventMessageConnected : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public EventMessageType MessageType { get; set; }

        public Guid? MessageId { get; set; }

        public EventMessageConnected(EventMessageType messageType, Guid? messageId)
        {
            MessageType = messageType;
            MessageId = messageId;
        }
    }
}
