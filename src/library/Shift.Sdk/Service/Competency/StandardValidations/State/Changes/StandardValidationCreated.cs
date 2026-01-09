using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardValidationCreated : Change
    {
        public Guid StandardId { get; }
        public Guid UserId { get; }

        public StandardValidationCreated(Guid standardId, Guid userId)
        {
            StandardId = standardId;
            UserId = userId;
        }
    }
}
