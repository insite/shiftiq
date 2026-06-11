using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchGroupFields : Query<IEnumerable<GroupFieldMatch>>, IGroupFieldCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
