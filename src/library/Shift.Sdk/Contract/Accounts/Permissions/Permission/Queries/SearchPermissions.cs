using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchPermissions : Query<IEnumerable<PermissionMatch>>, IPermissionCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? PermissionGrantedSince { get; set; }
        public DateTimeOffset? PermissionGrantedBefore { get; set; }
    }
}
