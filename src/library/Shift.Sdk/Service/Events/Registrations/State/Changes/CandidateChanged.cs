using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class CandidateChanged : Change
    {
        public Guid Candidate { get; set; }

        public CandidateChanged(Guid candidate)
        {
            Candidate = candidate;
        }
    }
}
