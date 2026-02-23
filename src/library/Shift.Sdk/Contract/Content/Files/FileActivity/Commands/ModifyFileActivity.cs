using System;

namespace Shift.Contract
{
    public class ModifyFileActivity
    {
        public Guid ActivityId { get; set; }
        public Guid FileId { get; set; }
        public Guid UserId { get; set; }

        public string ActivityChanges { get; set; }

        public DateTimeOffset ActivityTime { get; set; }
    }
}