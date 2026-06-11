using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class History
    {
        public Guid RecordId { get; set; }
        public DateTimeOffset RecordTime { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
    }
}
