using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchUserFields : Query<IEnumerable<UserFieldMatch>>, IUserFieldCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
