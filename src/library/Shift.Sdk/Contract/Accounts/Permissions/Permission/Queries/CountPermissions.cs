using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountPermissions : Query<int>, IPermissionCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? PermissionGrantedSince { get; set; }
        public DateTimeOffset? PermissionGrantedBefore { get; set; }
    }
}
