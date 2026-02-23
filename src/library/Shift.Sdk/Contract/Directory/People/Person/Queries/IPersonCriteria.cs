using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPersonCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? UserId { get; set; }

        string EmailExact { get; set; }
        string EventRole { get; set; }
        string FullName { get; set; }
    }
}