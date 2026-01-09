using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchPermissions : Query<IEnumerable<PermissionMatch>>, IPermissionCriteria
    {
        public Guid? GroupIdentifier { get; set; }
        public Guid? ObjectIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? PermissionGrantedBy { get; set; }
        public bool? AllowAdministrate { get; set; }
        public bool? AllowConfigure { get; set; }
        public bool? AllowCreate { get; set; }
        public bool? AllowDelete { get; set; }
        public bool? AllowExecute { get; set; }
        public bool? AllowRead { get; set; }
        public bool? AllowTrialAccess { get; set; }
        public bool? AllowWrite { get; set; }
        public string ObjectType { get; set; }
        public int? PermissionMask { get; set; }
        public DateTimeOffset? PermissionGranted { get; set; }
    }
}