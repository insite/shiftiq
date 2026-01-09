using System;

namespace Shift.Contract
{
    public partial class FileActivityModel
    {
        public Guid ActivityIdentifier { get; set; }
        public Guid FileIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string ActivityChanges { get; set; }

        public DateTimeOffset ActivityTime { get; set; }
    }
}