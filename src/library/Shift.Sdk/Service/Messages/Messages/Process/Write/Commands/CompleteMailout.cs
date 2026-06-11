using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class CompleteMailout : Command
    {
        public CompleteMailout(Guid message, Guid mailout)
        {
            AggregateIdentifier = message;
            Mailout = mailout;
        }

        public Guid Mailout { get; set; }
    }
}