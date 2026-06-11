using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class GradeWithheld : Change
    {
        public string Reason { get; set; }

        public GradeWithheld(string reason)
        {
            Reason = reason;
        }
    }
}
