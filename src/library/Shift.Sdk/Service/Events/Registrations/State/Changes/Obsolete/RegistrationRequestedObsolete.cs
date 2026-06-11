using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationRequestedObsolete : Change
    {
        public Guid Activity { get; set; }
        public Guid Candidate { get; set; }
        public string Password { get; set; }

        public decimal? Fee { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string Source { get; set; }
        public int? Sequence { get; set; }
        public string CandidateStatus { get; set; }

        public RegistrationRequestedObsolete(Guid activity, Guid candidate, string password, decimal? fee, string status, string comment, string source, int? sequence, string candidateStatus)
        {
            Activity = activity;
            Candidate = candidate;
            Password = password;

            Fee = fee;
            Status = status;
            Comment = comment;
            Source = source;
            Sequence = sequence;
            CandidateStatus = candidateStatus;
        }
    }
}
