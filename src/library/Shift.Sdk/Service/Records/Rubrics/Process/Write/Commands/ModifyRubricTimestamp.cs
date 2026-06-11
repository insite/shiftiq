using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Rubrics.Write
{
    public class ModifyRubricTimestamp : Command, IHasRun
    {
        public DateTimeOffset Created { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }

        public ModifyRubricTimestamp(Guid rubricId, DateTimeOffset created, Guid createdBy, DateTimeOffset modified, Guid modifiedBy)
        {
            AggregateIdentifier = rubricId;
            Created = created;
            CreatedBy = createdBy;
            Modified = modified;
            ModifiedBy = modifiedBy;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            aggregate.Apply(new RubricTimestampModified(Created, CreatedBy, Modified, ModifiedBy));

            return true;
        }
    }
}
