using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardContainmentAdded : Change
    {
        public StandardContainment[] Containments { get; set; }

        public StandardContainmentAdded(StandardContainment[] containments)
        {
            Containments = containments;
        }
    }
}
