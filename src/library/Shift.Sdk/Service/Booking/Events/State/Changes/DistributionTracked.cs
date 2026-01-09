using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class DistributionTracked : Change
    {
        public string Job { get; set; }
        public string Status { get; set; }
        public string Errors { get; set; }

        public DistributionTracked(string job, string status, string errors)
        {
            Job = job;
            Status = status;
            Errors = errors;
        }
    }
}
