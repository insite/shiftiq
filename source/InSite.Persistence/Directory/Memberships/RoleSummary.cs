using System;

namespace InSite.Persistence
{
    public class RoleSummary
    {
        public Guid GroupIdentifier { get; set; }
        public String GroupName { get; set; }
        public String GroupTenantCode { get; set; }
        public Guid GroupTenantIdentifier { get; set; }
        public String GroupTenantName { get; set; }
        public String GroupTenantTitle { get; set; }
        public String GroupType { get; set; }
        public DateTimeOffset RoleAssigned { get; set; }
        public String RoleName { get; set; }
        public String UserEmail { get; set; }
        public String UserFirstName { get; set; }
        public String UserFullName { get; set; }
        public Int32 UserHasPassword { get; set; }
        public Boolean? UserIsArchived { get; set; }
        public Guid UserIdentifier { get; set; }
        public String UserLastName { get; set; }
    }
}
