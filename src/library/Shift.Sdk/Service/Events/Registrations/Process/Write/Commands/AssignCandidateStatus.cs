using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignCandidateStatus : Command
    {
        public string Status { get; set; }
        public string Indicator { get; set; }
        public string[] Errors { get; set; }

        public AssignCandidateStatus(Guid aggregate, string status, string indicator, string[] errors)
        {
            AggregateIdentifier = aggregate;
            Status = status;
            Indicator = indicator;
            Errors = errors;
        }
    }
}
