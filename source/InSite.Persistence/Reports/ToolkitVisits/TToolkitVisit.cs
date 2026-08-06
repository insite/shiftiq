using System;

namespace InSite.Persistence
{
    public class TToolkitVisit
    {
        public Guid ToolkitVisitIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid TokenIdentifier { get; set; }
        public Guid ActionIdentifier { get; set; }
        public DateTimeOffset Visited { get; set; }
    }
}
