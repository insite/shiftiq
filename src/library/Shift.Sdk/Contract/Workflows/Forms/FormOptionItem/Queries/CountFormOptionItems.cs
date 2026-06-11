using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFormOptionItems : Query<int>, IFormOptionItemCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
