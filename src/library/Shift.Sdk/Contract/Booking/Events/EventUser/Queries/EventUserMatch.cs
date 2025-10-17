using System;

namespace Shift.Contract
{
    public partial class EventUserMatch
    {
        public Guid EventId { get; set; }
        public string EventTitle { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public DateTimeOffset? RelationCreated { get; set; }
        public string RelationRole { get; set; }
    }
}