using System;

namespace InSite.Persistence
{
    public class TGroupPermissionSearchResult
    {
        public Guid PermissionIdentifier { get; set; }

        public string GroupName { get; set; }
        public Guid GroupIdentifier { get; internal set; }
        public Guid OrganizationIdentifier { get; set; }
        public string OrganizationCode { get; set; }
        public string GroupType { get; set; }

        public string ObjectName { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public string ObjectType { get; set; }
        public string ObjectSubtype { get; set; }

        public bool Allow { get; set; }
        
        public bool AllowExecute { get; set; }
        public bool AllowRead { get; set; }
        public bool AllowWrite { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowAdministrate { get; set; }
        public bool AllowConfigure { get; set; }
        public bool AllowTrialAccess { get; set; }
    }
}
