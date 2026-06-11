using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class CompleteDelivery : Command
    {
        public CompleteDelivery(Guid message, Guid mailout, Guid recipient, string error)
        {
            AggregateIdentifier = message;
            MailoutIdentifier = mailout;
            RecipientIdentifier = recipient;

            if (error != "OK")
                Error = error;
        }

        public Guid MailoutIdentifier { get; set; }
        public Guid RecipientIdentifier { get; set; }
        public string Error { get; set; }
    }

    public class CompleteCarbonCopy : Command
    {
        public CompleteCarbonCopy(Guid message, Guid mailout, Guid recipient, string ccType, Guid cc, string error)
        {
            AggregateIdentifier = message;
            MailoutIdentifier = mailout;
            RecipientIdentifier = recipient;
            CcType = ccType;
            CcIdentifier = cc;

            if (error != "OK")
                Error = error;
        }

        public Guid MailoutIdentifier { get; set; }
        public Guid RecipientIdentifier { get; set; }
        public string CcType { get; set; }
        public Guid CcIdentifier { get; set; }
        public string Error { get; set; }
    }
}