using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Standards.Write
{
    public class RemoveStandardContainment : Command
    {
        public Guid[] ChildStandardIds { get; set; }

        public RemoveStandardContainment(Guid standardId, Guid childStandardId)
            : this(standardId, new[] { childStandardId })
        {
        }

        public RemoveStandardContainment(Guid standardId, Guid[] childStandardIds)
        {
            AggregateIdentifier = standardId;
            ChildStandardIds = childStandardIds;
        }

    }
}
