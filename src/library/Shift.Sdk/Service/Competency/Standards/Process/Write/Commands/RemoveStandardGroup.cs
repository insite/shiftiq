using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Standards.Write
{
    public class RemoveStandardGroup : Command
    {
        public Guid[] GroupIds { get; set; }

        public RemoveStandardGroup(Guid standardId, Guid groupId)
            : this(standardId, new[] { groupId })
        {
        }

        public RemoveStandardGroup(Guid standardId, Guid[] groupIds)
        {
            AggregateIdentifier = standardId;
            GroupIds = groupIds;
        }

    }
}
