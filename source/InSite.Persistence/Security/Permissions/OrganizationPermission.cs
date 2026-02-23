using System;

namespace InSite.Persistence
{
    public class OrganizationPermission
    {
        public Guid OrganizationId { get; set; }
        public string AccessGranted { get; set; }
        public string AccessDenied { get; set; }
        public string AccessGrantedToActions { get; set; }
    }
}
