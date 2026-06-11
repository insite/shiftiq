using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFormOptionItems : Query<IEnumerable<FormOptionItemMatch>>, IFormOptionItemCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
