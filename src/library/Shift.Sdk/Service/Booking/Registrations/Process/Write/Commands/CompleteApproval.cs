using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class CompleteApproval : Command
    {
        public string Status { get; set; }
        public string Errors { get; set; }

        public CompleteApproval(Guid aggregate, string status, string errors)
        {
            AggregateIdentifier = aggregate;
            Status = status;
            Errors = errors;
        }
    }
}
