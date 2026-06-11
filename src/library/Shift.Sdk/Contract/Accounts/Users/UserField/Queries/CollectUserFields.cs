using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectUserFields : Query<IEnumerable<UserFieldModel>>, IUserFieldCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
