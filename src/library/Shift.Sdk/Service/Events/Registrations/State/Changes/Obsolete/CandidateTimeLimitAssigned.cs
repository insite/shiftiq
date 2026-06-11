using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class CandidateTimeLimitAssigned : Change
    {
        public int? CandidateTimeLimit { get; set; }

        public CandidateTimeLimitAssigned(int? candidateTimeLimit)
        {
            CandidateTimeLimit = candidateTimeLimit;
        }
    }
}
