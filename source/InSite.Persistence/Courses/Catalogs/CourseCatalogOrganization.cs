using System;

namespace InSite.Persistence
{
    public class CourseCatalogOrganization
    {
        public Guid OrganizationIdentifier { get; set; }
        public string OrganizationName { get; set; }
        public bool IsSelected { get; set; }
        public int OrganizationSize { get; set; }
    }
}