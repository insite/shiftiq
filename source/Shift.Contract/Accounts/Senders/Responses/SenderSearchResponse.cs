using System;

namespace Shift.Contract.Responses
{
    public class SenderSearchResponse : Mailbox
    {
        public Guid Identifier { get; set; }

        public string Type { get; set; }

        public Mailbox Server { get; set; }

        public bool Enabled { get; set; }
    }
}