using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPermissionCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationId { get; set; }

        DateTimeOffset? PermissionGrantedSince { get; set; }
        DateTimeOffset? PermissionGrantedBefore { get; set; }
    }
}
