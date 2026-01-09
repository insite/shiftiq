using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFormConditions : Query<IEnumerable<FormConditionMatch>>, IFormConditionCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}