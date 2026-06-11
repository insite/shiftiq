using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchCaseGroups : Query<IEnumerable<CaseGroupMatch>>, ICaseGroupCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
