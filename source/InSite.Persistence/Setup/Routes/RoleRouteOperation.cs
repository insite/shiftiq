using System;

namespace InSite.Persistence
{
    public class RoleRouteOperation
    {
        public Guid OrganizationIdentifier { get; set; }
        public string OrganizationCode { get; set; }

        public string RoleName { get; set; }
        public string RouteUrl { get; set; }

        public bool AllowExecute { get; set; }
        public bool AllowRead { get; set; }
        public bool AllowWrite { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowAdministrate { get; set; }
        public bool AllowConfigure { get; set; }
    }
}