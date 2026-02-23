using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCaseGroups : Query<int>, ICaseGroupCriteria
    {
        public Guid? GroupId { get; set; }
        public Guid? CaseId { get; set; }
        public Guid? OrganizationId { get; set; }

        public string CaseRole { get; set; }
    }
}