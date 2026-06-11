using System;

namespace Shift.Contract
{
    public partial class MailoutMatch
    {
        public Guid MailoutId { get; set; }
        public Guid? MessageId { get; set; }
        public string MessageName { get; set; }
        public string MailoutStatus { get; set; }
        public string ContentSubject { get; set; }
        public DateTimeOffset MailoutScheduled { get; set; }
    }
}