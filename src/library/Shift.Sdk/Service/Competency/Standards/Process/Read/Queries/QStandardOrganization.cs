using System;

namespace InSite.Application.Standards.Read
{
    public class QStandardOrganization
    {
        public Guid StandardIdentifier { get; set; }
        public Guid ConnectedOrganizationIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
    }
}
