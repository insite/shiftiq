using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectGroupFields : Query<IEnumerable<GroupFieldModel>>, IGroupFieldCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
