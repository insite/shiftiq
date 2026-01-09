
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class CandidateTypeChanged : Change
    {
        public string Type { get; set; }

        public CandidateTypeChanged(string type)
        {
            Type = type;
        }
    }
}