using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCaseUsers : Query<int>, ICaseUserCriteria
    {
        public Guid? CaseIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string CaseRole { get; set; }
    }
}