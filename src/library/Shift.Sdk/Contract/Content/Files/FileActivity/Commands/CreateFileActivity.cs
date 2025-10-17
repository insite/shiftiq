using System;

namespace Shift.Contract
{
    public class CreateFileActivity
    {
        public Guid ActivityIdentifier { get; set; }
        public Guid FileIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string ActivityChanges { get; set; }

        public DateTimeOffset ActivityTime { get; set; }
    }
}