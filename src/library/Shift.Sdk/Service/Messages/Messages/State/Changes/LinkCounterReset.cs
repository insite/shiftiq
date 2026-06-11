using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class LinkCounterReset : Change
    {
        public Guid LinkIdentifier { get; set; }

        public LinkCounterReset(Guid linkIdentifier)
        {
            LinkIdentifier = linkIdentifier;
        }
    }
}