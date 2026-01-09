using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCaseUsers : Query<IEnumerable<CaseUserModel>>, ICaseUserCriteria
    {
        public Guid? CaseIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string CaseRole { get; set; }
    }
}