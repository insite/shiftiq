using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchCaseUsers : Query<IEnumerable<CaseUserMatch>>, ICaseUserCriteria
    {
        public Guid? CaseId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        public string CaseRole { get; set; }
    }
}