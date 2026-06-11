using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Sites.Write
{
    public class DeleteSite : Command
    {
        public DeleteSite(Guid site)
        {
            AggregateIdentifier = site;
        }
    }
}
