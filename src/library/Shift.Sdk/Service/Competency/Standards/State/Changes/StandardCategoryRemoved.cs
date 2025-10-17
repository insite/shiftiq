using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardCategoryRemoved : Change
    {
        public Guid[] CategoryIds { get; }

        public StandardCategoryRemoved(Guid[] categoryIds)
        {
            CategoryIds = categoryIds;
        }
    }
}
