using System;

using InSite.Domain.Messages;

namespace InSite.Persistence
{
    [Serializable]
    public class TriggerModel
    {
        public string MessagePriority { get; set; }
        public NotificationType MessageName { get; set; }
        public string MessageTitle { get; set; }
        public DateTimeOffset? Notified { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string TextContent { get; set; }

        public int FromNumber { get; set; }
        public int ToNumber { get; set; }
        public string Condition { get; set; }
        public string ConditionRange { get; set; }
    }
}
