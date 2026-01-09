using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class AbortMailout : Command
    {
        public AbortMailout(Guid message, Guid mailout, string reason)
        {
            AggregateIdentifier = message;
            Mailout = mailout;
            Reason = reason;
        }

        public Guid Mailout { get; set; }
        public string Reason { get; set; }
    }
}