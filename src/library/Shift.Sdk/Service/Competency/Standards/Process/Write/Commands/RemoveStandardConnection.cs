using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Standards.Write
{
    public class RemoveStandardConnection : Command
    {
        public Guid[] ToStandardIds { get; set; }

        public RemoveStandardConnection(Guid standardId, Guid toStandardId)
            : this(standardId, new[] { toStandardId })
        {
        }

        public RemoveStandardConnection(Guid standardId, Guid[] toStandardIds)
        {
            AggregateIdentifier = standardId;
            ToStandardIds = toStandardIds;
        }

    }
}
