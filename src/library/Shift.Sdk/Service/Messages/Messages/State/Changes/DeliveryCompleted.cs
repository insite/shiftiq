using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Messages
{
    [Obsolete]
    public class DeliveryCompleted : Change
    {
        public Guid MailoutIdentifier { get; set; }
        public EmailAddress Recipient { get; set; }
        public string Error { get; set; }

        public DeliveryCompleted(Guid mailout, EmailAddress recipient, string error)
        {
            MailoutIdentifier = mailout;
            Recipient = recipient;
            Error = error;
        }
    }

    public class DeliveryCompleted2 : Change
    {
        public Guid MailoutIdentifier { get; set; }
        public Guid RecipientIdentifier { get; set; }
        public string Error { get; set; }

        public DeliveryCompleted2(Guid mailout, Guid recipient, string error)
        {
            MailoutIdentifier = mailout;
            RecipientIdentifier = recipient;
            Error = error;
        }
    }
}
