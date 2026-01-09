using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SetImported : Change
    {
        public Set Set { get; set; }

        public SetImported(Set set)
        {
            Set = set;
        }
    }
}
