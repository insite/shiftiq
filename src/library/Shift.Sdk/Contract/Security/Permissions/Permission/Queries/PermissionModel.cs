using System;

namespace Shift.Contract
{
    public partial class PermissionModel
    {
        public Guid GroupId { get; set; }
        public Guid ObjectId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? PermissionGrantedBy { get; set; }
        public Guid PermissionId { get; set; }
        public bool AllowAdministrate { get; set; }
        public bool AllowConfigure { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowRead { get; set; }
        public bool AllowTrialAccess { get; set; }
        public bool AllowWrite { get; set; }
        public string ObjectType { get; set; }
        public DateTimeOffset? PermissionGranted { get; set; }

        public string GroupName { get; set; }
        public string OrganizationCode { get; set; }
    }
}