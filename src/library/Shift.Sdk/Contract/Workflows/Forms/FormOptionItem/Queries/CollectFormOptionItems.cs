using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFormOptionItems : Query<IEnumerable<FormOptionItemModel>>, IFormOptionItemCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
