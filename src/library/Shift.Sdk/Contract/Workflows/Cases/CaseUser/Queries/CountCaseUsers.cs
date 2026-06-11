using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCaseUsers : Query<int>, ICaseUserCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
