using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.StandardValidations.Write
{
    public class ModifyStandardValidationTimestamps : Command
    {
        public DateTimeOffset Created { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTimeOffset Modified { get; private set; }
        public Guid ModifiedBy { get; private set; }

        public ModifyStandardValidationTimestamps(Guid standardValidationId, DateTimeOffset created, Guid createdBy, DateTimeOffset modified, Guid modifiedBy)
        {
            AggregateIdentifier = standardValidationId;
            Created = created;
            CreatedBy = createdBy;
            Modified = modified;
            ModifiedBy = modifiedBy;
        }
    }
}
