using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFormConditions : Query<int>, IFormConditionCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}