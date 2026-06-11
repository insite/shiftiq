using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class ResetLinkCounter : Command
    {
        public Guid LinkIdentifier { get; set; }
        
        public ResetLinkCounter(Guid messageIdentifier, Guid linkIdentifier)
        {
            AggregateIdentifier = messageIdentifier;
            LinkIdentifier = linkIdentifier;
        }
    }
}