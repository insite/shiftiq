using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class DeleteGroup : Command
    {
        public string Reason { get; }

        public DeleteGroup(Guid group, string reason)
        {
            AggregateIdentifier = group;
            Reason = reason;
        }
    }
}
