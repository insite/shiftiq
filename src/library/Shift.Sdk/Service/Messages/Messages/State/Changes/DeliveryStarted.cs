using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Messages
{
    [Obsolete]
    public class DeliveryStarted : Change
    {
        public Guid MailoutIdentifier { get; set; }
        public EmailAddress Recipient { get; set; }

        public DeliveryStarted(Guid mailout, EmailAddress recipient)
        {
            MailoutIdentifier = mailout;
            Recipient = recipient;
        }
    }

    public class DeliveryStarted2 : Change
    {
        public Guid MailoutIdentifier { get; set; }
        public Guid RecipientIdentifier { get; set; }

        public DeliveryStarted2(Guid mailout, Guid recipient)
        {
            MailoutIdentifier = mailout;
            RecipientIdentifier = recipient;
        }
    }
}