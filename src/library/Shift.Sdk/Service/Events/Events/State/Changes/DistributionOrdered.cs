using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class DistributionOrdered : Change
    {
        public string Code { get; set; }
        public string Status { get; set; }
        public string Errors { get; set; }

        public DistributionOrdered(string code, string status, string errors)
        {
            Code = code;
            Status = status;
            Errors = errors;
        }
    }
}
