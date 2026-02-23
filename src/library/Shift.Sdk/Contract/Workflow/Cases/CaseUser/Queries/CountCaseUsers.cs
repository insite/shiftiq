using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCaseUsers : Query<int>, ICaseUserCriteria
    {
        public Guid? CaseId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        public string CaseRole { get; set; }
    }
}