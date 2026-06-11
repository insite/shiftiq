using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IUserFieldCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationId { get; set; }
    }
}
