using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCaseGroups : Query<IEnumerable<CaseGroupModel>>, ICaseGroupCriteria
    {
        public Guid? GroupId { get; set; }
        public Guid? CaseId { get; set; }
        public Guid? OrganizationId { get; set; }

        public string CaseRole { get; set; }
    }
}