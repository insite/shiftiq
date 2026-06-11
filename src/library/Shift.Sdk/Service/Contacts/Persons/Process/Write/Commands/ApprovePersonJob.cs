using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.People.Write
{
    public class ApprovePersonJob : Command
    {
        public DateTimeOffset? Approved { get; set; }
        public string ApprovedBy { get; set; }

        public ApprovePersonJob(Guid personId, DateTimeOffset? approved, string approvedBy)
        {
            AggregateIdentifier = personId;
            Approved = approved;
            ApprovedBy = approvedBy;
        }
    }
}
