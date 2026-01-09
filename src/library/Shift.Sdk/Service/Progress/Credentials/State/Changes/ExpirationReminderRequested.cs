using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Records
{
    [Obsolete]
    public class ExpirationReminderRequested : Change
    {
        public ExpirationReminderRequested(ReminderType type, Guid user, DateTimeOffset requested)
        {
            Type = type;
            User = user;
            Requested = requested;
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ReminderType Type { get; set; }

        public Guid User { get; set; }
        public DateTimeOffset Requested { get; set; }
    }

    public class ExpirationReminderRequested2 : Change
    {
        public ExpirationReminderRequested2(ReminderType type, DateTimeOffset requested)
        {
            Type = type;
            Requested = requested;
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ReminderType Type { get; set; }

        public DateTimeOffset Requested { get; set; }
    }
}