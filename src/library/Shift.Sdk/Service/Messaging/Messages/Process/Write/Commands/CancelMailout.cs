using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class CancelMailout : Command
    {
        public CancelMailout(Guid message, Guid mailout)
        {
            AggregateIdentifier = message;
            MailoutIdentifier = mailout;
        }

        public Guid MailoutIdentifier { get; set; }
    }
}