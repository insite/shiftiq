using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Standards.Write
{
    public class RemoveStandardCategory : Command
    {
        public Guid[] CategoryIds { get; set; }

        public RemoveStandardCategory(Guid standardId, Guid categoryId)
            : this(standardId, new[] { categoryId })
        {
        }

        public RemoveStandardCategory(Guid standardId, Guid[] categoryIds)
        {
            AggregateIdentifier = standardId;
            CategoryIds = categoryIds;
        }

    }
}
