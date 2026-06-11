using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFormOptionItemCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationId { get; set; }
    }
}
