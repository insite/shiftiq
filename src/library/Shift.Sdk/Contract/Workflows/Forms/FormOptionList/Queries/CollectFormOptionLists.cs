using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFormOptionLists : Query<IEnumerable<FormOptionListModel>>, IFormOptionListCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
