using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCaseGroups : Query<IEnumerable<CaseGroupModel>>, ICaseGroupCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
