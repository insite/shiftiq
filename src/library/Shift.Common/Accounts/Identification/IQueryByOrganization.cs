using System;

namespace Shift.Common
{
    public interface IQueryByOrganization
    {
        Guid? OrganizationId { get; set; }
    }
}
