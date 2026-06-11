using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFormOptionLists : Query<int>, IFormOptionListCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
