using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class ExpirationReminderDelivered2 : Change
    {
        public ExpirationReminderDelivered2(ReminderType type, DateTimeOffset? delivered)
        {
            Type = type;
            Delivered = delivered;
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ReminderType Type { get; set; }

        public DateTimeOffset? Delivered { get; set; }
    }
}