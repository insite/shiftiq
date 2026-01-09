using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchCaseGroups : Query<IEnumerable<CaseGroupMatch>>, ICaseGroupCriteria
    {
        public Guid? GroupIdentifier { get; set; }
        public Guid? CaseIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string CaseRole { get; set; }
    }
}