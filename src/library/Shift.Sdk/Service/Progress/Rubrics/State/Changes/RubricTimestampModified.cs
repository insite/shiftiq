using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricTimestampModified : Change
    {
        public DateTimeOffset Created { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }

        public RubricTimestampModified(DateTimeOffset created, Guid createdBy, DateTimeOffset modified, Guid modifiedBy)
        {
            Created = created;
            CreatedBy = createdBy;
            Modified = modified;
            ModifiedBy = modifiedBy;
        }
    }
}
