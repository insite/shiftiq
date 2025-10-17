using System;

namespace InSite.Application.Contacts.Read
{
    public class TGroupPermission
    {
        public Guid GroupIdentifier { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? PermissionGrantedBy { get; set; }
        public Guid PermissionIdentifier { get; set; }

        public string ObjectType { get; set; }

        public bool AllowAdministrate { get; set; }
        public bool AllowConfigure { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowExecute { get; set; }
        public bool AllowRead { get; set; }
        public bool AllowWrite { get; set; }
        public bool AllowTrialAccess { get; set; }

        public int PermissionMask { get; set; }

        public DateTimeOffset? PermissionGranted { get; set; }

        public virtual QGroup Group { get; set; }
    }
}
