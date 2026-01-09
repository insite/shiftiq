using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Standards.Write
{
    public class ModifyStandardTimestamps : Command
    {
        public DateTimeOffset Created { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }

        public ModifyStandardTimestamps(Guid standardId, DateTimeOffset created, Guid createdBy, DateTimeOffset modified, Guid modifiedBy)
        {
            AggregateIdentifier = standardId;
            Created = created;
            CreatedBy = createdBy;
            Modified = modified;
            ModifiedBy = modifiedBy;
        }
    }
}
