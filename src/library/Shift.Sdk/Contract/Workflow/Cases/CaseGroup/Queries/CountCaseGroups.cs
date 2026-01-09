using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCaseGroups : Query<int>, ICaseGroupCriteria
    {
        public Guid? GroupIdentifier { get; set; }
        public Guid? CaseIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string CaseRole { get; set; }
    }
}