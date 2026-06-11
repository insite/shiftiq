using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchCaseUsers : Query<IEnumerable<CaseUserMatch>>, ICaseUserCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
