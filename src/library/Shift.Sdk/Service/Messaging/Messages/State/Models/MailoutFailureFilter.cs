using System;

using Shift.Common;

namespace InSite.Domain.Messages
{
    [Serializable]
    public class MailoutFailureFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? MessageIdentifier { get; set; }

        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string MessageType { get; set; }

        public DateTimeOffsetRange Scheduled { get; set; }
        public DateTimeOffsetRange Failed { get; set; }
    }
}
