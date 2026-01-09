using System;

namespace InSite.Application.Standards.Read
{
    public class QStandardGroup
    {
        public Guid StandardIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public DateTimeOffset Assigned { get; set; }
    }
}
