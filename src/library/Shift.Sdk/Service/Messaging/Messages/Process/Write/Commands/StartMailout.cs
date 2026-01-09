using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class StartMailout : Command
    {
        public StartMailout(Guid message, Guid mailout)
        {
            AggregateIdentifier = message;
            MailoutIdentifier = mailout;
        }

        public Guid MailoutIdentifier { get; set; }
    }
}