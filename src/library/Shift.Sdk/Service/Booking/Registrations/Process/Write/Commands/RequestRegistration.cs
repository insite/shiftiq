using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class RequestRegistration : Command
    {
        public Guid Tenant { get; set; }
        public Guid Event { get; set; }
        public Guid Candidate { get; set; }
        public string AttendanceStatus { get; set; } // Old RegistrationStatus -> GradingStatus
        public string ApprovalStatus { get; set; }   // Old CandidateStatus    -> Administration
        public decimal? Fee { get; set; }
        public string Comment { get; set; }
        public string Source { get; set; }

        public RequestRegistration(Guid aggregate, Guid tenant, Guid @event, Guid candidate, string attendanceStatus, string approvalStatus, decimal? fee, string comment, string source)
        {
            AggregateIdentifier = aggregate;

            Tenant = tenant;
            Event = @event;
            Candidate = candidate;
            AttendanceStatus = attendanceStatus;
            ApprovalStatus = approvalStatus;
            Fee = fee;
            Comment = comment;
            Source = source;
        }
    }
}
