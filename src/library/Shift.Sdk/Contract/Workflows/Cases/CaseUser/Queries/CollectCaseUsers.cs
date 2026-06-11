using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCaseUsers : Query<IEnumerable<CaseUserModel>>, ICaseUserCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
