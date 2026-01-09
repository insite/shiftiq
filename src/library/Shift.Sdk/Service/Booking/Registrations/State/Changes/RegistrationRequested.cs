using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationRequested : Change
    {
        public Guid Tenant { get; set; }
        public Guid Event { get; set; }
        public Guid Candidate { get; set; }
        public string AttendanceStatus { get; set; } // Old RegistrationStatus -> GradingStatus
        public string ApprovalStatus { get; set; } // Old CandidateStatus -> Administration
        public string Password { get; set; }
        public decimal? Fee { get; set; }
        public string Comment { get; set; }
        public string Source { get; set; }
        public int? Sequence { get; set; }

        public RegistrationRequested(Guid tenant, Guid @event, Guid candidate, string attendanceStatus, string approvalStatus, string password, decimal? fee, string comment, string source, int? sequence)
        {
            Tenant = tenant;
            Event = @event;
            Candidate = candidate;
            AttendanceStatus = attendanceStatus;
            ApprovalStatus = approvalStatus;
            Password = password;
            Fee = fee;
            Comment = comment;
            Source = source;
            Sequence = sequence;
        }
    }
}
