
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class CandidateStatusAssigned : Change
    {
        public string Status { get; set; }
        public string Indicator { get; set; }
        public string[] Errors { get; set; }

        public CandidateStatusAssigned(string status, string indicator, string[] errors)
        {
            Status = status;
            Indicator = indicator;
            Errors = errors;
        }
    }
}
