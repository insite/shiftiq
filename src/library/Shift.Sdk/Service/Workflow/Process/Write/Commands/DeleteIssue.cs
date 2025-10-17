using System;

using Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class DeleteIssue : Command
    {
        public DeleteIssue(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
