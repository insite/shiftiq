using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCaseGroups : Query<int>, ICaseGroupCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
