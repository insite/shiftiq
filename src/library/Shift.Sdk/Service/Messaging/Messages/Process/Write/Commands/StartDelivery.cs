using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Messages.Write
{
    public class StartDelivery : Command
    {
        public StartDelivery(Guid message, Guid mailout, Guid recipient)
        {
            AggregateIdentifier = message;
            MailoutIdentifier = mailout;
            RecipientIdentifier = recipient;
        }

        public Guid MailoutIdentifier { get; set; }
        public Guid RecipientIdentifier { get; set; }
    }

    public class StartCarbonCopy : Command
    {
        public StartCarbonCopy(Guid message, Guid mailout, Guid recipient, string ccType, Guid cc)
        {
            AggregateIdentifier = message;
            MailoutIdentifier = mailout;
            RecipientIdentifier = recipient;
            CcType = ccType;
            CcIdentifier = cc;
        }

        public Guid MailoutIdentifier { get; set; }
        public Guid RecipientIdentifier { get; set; }
        public string CcType { get; set; }
        public Guid CcIdentifier { get; set; }
    }

    public class StartDeliveries : Command
    {
        public StartDeliveries(Guid message, Guid mailout, EmailAddress[] recipients)
        {
            AggregateIdentifier = message;
            MailoutIdentifier = mailout;
            Recipients = recipients;
        }

        public Guid MailoutIdentifier { get; set; }
        public EmailAddress[] Recipients { get; set; }
    }
}