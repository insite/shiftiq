using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardTimestampsModified : Change
    {
        public DateTimeOffset Created { get; }
        public Guid CreatedBy { get; }
        public DateTimeOffset Modified { get; }
        public Guid ModifiedBy { get; }

        public StandardTimestampsModified(DateTimeOffset created, Guid createdBy, DateTimeOffset modified, Guid modifiedBy)
        {
            Created = created;
            CreatedBy = createdBy;
            Modified = modified;
            ModifiedBy = modifiedBy;
        }
    }
}
