using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFormConditions : Query<IEnumerable<FormConditionModel>>, IFormConditionCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}