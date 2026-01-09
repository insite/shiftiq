using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPermissionCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? GroupIdentifier { get; set; }

        Guid? ObjectIdentifier { get; set; }

        Guid? OrganizationIdentifier { get; set; }

        Guid? PermissionGrantedBy { get; set; }

        bool? AllowAdministrate { get; set; }

        bool? AllowConfigure { get; set; }

        bool? AllowCreate { get; set; }

        bool? AllowDelete { get; set; }

        bool? AllowExecute { get; set; }

        bool? AllowRead { get; set; }

        bool? AllowTrialAccess { get; set; }

        bool? AllowWrite { get; set; }

        string ObjectType { get; set; }

        int? PermissionMask { get; set; }

        DateTimeOffset? PermissionGranted { get; set; }
    }
}