using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardContainmentModified : Change
    {
        public StandardContainment[] Containments { get; set; }

        public StandardContainmentModified(StandardContainment[] containments)
        {
            Containments = containments;
        }
    }
}
